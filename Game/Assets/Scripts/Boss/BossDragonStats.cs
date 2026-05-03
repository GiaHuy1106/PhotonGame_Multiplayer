using UnityEngine;

public class BossDragonStats : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 500f;
    public float currentHealth;
    public float damage = 25f;
    public float getHitDuration = 0.8f;
    private Animator animator;
    private BossDragonAI aiScript;


    private void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        aiScript = GetComponent<BossDragonAI>();
    }
    
    public void TakeDamage(float damage)
    {
        if (aiScript != null && aiScript.currentState == BossDragonAI.BossState.Die) return;

        currentHealth -= damage;

        if (currentHealth <= 0f)
        {
            Die();
        }
        else
        {
            if (animator != null)
            {
                animator.ResetTrigger("MeleeAttack");
                animator.ResetTrigger("FireAttack");
                animator.SetTrigger("GetHit");
            }
            if (aiScript != null) aiScript.TriggerGetHit(getHitDuration);
        }
    }

    private void Die()
    {
        if (aiScript != null) aiScript.TriggerDeath();

        if (animator != null) animator.SetTrigger("Die");
        Destroy(gameObject, 10f);
    }
}
