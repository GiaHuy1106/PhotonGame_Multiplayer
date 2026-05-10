using System.Collections.Generic;
using Fusion;
using Fusion.LagCompensation;
using UnityEngine;
using static Unity.Collections.Unicode;

public class SwordDamage : NetworkBehaviour
{
    public float damageAmount = 20f;
    private bool canDamage = false;
    [SerializeField] BoxCollider boxCollider;
    [SerializeField] LayerMask layerTakeDamage;

    private List<LagCompensatedHit> hits = new();
    private HashSet<NetworkId> alreadyTakeDamage = new HashSet<NetworkId>();

    public void EnableDamage()
    {
        alreadyTakeDamage.Clear(); // Reset để có thể gây sát thương cho đợt chém mới
        canDamage = true;
    }

    public void DisableDamage()
    {
        canDamage = false;
    }

    public override void FixedUpdateNetwork()
    {
        if (canDamage)
        {
            PerformOverlapCheck();
        }
    }

    void PerformOverlapCheck()
    {
        // Lấy thông số từ BoxCollider
        Vector3 worldCenter = boxCollider.transform.TransformPoint(boxCollider.center);
        Vector3 halfExtents = boxCollider.size * 0.5f;
        Quaternion worldRotation = boxCollider.transform.rotation;

        var queryParams = new BoxOverlapQueryParams
        {
            Center = worldCenter,
            Extents = halfExtents,
            Rotation = worldRotation,
        };
        var query = new BoxOverlapQuery(ref queryParams);
        query.LayerMask = layerTakeDamage;
        query.Options = HitOptions.IgnoreInputAuthority;

        hits.Clear();
        // Gọi trực tiếp params sẽ gọn hơn
        if (query == null)
        {
            Debug.Log("Quey null");
        }
        if(hits == null)
        {
            Debug.Log("hits null");
        }
        if(Runner == null)
        {
            Debug.Log("Runner is Null");
        }
        int hitCount = Runner.LagCompensation.OverlapBox(query, hits);

        for (int i = 0; i < hitCount; i++)
        {
            var hit = hits[i];
            NetworkObject rootObj = null;

            // Xác định NetworkObject để lấy ID
            if (hit.Hitbox != null) rootObj = hit.Hitbox.Root.Object;
            else rootObj = hit.GameObject.GetComponentInParent<NetworkObject>();

            if (rootObj != null && !alreadyTakeDamage.Contains(rootObj.Id))
            {
                if (hit.GameObject.TryGetComponent<ITakeDamageable>(out var takeDamageable))
                {
                    takeDamageable.TakeDamage(damageAmount);
                    alreadyTakeDamage.Add(rootObj.Id);
                }
            }
        }
    }
}