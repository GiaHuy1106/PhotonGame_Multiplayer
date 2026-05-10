using System.Collections.Generic;
using System.Linq;
using Fusion;
using Fusion.LagCompensation;
using NUnit.Framework;
using UnityEngine;

public class DoorNetwork : NetworkBehaviour, IInteractable
{
    bool isPlayerNear;
    [SerializeField] GameObject floatingText;
    [SerializeField] Animator doorOpen;
    [Networked, OnChangedRender(nameof(ChangeOpen))]
    public bool IsOpen { get; set; }
    [SerializeField]
    BoxCollider box;
    HashSet<NetworkObject> _objectsInZone = new HashSet<NetworkObject>();
    List<NetworkObject> hitsObject = new List<NetworkObject>();
    [SerializeField] LayerMask layerMask;

    public override void Spawned()
    {
        floatingText.SetActive(false);   
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
        var overlaps = Physics.OverlapBox(
            box.bounds.center,
            box.bounds.extents,
            transform.rotation,
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
    


    private void HandleEnter(NetworkObject no) {
            isPlayerNear = true;
        if(no.HasInputAuthority)
            floatingText.SetActive(true);
        
    }
    private void HandleExit(NetworkObject no) 
    {
            isPlayerNear = false;
     
        if(no.HasInputAuthority)
        {
            floatingText.SetActive(false);
        }
    }

    void ChangeOpen()
    {
        if (IsOpen)
        {
            if (HasStateAuthority && GameManager.Ins != null)
            {
                GameManager.Ins.SpawndEnenmy();
            }
            doorOpen.SetTrigger("Open");
        }
    }

    public void Interact()
    {
        if(isPlayerNear && Object.HasStateAuthority && !IsOpen)
        {
            IsOpen = true;
        }

    }
}
