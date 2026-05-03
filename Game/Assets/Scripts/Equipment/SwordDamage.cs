using UnityEngine;

public class SwordDamage : MonoBehaviour
{
    public float damageAmount = 20f;
    private bool canDamage = false;
    public void EnableDamage() { canDamage = true; }
    public void DisableDamage() { canDamage = false; }

    private void OnTriggerEnter(Collider other)
    {
        if (canDamage && other.CompareTag("Enemy"))
        {
            BossDragonStats bossStats = other.GetComponent<BossDragonStats>();
            if (bossStats != null)
            {
                bossStats.TakeDamage(damageAmount);
                canDamage = false;
                return; 
            }
            EnemyAI enemy = other.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                enemy.TakeDamage(damageAmount);
                canDamage = false;
            }
        }
    }
}