using Fusion;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{

    [SerializeField] Transform _camera;
    [SerializeField] Transform cinemachine;

    public static NetworkPlayer Local;
    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Local = this;
        }
        else
        {

        }
    }
    
}
