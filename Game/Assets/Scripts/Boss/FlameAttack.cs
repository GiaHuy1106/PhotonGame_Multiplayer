using System.Collections.Generic;
using Fusion;
using Fusion.LagCompensation;
using UnityEngine;

public class FlameAttack : NetworkBehaviour
{
    [SerializeField] BoxCollider box;
    [SerializeField] float durationDamage = 0.2f;
    [SerializeField] LayerMask layerDamage;
    [SerializeField] float damage = 5f;
    [Networked]
    public bool IsFlameThrow { get; set; }
    BoxOverlapQueryParams queryParams = new();
    List<LagCompensatedHit> hits = new();
    [Networked]
    TickTimer coolDownDamage { get; set; }
   
    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;
        if(!IsFlameThrow) return;

        if (!coolDownDamage.ExpiredOrNotRunning(Runner)) return;

        ApplyDamage();    
        coolDownDamage = TickTimer.CreateFromSeconds(Runner, durationDamage);
        
    }


    void ApplyDamage()
    {

        queryParams.Center = box.bounds.center;
        queryParams.Extents = box.bounds.extents;

        var query = new BoxOverlapQuery(ref queryParams)
        {
            Options = HitOptions.IncludePhysX,
            LayerMask = layerDamage,
        };
        int hitCount = Runner.LagCompensation.OverlapBox(query, hits);
        if (hitCount <= 0) return;

        foreach (var hit in hits)
        {
            // Lấy GameObject từ hitbox Fusion hoặc PhysX collider
            var go = hit.Hitbox?.Root?.gameObject ?? hit.Collider?.gameObject;
            if (go == null) continue;

            // Tìm NetworkObject ở cả component trực tiếp lẫn parent
            var netObj = go.GetComponentInParent<NetworkObject>();
            if (netObj == null) continue;

            // Apply damage nếu implement ITakeDamageable
            if (netObj.TryGetComponent<ITakeDamageable>(out var target))
                target.TakeDamage(damage);
        }

    }
    public void StartFlameThrower_AnimEvent()
    {
        if (HasStateAuthority)
        {
            IsFlameThrow = true;
        }
    }
    public void StopFlameThrower_AnimEvent()
    {
        if (HasStateAuthority)
        {
            IsFlameThrow = false;
        }
    }
}