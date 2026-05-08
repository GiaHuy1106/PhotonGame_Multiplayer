using System.Collections.Generic;
using Fusion; // Thêm namespace của Fusion
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyNetworkBase : NetworkBehaviour // Kế thừa NetworkBehaviour
{
    public enum EnemyState { Idle, Patrol, Chasing, Attacking, GetHit, Die }

    [Header("Networked State")]
    [Networked, OnChangedRender(nameof(OnStateChanged))]
    public EnemyState currentState { get; set; }

    [Networked]
    public float health { get; set; }

    [Header("Stats")]
    public float walkSpeed = 2f;
    public float chaseSpeed = 5f;

    [Header("Patrol Settings")]
    public Transform[] waypoints;
    public float waitTimeAtWaypoint = 2f;
    [Networked] private int currentWaypointIndex { get; set; }
    [Networked] private float waitTimer { get; set; }

    [Header("Detection & Combat")]
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    [Networked] private TickTimer lastAttackTimer { get; set; } // Dùng TickTimer thay cho float Time

    [Header("Animation Settings")]
    public float getHitDuration = 0.5f;
    [Networked] private TickTimer hitTimer { get; set; }

    private Transform playerTarget;
    private NavMeshAgent agent;
    private Animator animator;
    public float TimeToDie = 5f;

    public override void Spawned()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        // Khởi tạo giá trị mặc định chỉ trên State Authority
        if (Object.HasStateAuthority)
        {
            health = 100f;
            if (waypoints.Length > 0) currentState = EnemyState.Patrol;
            else currentState = EnemyState.Idle;
        }
    }

    // FixedUpdateNetwork chạy trên Server và thực hiện dự đoán
    public override void FixedUpdateNetwork()
    {
        // CHỈ logic trên State Authority (Server/Host) mới được tính toán AI
        if (!Object.HasStateAuthority) return;
        if (currentState == EnemyState.Die) return;

        FindPlayer();

        // Nếu không có player, đứng im hoặc tuần tra tùy ý
        float distanceToPlayer = playerTarget != null ? Vector3.Distance(transform.position, playerTarget.position) : float.MaxValue;

        switch (currentState)
        {
            case EnemyState.Idle: HandleIdle(distanceToPlayer); break;
            case EnemyState.Patrol: HandlePatrol(distanceToPlayer); break;
            case EnemyState.Chasing: HandleChasing(distanceToPlayer); break;
            case EnemyState.Attacking: HandleAttacking(distanceToPlayer); break;
            case EnemyState.GetHit: HandleGetHitState(); break;
        }
    }

    // Logic xử lý di chuyển và trạng thái (Chỉ chạy trên Server)
    #region Server Logic
    private void HandleIdle(float dist)
    {
        agent.isStopped = true;
        if (dist <= detectionRange) currentState = EnemyState.Chasing;
        else if (waypoints.Length > 0) currentState = EnemyState.Patrol;
    }

    private void HandlePatrol(float dist)
    {
        if (dist <= detectionRange) { currentState = EnemyState.Chasing; return; }

        agent.isStopped = false;
        agent.speed = walkSpeed;
        if (waypoints.Length == 0) return;

        agent.SetDestination(waypoints[currentWaypointIndex].position);

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            waitTimer += Runner.DeltaTime;
            if (waitTimer >= waitTimeAtWaypoint)
            {
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
                waitTimer = 0f;
            }
        }
    }

    private void HandleChasing(float dist)
    {
        if (playerTarget == null) { currentState = EnemyState.Patrol; return; }

        agent.isStopped = false;
        agent.speed = chaseSpeed;
        agent.SetDestination(playerTarget.position);

        if (dist <= attackRange) currentState = EnemyState.Attacking;
        else if (dist > detectionRange) currentState = EnemyState.Patrol;
    }

    private void HandleAttacking(float dist)
    {
        agent.isStopped = true;
        if (playerTarget == null) { currentState = EnemyState.Chasing; return; }

        FaceTarget();

        if (lastAttackTimer.ExpiredOrNotRunning(Runner))
        {
            // Trigger Animation qua RPC hoặc Networked Var (Ở đây dùng RPC cho Trigger)
            RPC_PlayAttackEffects();
            lastAttackTimer = TickTimer.CreateFromSeconds(Runner, attackCooldown);

            // Thực hiện gây sát thương thực tế trên Server
            //ApplyDamageToPlayer();
        }

        if (dist > attackRange) currentState = EnemyState.Chasing;
    }

    private void HandleGetHitState()
    {
        agent.isStopped = true;
        if (hitTimer.Expired(Runner)) currentState = EnemyState.Chasing;
    }
    #endregion

    // Đồng bộ hóa Visual/Animation cho tất cả Clients
    void OnStateChanged()
    {
        if (animator == null) return;

        switch (currentState)
        {
            case EnemyState.GetHit: animator.SetTrigger("GetHit"); break;
            case EnemyState.Die: animator.SetTrigger("Die"); break;
        }
    }

    public override void Render()
    {
        // Cập nhật Speed Animation mượt mà trên mọi máy
        if (animator != null && agent != null)
        {
            float speedMagnitude = agent.velocity.magnitude;
            animator.SetFloat("Speed", speedMagnitude, 0.1f, Time.deltaTime);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_PlayAttackEffects()
    {
        if (animator != null) animator.SetTrigger("Attack");
        // Các hiệu ứng âm thanh, VFX nhẹ có thể gọi ở đây
    }

    public void TakeDamage(float damage)
    {
        if (!Object.HasStateAuthority || currentState == EnemyState.Die) return;

        health -= damage;
        if (health <= 0) Die();
        else
        {
            currentState = EnemyState.GetHit;
            hitTimer = TickTimer.CreateFromSeconds(Runner, getHitDuration);
        }
    }

    private void Die()
    {
        currentState = EnemyState.Die;
        agent.enabled = false;
        // Runner.Despawn thay vì Destroy
        Runner.Despawn(Object);
    }

    [Header("Detection Setup")]
    [SerializeField] private LayerMask playerLayer; // Đảm bảo Layer của Player được thiết lập đúng
    private readonly List<LagCompensatedHit> _hits = new List<LagCompensatedHit>();

    private void FindPlayer()
    {
        // Chỉ thực hiện quét trên State Authority
        if (!Object.HasStateAuthority) return;

        // Xóa danh sách hit cũ
        _hits.Clear();

        // Quét các đối tượng trong phạm vi detectionRange
        // Dùng LagCompensation giúp việc xác định vị trí chính xác hơn kể cả khi có độ trễ
        int hitCount = Runner.LagCompensation.OverlapSphere(
            transform.position,
            detectionRange,
            Object.InputAuthority,
            _hits,
            playerLayer,
            HitOptions.IncludePhysX // Bao gồm cả các vật thể có collider vật lý
        );

        float closestDistance = float.MaxValue;
        Transform temporaryTarget = null;

        for (int i = 0; i < hitCount; i++)
        {
            var hit = _hits[i];

            // Kiểm tra xem đối tượng va chạm có phải là Player không
            // Thông thường chúng ta kiểm tra component NetworkObject hoặc NetworkPlayer
            if (hit.GameObject != null)
            {
                float distance = Vector3.Distance(transform.position, hit.GameObject.transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    temporaryTarget = hit.GameObject.transform;
                }
            }
        }

        // Cập nhật mục tiêu
        playerTarget = temporaryTarget;
    }

    private void FaceTarget()
    {
        if (playerTarget == null) return;
        Vector3 direction = (playerTarget.position - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Runner.DeltaTime * 5f);
        }
    }
    private EnemyNetworkManager _spawnManager;

    // Hàm này sẽ được gọi từ Manager ngay khi Spawn
    public void SetupEnemy(EnemyNetworkManager manager)
    {
        // Lưu tham chiếu để báo cáo khi chết
        _spawnManager = manager;

        // Nếu bạn muốn khởi tạo các thông số Networked khác
        if (Object.HasStateAuthority)
        {
            // Ví dụ: Reset máu về đầy
            this.health = 100f;

            // Thiết lập trạng thái mặc định ban đầu
            this.currentState = EnemyState.Idle;

            // Đảm bảo NavMeshAgent được bật và đặt đúng vị trí
            if (TryGetComponent<NavMeshAgent>(out var agent))
            {
                agent.enabled = true;
                agent.velocity = Vector3.zero;
            }

            Debug.Log($"Enemy {Object.Id} đã được thiết lập bởi Manager.");
        }
    }
}