using System.Collections.Generic;
using UnityEngine;

public class FireColumnDamage : MonoBehaviour
{
    public float damagePerTick = 10f;
    public float tickInterval = 1f;

    private Dictionary<PlayerHealth, float> targetTimers = new Dictionary<PlayerHealth, float>();

    private void OnDisable()
    {
        targetTimers.Clear();
    }

    private void Update()
    {
        List<PlayerHealth> targetsToRemove = new List<PlayerHealth>();
        foreach (KeyValuePair<PlayerHealth, float> target in targetTimers)
        {
            float newTimer = target.Value + Time.deltaTime;

            if (newTimer >= tickInterval)
            {
                if (target.Key != null)
                {
                    target.Key.TakeDamage(damagePerTick);
                }
                newTimer = 0f;
            }
            targetsToRemove.Add(target.Key);
        }

        foreach (PlayerHealth p in targetsToRemove)
        {
            if (targetTimers.ContainsKey(p))
            {
                // targetTimers[p] = newTimer; // Cần viết lại logic để lưu newTimer chính xác hơn
            }

        }
    }

    private void EfficientUpdate()
    {
        List<PlayerHealth> playersInFire = new List<PlayerHealth>(targetTimers.Keys);

        foreach (PlayerHealth player in playersInFire)
        {
            if (player == null || player.maxHealth <= 0) 
            {
                targetTimers.Remove(player);
                continue;
            }
            targetTimers[player] += Time.deltaTime;

            if (targetTimers[player] >= tickInterval)
            {
                player.TakeDamage(damagePerTick);
                targetTimers[player] = 0f;
            }
        }
    }
    void LateUpdate()
    {
        EfficientUpdate();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player != null && !targetTimers.ContainsKey(player))
            {
                targetTimers.Add(player, 0f);
                player.TakeDamage(damagePerTick); 
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player != null && targetTimers.ContainsKey(player))
            {
                targetTimers.Remove(player);
            }
        }
    }
}
