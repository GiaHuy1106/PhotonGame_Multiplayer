using Fusion;
using UnityEngine;

public class DoorNetwork : NetworkBehaviour, IInteractable
{
    bool isPlayerNear;
    [SerializeField] GameObject floatingText;
    [SerializeField] Animator doorOpen;
    [Networked, OnChangedRender(nameof(ChangeOpen))]
    public bool IsOpen { get; set; }
    public override void Spawned()
    {
        floatingText.SetActive(false);
    }   


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Enter open door");
            isPlayerNear = true;
            if(other.GetComponent<NetworkObject>().HasInputAuthority)
                floatingText.SetActive(true);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Exit Open door");
            isPlayerNear = false;
            if (other.GetComponent<NetworkObject>().HasInputAuthority)
            {
                floatingText.SetActive(false); 
            }
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
