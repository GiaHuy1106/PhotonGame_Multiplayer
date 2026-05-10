using Fusion;
using UnityEngine;

public class PlayerInteract : NetworkBehaviour
{
    [SerializeField] private LayerMask interactLayer;
    
    public override void FixedUpdateNetwork()
    {
        if (!GetInput(out NetworkInputData inputData)) return;

        if (inputData.isInteract)
        {
            Debug.Log("TryInteract");
            TryInteract();
        }
    }

    private void TryInteract()
    {
        if (Object.HasStateAuthority)
        {
            Debug.Log("Server Called TryInteract");
        }
        if (Object.HasInputAuthority)
        {
            Debug.Log("Client called TryInteract");
        }
        var hits = Physics.OverlapSphere(transform.position, 3f, interactLayer);
        if(hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<IInteractable>(out var interactable))
                {
                    interactable.Interact(Object);
                }
            }
        }
    }
}