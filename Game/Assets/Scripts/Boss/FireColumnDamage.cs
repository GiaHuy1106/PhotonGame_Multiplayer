using System.Collections.Generic;
using UnityEngine;

public class FireColumnDamage : MonoBehaviour
{
    public float damagePerTick = 10f;
    public float tickInterval = 1f;

    [Header("Visual Effect Settings (Looks Only)")]
    public GameObject burningVFXPrefab;
    public float burnDuration = 4f;
    public Vector3 vfxOffset = new Vector3(0, 0f, 0);
    //private Dictionary<PlayerHealth, float> targetTimers = new Dictionary<PlayerHealth, float>();
    //private List<PlayerHealth> keysBuffer = new List<PlayerHealth>();
    private void OnDisable()
    {
        //targetTimers.Clear();
    }

    private void Update()
    {
        //List<PlayerHealth> targetsToRemove = new List<PlayerHealth>();
        //foreach (KeyValuePair<PlayerHealth, float> target in targetTimers)
        //{
        //    float newTimer = target.Value + Time.deltaTime;

        //    if (newTimer >= tickInterval)
        //    {
        //        if (target.Key != null)
        //        {
        //            target.Key.TakeDamage(damagePerTick);
        //        }
        //        newTimer = 0f;
        //    }
        //    targetsToRemove.Add(target.Key);
        //}
    }

    private void EfficientUpdate()
    {
        //keysBuffer.Clear();
        //keysBuffer.AddRange(targetTimers.Keys); 

        //for (int i = 0; i < keysBuffer.Count; i++)
        //{
        //    PlayerHealth player = keysBuffer[i];
        //    if (player == null) 
        //    { 
        //        targetTimers.Remove(player); 
        //        continue; 
        //    }

        //    targetTimers[player] += Time.deltaTime;
        //    if (targetTimers[player] >= tickInterval)
        //    {
        //        player.TakeDamage(damagePerTick);
        //        targetTimers[player] = 0f;
        //    }
        //}
    }
    void LateUpdate()
    {
        EfficientUpdate();
    }
    private void OnTriggerEnter(Collider other)
    {
        //if (other.CompareTag("Player"))
        //{
        //    PlayerHealth player = other.GetComponent<PlayerHealth>();
        //    if (player != null && !targetTimers.ContainsKey(player))
        //    {
        //        targetTimers.Add(player, 0f);
        //        player.TakeDamage(damagePerTick);
        //        ApplyBurnVisual(other.gameObject);
        //    }
        //}
    }
    private void ApplyBurnVisual(GameObject target)
    {
        BurnEffectVisual existingBurn = target.GetComponent<BurnEffectVisual>();

        if (existingBurn == null)
        {
            BurnEffectVisual newBurn = target.AddComponent<BurnEffectVisual>();
            Animator anim = target.GetComponentInChildren<Animator>();
            Transform spine = anim != null ? anim.GetBoneTransform(HumanBodyBones.Spine) : target.transform;

            newBurn.Initialize(burnDuration, burningVFXPrefab, vfxOffset, spine);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        //if (other.CompareTag("Player"))
        //{
        //    PlayerHealth player = other.GetComponent<PlayerHealth>();
        //    if (player != null && targetTimers.ContainsKey(player))
        //    {
        //        targetTimers.Remove(player);
        //    }
        //}
    }
}
