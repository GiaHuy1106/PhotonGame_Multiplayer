using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    public enum EnemyState { Idle, Patrol, Chasing, Attacking, GetHit, Die }

    [Header("State Control")]
    public EnemyState currentState = EnemyState.Idle;

    [Header("Stats")]
    public float health = 100f;
    public float walkSpeed = 2f;
    public float chaseSpeed = 5f;

    [Header("Patrol Settings")]
    public Transform[] waypoints;
    public float waitTimeAtWaypoint = 2f;
    private int currentWaypointIndex = 0;
    private float waitTimer = 0f;

    [Header("Detection & Combat")]
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    private float lastAttackTime = 0f;

    [Header("Animation Settings")]
    public float getHitDuration = 0.5f;
    private float hitTimer = 0f;

    private Transform playerTarget;
    private NavMeshAgent agent;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        if (waypoints.Length > 0) currentState = EnemyState.Patrol;
    }

    void Update()
    {
        if (currentState == EnemyState.Die) return;

        FindPlayer();
        if (playerTarget == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

        switch (currentState)
        {
            case EnemyState.Idle:
                HandleIdle(distanceToPlayer);
                break;
            case EnemyState.Patrol:
                HandlePatrol(distanceToPlayer);
                break;
            case EnemyState.Chasing:
                HandleChasing(distanceToPlayer);
                break;
            case EnemyState.Attacking:
                HandleAttacking(distanceToPlayer);
                break;
            case EnemyState.GetHit:
                HandleGetHitState();
                break;
        }

        UpdateAnimations();
    }

    private void HandleIdle(float dist)
    {
        agent.isStopped = true;

        if (dist <= detectionRange) currentState = EnemyState.Chasing;
        else if (waypoints.Length > 0) currentState = EnemyState.Patrol;
    }

    private void HandlePatrol(float dist)
    {
        if (dist <= detectionRange)
        {
            currentState = EnemyState.Chasing;
            return;
        }

        agent.isStopped = false;
        agent.speed = walkSpeed;

        if (waypoints.Length == 0) return;

        agent.SetDestination(waypoints[currentWaypointIndex].position);

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtWaypoint)
            {
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
                waitTimer = 0f;
            }
        }
    }

    private void HandleChasing(float dist)
    {
        agent.isStopped = false;
        agent.speed = chaseSpeed;
        agent.SetDestination(playerTarget.position);

        if (dist <= attackRange) currentState = EnemyState.Attacking;
        else if (dist > detectionRange) currentState = EnemyState.Patrol;
    }

    private void HandleAttacking(float dist)
    {
        agent.isStopped = true;
        FaceTarget();

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            if (animator != null) animator.SetTrigger("Attack");
            lastAttackTime = Time.time;
        }

        if (dist > attackRange) currentState = EnemyState.Chasing;
    }

    public void TriggerAttackDamage()
    {
        if (playerTarget != null)
        {
            float dist = Vector3.Distance(transform.position, playerTarget.position);
            if (dist <= attackRange + 0.5f)
            {
                PlayerHealth pHealth = playerTarget.GetComponent<PlayerHealth>();
                if (pHealth != null) pHealth.TakeDamage(10f);
            }
        }
    }

    private void HandleGetHitState()
    {
        agent.isStopped = true;

        hitTimer -= Time.deltaTime;
        if (hitTimer <= 0)
        {
            currentState = EnemyState.Chasing;
        }
    }

    public void TakeDamage(float damage)
    {
        if (currentState == EnemyState.Die) return;

        health -= damage;

        if (health <= 0)
        {
            Die();
        }
        else
        {
            if (animator != null) animator.ResetTrigger("Attack");
            if (animator != null) animator.SetTrigger("GetHit");

            currentState = EnemyState.GetHit;
            hitTimer = getHitDuration;
        }
    }

    private void Die()
    {
        currentState = EnemyState.Die;
        agent.isStopped = true;
        agent.enabled = false;

        if (animator != null) animator.SetTrigger("Die");
        Destroy(gameObject, 5f);
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
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}