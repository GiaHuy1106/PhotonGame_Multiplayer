using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public CharacterController controller;

    public float knockbackForce = 8f;
    public float knockbackDuration = 0.2f;

    private Vector3 knockbackVelocity;
    private float knockbackTimer;

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Axe") || hit.gameObject.CompareTag("Blade"))
        {
            playerHealth.TakeDamage(15);

            // 👉 Tính hướng văng
            Vector3 direction = transform.position - hit.transform.position;
            direction.y = 0; // tránh bị hất lên trời
            direction.Normalize();

            knockbackVelocity = direction * knockbackForce;
            knockbackTimer = knockbackDuration;
        }
    }

    void Update()
    {
        if (knockbackTimer > 0)
        {
            controller.Move(knockbackVelocity * Time.deltaTime);
            knockbackTimer -= Time.deltaTime;
        }
    }
}