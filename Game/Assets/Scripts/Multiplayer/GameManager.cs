using Fusion;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    
    [SerializeField] NetworkPlayer playerPrefab;
    [SerializeField] Transform spawnPoint;
    public static GameManager Ins;

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            Ins = this;
        }
    }

    public void SpawnPlayer(PlayerRef player)
    {
        RPC_RequestSpawn(player);
    }


    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_RequestSpawn(PlayerRef player)
    {
        
        Runner.Spawn(playerPrefab, Utils.GetRandomArroundPoint(spawnPoint.position), inputAuthority: player, onBeforeSpawned: (Runner, newOBJ) => 
        {
            Runner.SetPlayerObject(player, newOBJ);
        });
    }




}
