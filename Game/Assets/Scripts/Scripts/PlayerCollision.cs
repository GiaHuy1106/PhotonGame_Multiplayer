using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public PlayerMovement playerMovement;
    //public PlayerHealth playerHealth;

    private float hitCooldown = 0.3f; // ✅ NOTE: thêm cooldown
    private float hitTimer = 0f;

    void Update()
    {
        if (hitTimer > 0) hitTimer -= Time.deltaTime; // ✅ NOTE
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hitTimer > 0) return; // ✅ NOTE: chặn spam

        if(hit.gameObject.CompareTag("Axe") || hit.gameObject.CompareTag("Blade"))
        {
            //playerHealth.TakeDamage(15);

            playerMovement.ApplyKnockback(hit.transform.position);

            hitTimer = hitCooldown; // ✅ NOTE: reset cooldown
        }
    }
}