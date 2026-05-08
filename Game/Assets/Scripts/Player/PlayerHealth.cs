using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    private Animator animator;
    public PlayerHealthUI healthUI;
    void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponentInChildren<Animator>();
        if (healthUI != null)
        {
            healthUI.SetupMaxHealth(maxHealth);
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log("Player con: " + currentHealth + " mau");
        if (healthUI != null) healthUI.UpdateHealth(currentHealth);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (animator != null) animator.SetTrigger("GetHit");
        Debug.Log("Player con: " + currentHealth + " mau");
        if (healthUI != null) healthUI.UpdateHealth(currentHealth);
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        if (animator != null) animator.SetTrigger("Die");
        GetComponent<PlayerMovement>().enabled = false;
    }
}
