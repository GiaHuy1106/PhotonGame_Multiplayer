using System;
using System.Collections.Generic;
using Fusion;
using Fusion.LagCompensation;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : NetworkBehaviour, ITakeDamageable
{
    public enum EnemyState { Idle, Patrol, Chasing, Attacking, GetHit, Die }

    [Header("State Control")]
    [Networked, OnChangedRender(nameof(OnStateChanged))]
    public EnemyState currentState { get; set; } = EnemyState.Idle;

    [Header("Stats")]
    [SerializeField] float maxHealth = 100f;
    [Networked, OnChangedRender(nameof(OnHealthChanged))]
    public float health { get; set; }

    [Networked] public bool IsDead { get; set; }

    public float walkSpeed = 2f;
    public float chaseSpeed = 5f;

    [Header("Patrol Settings")]
    public Transform[] waypoints;
    public float waitTimeAtWaypoint = 2f;
    [Networked] private int currentWaypointIndex { get; set; }
    [Networked] private TickTimer waitTimer { get; set; }

    [Header("Detection & Combat")]
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    public float damage = 5f;
    public LayerMask layerDamage;
    [Networked] private TickTimer attackCooldownTimer { get; set; }

    [Header("Timers")]
    public float getHitDuration = 0.5f;
    [Networked] private TickTimer hitStateTimer { get; set; }
    public float TimeToDie = 3f;
    [Networked] private TickTimer despawnTimer { get; set; }

    [Header("References")]
    public EnemyHealth enemyHealth;
    [SerializeField] NavMeshAgent agent;
    private Animator animator;
    private EnemyLootDrop lootDrop;
    private EnemyFX enemyFX;
    List<LagCompensatedHit> hits = new();
    public event Action OnEnemyDie;
    // Trong Multiplayer, ta lưu ID hoặc tham chiếu thay vì GameObject.Find liên tục
    [Networked] private NetworkObject playerTarget { get; set; }

    private void Awake()
    {
        if(agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
    }
    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            health = maxHealth;
            if (waypoints.Length > 0) currentState = EnemyState.Patrol;
        }
        if (waypoints.Length == 0)
        {
            waypoints = GameManager.Ins.wayPoints;
        }
    }

    public override void FixedUpdateNetwork()
    {
        // 1. Kiểm tra nếu đã chết và chờ Despawn
        if (IsDead)
        {
            if (despawnTimer.Expired(Runner) && HasStateAuthority)
            {
                Runner.Despawn(Object);
            }
            return;
        }

        // 2. Chỉ Server (State Authority) mới xử lý AI Logic
        if (HasStateAuthority)
        {
            UpdateAI();
        }
    }

    private void UpdateAI()
    {
        FindNearestPlayer();

        float distanceToPlayer = playerTarget != null
            ? Vector3.Distance(transform.position, playerTarget.transform.position)
            : float.MaxValue;

        switch (currentState)
        {
            case EnemyState.Idle: HandleIdle(distanceToPlayer); break;
            case EnemyState.Patrol: HandlePatrol(distanceToPlayer); break;
            case EnemyState.Chasing: HandleChasing(distanceToPlayer); break;
            case EnemyState.Attacking: HandleAttacking(distanceToPlayer); break;
            case EnemyState.GetHit: HandleGetHitState(); break;
        }
    }

    // --- LOGIC XỬ LÝ TRẠNG THÁI ---

    private void HandleIdle(float dist)
    {
        if (dist <= detectionRange) currentState = EnemyState.Chasing;
        else if (waypoints.Length > 0) currentState = EnemyState.Patrol;
    }

    private void HandlePatrol(float dist)
    {
        if (dist <= detectionRange) { currentState = EnemyState.Chasing; return; }

        if (waypoints.Length == 0) return;

        agent.isStopped = false;
        agent.speed = walkSpeed;
        agent.SetDestination(waypoints[currentWaypointIndex].position);

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (waitTimer.IsRunning == false)
                waitTimer = TickTimer.CreateFromSeconds(Runner, waitTimeAtWaypoint);

            if (waitTimer.Expired(Runner))
            {
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
                waitTimer = TickTimer.None; // Reset timer
            }
        }
    }

    private void HandleChasing(float dist)
    {
        if (playerTarget == null) { currentState = EnemyState.Patrol; return; }
        agent.isStopped = false;
        agent.speed = chaseSpeed;
        agent.SetDestination(playerTarget.transform.position);

        if (dist <= attackRange) currentState = EnemyState.Attacking;
        else if (dist > detectionRange) currentState = EnemyState.Patrol;
    }

    private void HandleAttacking(float dist)
    {
        agent.isStopped = true;

        // Quay mặt về phía player
        if (playerTarget != null)
        {
            Vector3 dir = (playerTarget.transform.position - transform.position).normalized;
            dir.y = 0;
            if (dir != Vector3.zero) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Runner.DeltaTime * 5f);
        }

        if (attackCooldownTimer.ExpiredOrNotRunning(Runner))
        {
            // Trigger Animation qua RPC hoặc Networked Var (ở đây dùng Trigger cho đơn giản)
            var queryParams = new SphereOverlapQueryParams { 
                Center = transform.position,
                Radius = attackRange,              
            };

            SphereOverlapQuery query = new SphereOverlapQuery(ref queryParams);
            query.Options = HitOptions.IncludePhysX;
            query.LayerMask = layerDamage;
            int hitCount = Runner.LagCompensation.OverlapSphere(
             query, hits
         );
            if (hits.Count > 0 )
            {
                foreach (var hit in hits)
                {
                    NetworkObject obj = null;
                    if (hit.Hitbox != null)
                    {
                        obj = hit.GameObject.GetComponent<NetworkObject>();
                    }else if (hit.Collider != null)
                    {
                        obj = hit.GameObject.GetComponent<NetworkObject>();
                    }
                    if(obj != null && obj.TryGetComponent<ITakeDamageable>(out var takeDamageOBJ))
                    {
                        takeDamageOBJ.TakeDamage(damage);
                    }
                }
            }
            RPC_PlayAttackEffects();
            attackCooldownTimer = TickTimer.CreateFromSeconds(Runner, attackCooldown);
        }

        if (dist > attackRange + 0.5f) currentState = EnemyState.Chasing;
    }

    private void HandleGetHitState()
    {
        agent.isStopped = true;
        if (hitStateTimer.Expired(Runner))
        {
            currentState = EnemyState.Chasing;
        }
    }

    // --- HỆ THỐNG GÂY SÁT THƯƠNG ---

    public void TakeDamage(float damage)
    {
        if (IsDead || !HasStateAuthority) return;

        health -= damage;

        if (health <= 0)
        {
            Die();
        }
        else
        {
            
            currentState = EnemyState.GetHit;
            hitStateTimer = TickTimer.CreateFromSeconds(Runner, getHitDuration);
            RPC_PlayHitEffects();
        }
    }

    private void Die()
    {
        IsDead = true;
        currentState = EnemyState.Die;
        despawnTimer = TickTimer.CreateFromSeconds(Runner, TimeToDie);
        OnEnemyDie?.Invoke();
        // Tắt vật lý trên Server
        agent.enabled = false;
        if (TryGetComponent<Collider>(out var c)) c.enabled = false;

        if (lootDrop != null) lootDrop.DropLoot();
    }

    // --- HELPER FUNCTIONS ---

    private void FindNearestPlayer()
    {
        // Tối ưu: Chỉ tìm kiếm mỗi giây một lần thay vì mỗi tick
        if (Runner.Tick % 30 != 0 && playerTarget != null) return;

        float closestDist = float.MaxValue;
        foreach (var player in Runner.ActivePlayers)
        {
            var pObj = Runner.GetPlayerObject(player);
            if (pObj != null)
            {
                float d = Vector3.Distance(transform.position, pObj.transform.position);
                if (d < closestDist)
                {
                    closestDist = d;
                    playerTarget = pObj;
                }
            }
        }
    }
   

    // --- ĐỒNG BỘ HIỆU ỨNG (RPC) ---

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_PlayAttackEffects() { if (animator) animator.Play("Attack"); }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_PlayHitEffects() { if (animator) animator.Play("GetHit"); }
   

    // --- ON CHANGED CALLBACKS ---  
    void OnStateChanged()
    {
        if (animator == null) return;

        switch (currentState)
        {
            case EnemyState.Idle:
                animator.Play("Idle");
                break;
            case EnemyState.Patrol:
                animator.Play("Walk");
                break;
            case EnemyState.Chasing:
                animator.Play("Run");
                break;                       
            case EnemyState.Die:
                animator.Play("Die");
                break;
        }
    }
    void OnHealthChanged()
    {
        if (enemyHealth)
            enemyHealth.UpdateHP(health, maxHealth);
    }
}


