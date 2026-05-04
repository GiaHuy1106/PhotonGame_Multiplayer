using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    private Animator animator;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponentInChildren<Animator>();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (animator != null) animator.SetTrigger("GetHit");
        Debug.Log("Player c�n: " + currentHealth + " m�u");
        
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        if (animator != null) animator.SetTrigger("Die");
        GetComponent<PlayerMovement>().enabled = false;
    }
}