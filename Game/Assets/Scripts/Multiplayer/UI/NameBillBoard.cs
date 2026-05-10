using UnityEngine;
using Fusion;

public class NameBillBoard : NetworkBehaviour
{
    Camera targetCamera;

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            this.gameObject.SetActive(false);
        }
        targetCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;

            if (targetCamera == null)
                return;
        }

        transform.forward = targetCamera.transform.forward;
    }
}