using System.Collections.Generic;
using System.Linq;
using Fusion;
using Fusion.LagCompensation;
using NUnit.Framework;
using UnityEngine;

public class Posion : NetworkBehaviour, IInteractable
{
    bool isPlayerNear;
    [SerializeField] float heal = 20f;
    [SerializeField] GameObject pickupIcon;
    [SerializeField] SphereCollider box;
    HashSet<NetworkObject> _objectsInZone = new HashSet<NetworkObject>();
    List<NetworkObject> hitsObject = new List<NetworkObject>();
    [SerializeField] LayerMask layerMask;
    Transform root;
    private void Awake()
    {
        root = pickupIcon.transform.parent;
        
    }
    public override void Spawned()
    {
        pickupIcon.SetActive(false);
    }


    private void Update()
    {
        if (Camera.main != null)
        {
          root.forward = Camera.main.transform.forward;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            NetworkObject obj = other.GetComponent<NetworkObject>();
            if (_objectsInZone.Add(obj))
            {
                HandleEnter(obj);
            }

        }

        // Chỉ xử lý nếu chưa có trong zone
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;
        if (!isPlayerNear) return;
        if (Runner.Tick % 30 != 0 || Runner.IsResimulation) return;

        // Thay LagCompensation bằng Physics thường
        var overlaps = Physics.OverlapSphere(
            box.bounds.center,
            box.radius,
            layerMask
        );

        hitsObject = overlaps
            .Select(x => x.GetComponent<NetworkObject>())
            .Where(x => x != null)
            .ToList();

        var objsExit = _objectsInZone.Except(hitsObject).ToList();

        foreach (var obj in objsExit)
        {
            _objectsInZone.Remove(obj);
            RPC_HandlerExit(obj);
        }

        if (overlaps.Length == 0)
        {
            isPlayerNear = false;
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_HandlerExit(NetworkObject obj)
    {
        _objectsInZone.Remove(obj);
        HandleExit(obj);
    }



    private void HandleEnter(NetworkObject no)
    {
        isPlayerNear = true;
        if (no.HasInputAuthority)
            pickupIcon.SetActive(true);

    }
    private void HandleExit(NetworkObject no)
    {
        isPlayerNear = false;

        if (no.HasInputAuthority)
        {
            pickupIcon.SetActive(false);
        }
    }   

    public void Interact(NetworkObject obj)
    {
        if (isPlayerNear && Object.HasStateAuthority)
        {
            if (obj.TryGetBehaviour<HPHandler>(out var hpPlayer))
            {
                hpPlayer.Heal(heal);
                Runner.Despawn(Object);
            }
        }

    }
}
