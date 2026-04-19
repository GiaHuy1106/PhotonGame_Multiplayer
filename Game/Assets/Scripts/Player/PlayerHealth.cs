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
        Debug.Log("Player còn: " + currentHealth + " máu");

        if (animator != null) animator.SetTrigger("GetHit");

        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        if (animator != null) animator.SetTrigger("Die");
        GetComponent<PlayerMovement>().enabled = false;
    }
}