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
    private bool hasTriggered66 = false;
    private bool hasTriggered33 = false;
    public string bossName = "Ancient Dragon";
    public BossHealthUI healthUI;
    private void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        aiScript = GetComponent<BossDragonAI>();

        if (healthUI != null)
        {
            healthUI.SetupBoss(bossName, maxHealth);
        }
    }
    
    public void TakeDamage(float damage)
    {
        if (aiScript != null && aiScript.currentState == BossDragonAI.BossState.Die) return;

        currentHealth -= damage;
        if (healthUI != null)
        {
            healthUI.UpdateHealth(currentHealth);
        }
        float healthPercentage = currentHealth / maxHealth;
        if (currentHealth <= 0f)
        {
            Die();
        }
        else
        {
            if (healthPercentage <= 0.66f && !hasTriggered66)
            {
                TriggerStagger();
                hasTriggered66 = true;
                Debug.Log("Boss còn 2/3 máu");
            }
            else if (healthPercentage <= 0.33f && !hasTriggered33)
            {
                TriggerStagger();
                hasTriggered33 = true;
                Debug.Log("Boss còn 1/3 máu");
            }
        }
    }
    private void TriggerStagger()
    {
        if (animator != null)
        {
            animator.ResetTrigger("MeleeAttack");
            animator.ResetTrigger("FireAttack");
            animator.SetTrigger("GetHit");
            GetComponent<EnemyFX>().PlayGetHitSound();
        }

        if (aiScript != null)
        {

            aiScript.TriggerGetHit(getHitDuration);
        }
    }

    private void Die()
    {
        if (aiScript != null) aiScript.TriggerDeath();

        if (animator != null) animator.SetTrigger("Die");
        if (healthUI != null) healthUI.HideUI();
        GetComponent<EnemyFX>().PlayDieSound();
        Destroy(gameObject, 10f);
    }
}
