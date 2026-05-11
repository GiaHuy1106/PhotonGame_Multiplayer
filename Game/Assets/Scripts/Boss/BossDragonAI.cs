using System;
using System.Collections.Generic;
using Fusion;
using Fusion.LagCompensation;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BossDragonAI : NetworkBehaviour, ITakeDamageable
{
    public enum BossState { Idle, Patrol, Chasing, Attacking, GetHit, Die }

    [Header("State Control")]
    [Networked, OnChangedRender(nameof(OnStateChanged))]
    public BossState currentState { get; set; } = BossState.Idle;

    [Header("Stats")]
    [SerializeField] float maxHealth = 100f;
    [SerializeField] HitboxRoot hitboxRoot;
    [Networked, OnChangedRender(nameof(OnHealthChanged))]
    public float health { get; set; }

    [Networked] public bool IsDead { get; set; }

    public float walkSpeed = 2f;
    public float chaseSpeed = 3.5f;

    [Header("Patrol Settings")]
    public Transform[] waypoints;
    public float waitTimeAtWaypoint = 5f;
    [Networked] private int currentWaypointIndex { get; set; }
    [Networked] private TickTimer waitTimer { get; set; }

    [Header("Detection & Combat")]
    public float detectionRange = 20f;
    public float attackMeleeRange = 5f;
    public float meleeDamage = 15f;
    public float fireRange = 15f;
    public float attackMeleeCoolDown = 1f;
    public float attackFlameCoolDown = 2.34f;
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
    List<LagCompensatedHit> hits = new();
    public event Action OnBossDie;
    // Trong Multiplayer, ta lưu ID hoặc tham chiếu thay vì GameObject.Find liên tục
    [Networked] private NetworkObject playerTarget { get; set; }


    private void Awake()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        if (hitboxRoot == null)
        {
            hitboxRoot = GetComponent<HitboxRoot>();
        }
    }
    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            health = maxHealth;
            if (waypoints.Length > 0) currentState = BossState.Patrol;
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
            case BossState.Idle: HandleIdle(distanceToPlayer); break;
            case BossState.Patrol: HandlePatrol(distanceToPlayer); break;
            case BossState.Chasing: HandleChasing(distanceToPlayer); break;
            case BossState.Attacking: HandleAttacking(distanceToPlayer); break;
            case BossState.GetHit: HandleGetHitState(); break;
        }
    }

    // --- LOGIC XỬ LÝ TRẠNG THÁI ---

    private void HandleIdle(float dist)
    {
        if (dist <= detectionRange) currentState = BossState.Chasing;
        else if (waypoints.Length > 0) currentState = BossState.Patrol;
    }

    private void HandlePatrol(float dist)
    {
        if (dist <= detectionRange) { currentState = BossState.Chasing; return; }

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
        if (playerTarget == null)
        {
            currentState = BossState.Patrol;
            return;
        }
        agent.isStopped = false;
        agent.speed = chaseSpeed;
        agent.SetDestination(playerTarget.transform.position);

        if (dist <= fireRange) currentState = BossState.Attacking;
        else if (dist > detectionRange) currentState = BossState.Patrol;
    }

    private void HandleAttacking(float dist)
    {
        agent.isStopped = true;
        if (attackCooldownTimer.ExpiredOrNotRunning(Runner) && IsAttacking)
        {
            IsAttacking = false;
        }
        // Quay mặt về phía player
        if (playerTarget != null)
        {
            Vector3 dir = (playerTarget.transform.position - transform.position).normalized;
            dir.y = 0;
            if (dir != Vector3.zero) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Runner.DeltaTime * 5f);
        }

        if (attackCooldownTimer.ExpiredOrNotRunning(Runner) && !IsAttacking)
        {            
            if (dist < attackMeleeRange)
            {
                BasicAttacking();
            } else if (dist <= fireRange)
            {
                FlameAttacking();
            }
        }
        if (!IsAttacking)
        {
            if(playerTarget == null || dist > fireRange + .5f){
                IsAttacking = false;
                agent.isStopped = false;
                currentState = BossState.Chasing;
            }
        }       
    }
    [Networked] bool IsAttacking { get; set; }

    void BasicAttacking()
    {
        IsAttacking = true;
        Debug.Log("Attack");
        attackCooldownTimer = TickTimer.CreateFromSeconds(Runner, attackMeleeCoolDown);
        // Trigger Animation qua RPC hoặc Networked Var (ở đây dùng Trigger cho đơn giản)
        var queryParams = new SphereOverlapQueryParams
        {
            Center = transform.position,
            Radius = attackMeleeRange,
        };

        SphereOverlapQuery query = new SphereOverlapQuery(ref queryParams);
        query.Options = HitOptions.IncludePhysX;
        query.LayerMask = layerDamage;
        int hitCount = Runner.LagCompensation.OverlapSphere(
         query, hits
     );
        if (hitCount > 0)
        {
            foreach (var hit in hits)
            {
                NetworkObject obj = null;
                if (hit.Hitbox != null)
                {
                    obj = hit.GameObject.GetComponent<NetworkObject>();
                }
                else if (hit.Collider != null)
                {
                    obj = hit.GameObject.GetComponent<NetworkObject>();
                }
                Debug.Log("hit null: " + hit != null);
                if (obj != null && obj.TryGetComponent<ITakeDamageable>(out var takeDamageOBJ))
                {
                    takeDamageOBJ.TakeDamage(meleeDamage);
                }
            }
        }
        RPC_PlayAttackAnim();
    }

    public void AnimationFinishAttacking_Event()
    {
        if(HasStateAuthority){
            IsAttacking = false;
            Debug.Log("Finish Attacking");
        }
    }
    void FlameAttacking()
    {
        Debug.Log("FlameAttacking called! IsAttacking was: " + IsAttacking);
        IsAttacking = true;
        attackCooldownTimer = TickTimer.CreateFromSeconds(Runner, attackFlameCoolDown);
        RPC_PlayFlameAnimation();
    }





    private void HandleGetHitState()
    {
        agent.isStopped = true;
        if (hitStateTimer.Expired(Runner))
        {
            currentState = BossState.Chasing;
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
        else if(!IsAttacking)
        {

            currentState = BossState.GetHit;
            hitStateTimer = TickTimer.CreateFromSeconds(Runner, getHitDuration);

            RPC_PlayGetHitAnim();
        }
    }

    private void Die()
    {
        if (IsDead) return;
        hitboxRoot.HitboxRootActive = false;
        GetComponent<Collider>().enabled = false;
        IsDead = true;
        currentState = BossState.Die;
        despawnTimer = TickTimer.CreateFromSeconds(Runner, TimeToDie);
        OnBossDie?.Invoke();
        // Tắt vật lý trên Server
        agent.enabled = false;
        if (TryGetComponent<Collider>(out var c)) c.enabled = false;
    }

    // --- HELPER FUNCTIONS ---

    private void FindNearestPlayer()
    {
        if (Runner.Tick % 30 != 0 && playerTarget != null) return;

        float closestDist = float.MaxValue;
        NetworkObject nearest = null; // Dùng biến tạm, không đụng playerTarget trong loop

        foreach (var player in Runner.ActivePlayers)
        {
            var pObj = Runner.GetPlayerObject(player);

            // Check null TRƯỚC
            if (pObj == null) continue;

            var hp = pObj.GetBehaviour<HPHandler>();
            if (hp == null || hp.IsDead) continue;

            float d = Vector3.Distance(transform.position, pObj.transform.position);
            if (d < closestDist)
            {
                closestDist = d;
                nearest = pObj;
            }
        }

        // Gán một lần duy nhất sau loop
        playerTarget = nearest; // null nếu không tìm được ai
    }


    // --- ĐỒNG BỘ HIỆU ỨNG (RPC) ---

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_PlayAttackAnim() { if (animator) animator.Play("Basic Attack"); }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_PlayFlameAnimation()
    {
        if (animator)
        {
            animator.Play("Flame Attack");
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_PlayGetHitAnim() { if (animator) animator.Play("GetHit"); }


    // --- ON CHANGED CALLBACKS ---  
    void OnStateChanged()
    {
        if (animator == null) return;

        Debug.Log(currentState.ToString());
        switch (currentState)
        {
            case BossState.Idle:
                break;
            case BossState.Patrol:
                animator.Play("Walk");
                break;
            case BossState.Chasing:
                animator.Play("Run");
                break;
            case BossState.Die:
                enemyHealth.Invisible();
                GetComponent<Collider>().enabled = false;
                animator.Play("Die");
                break;
        }
    }
    void OnHealthChanged()
    {
        if (enemyHealth)
            enemyHealth.UpdateHP(health, maxHealth);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackMeleeRange);
    }
}


