using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(BossDragonStats))]
public class BossDragonAI : MonoBehaviour
{
    public enum BossState { Idle, Chasing, Attacking, GetHit, Die }

    public BossState currentState = BossState.Idle;
    public float detectionRange = 25f;
    public float fireRange = 15f;
    public float meleeRange = 5f;

    public float attackCooldown = 3f;   
    public float lastAttackTime = 0f;

    [Header("FireAtack")]
    public GameObject fireColumnObject;

    private float hitTimer = 0f;
    private float nextGrowlTime = 0f;
    private Transform playerTarget;
    private NavMeshAgent agent;
    private Animator animator;
    private BossDragonStats stats;
    public bool isAttacking = false;      
    public bool isBreathingFire = false;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        stats = GetComponent<BossDragonStats>();

        agent.speed = 3.5f;
    }

    void Update()
    {
        if (currentState == BossState.Die) return;

        FindPlayer();
        if (playerTarget == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

        switch (currentState)
        {
            case BossState.Idle:
                agent.isStopped = true;
                if (Time.time >= nextGrowlTime)
                {
                    GetComponent<EnemyFX>().PlayDragonBossGrawl();
                    nextGrowlTime = Time.time + Random.Range(4f, 10f);
                }
                if (distanceToPlayer <= detectionRange)
                {
                    currentState = BossState.Chasing;
                    if (stats != null && stats.healthUI != null)
                        stats.healthUI.ShowUI();
                }
                break;

            case BossState.Chasing:
                agent.isStopped = false;
                agent.SetDestination(playerTarget.position);

                if (distanceToPlayer <= fireRange)
                {
                    currentState = BossState.Attacking;
                }
                else if (distanceToPlayer > detectionRange)
                {
                    stats.healthUI.HideUI();
                    currentState = BossState.Idle;
                }
                break;

            case BossState.Attacking:
                 agent.isStopped = true;
                if (!isAttacking)
                {
                    FaceTarget();
                }
                if (isAttacking) break;
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    isAttacking = true;
                    if (distanceToPlayer <= meleeRange)
                    {
                        if (animator != null) animator.SetTrigger("MeleeAttack");
                    }
                    else if (distanceToPlayer <= fireRange)
                    {
                        if (animator != null) animator.SetTrigger("FireAttack");
                        isBreathingFire = true;
                    }

                    lastAttackTime = Time.time;
                }

                if (!isAttacking && distanceToPlayer > fireRange) currentState = BossState.Chasing;
                break;

            case BossState.GetHit:
                agent.isStopped = true;
                hitTimer -= Time.deltaTime;
                if (hitTimer <= 0) currentState = BossState.Chasing;
                break;
        }

        UpdateAnimations();
    }

    public void TriggerGetHit(float duration)
    {
        currentState = BossState.GetHit;
        hitTimer = duration;
    }

    public void TriggerDeath()
    {
        currentState = BossState.Die;
        agent.isStopped = true;
        agent.enabled = false;
    }

    public void StartFlamethrower()
    {
        if (fireColumnObject != null)
        {
            fireColumnObject.SetActive(true);
        }
    }
    public void StopFlamethrower()
    {
        if (fireColumnObject != null)
        {
            fireColumnObject.SetActive(false);
        }
    }
    public void ForceStopFireBreath()
    {
        StopFlamethrower();
        isBreathingFire = false;
    }
    public void ResetAttackState()
    {
        isAttacking = false;
        if (isBreathingFire)
        {
            ForceStopFireBreath();
        }
    }
    private void FindPlayer()
    {
        if (playerTarget == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) playerTarget = p.transform;
        }
    }

    private void FaceTarget()
    {
        Vector3 direction = (playerTarget.position - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 2.5f);
        }
    }
    public void TriggerMeleeDamage()
    {
        if (playerTarget != null && stats != null)
        {
            float dist = Vector3.Distance(transform.position, playerTarget.position);
            if (dist <= meleeRange + 1f)
            {
                PlayerHealth pHealth = playerTarget.GetComponent<PlayerHealth>();
                if (pHealth != null)
                {
                    pHealth.TakeDamage(stats.damage);
                    Vector3 hitPoint = playerTarget.position + Vector3.up * 1f;
                    GetComponent<EnemyFX>().SpawnHitVFX(hitPoint);
                    GetComponent<EnemyFX>().PlayDragonClaw();
                }
            }
        }
    }
    private void UpdateAnimations()
    {
        if (animator == null) return;
        float speedMagnitude = agent.velocity.magnitude;
        animator.SetFloat("Speed", speedMagnitude, 0.1f, Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.magenta; Gizmos.DrawWireSphere(transform.position, fireRange);
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, meleeRange);
    }
}
