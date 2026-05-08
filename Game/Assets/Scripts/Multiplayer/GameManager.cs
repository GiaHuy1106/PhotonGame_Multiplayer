using Fusion;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [SerializeField] NetworkPlayer playerPrefab;

    public void SpawnPlayer(PlayerRef player)
    {

        RPC_RequestSpawn(player);
    }


    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_RequestSpawn(PlayerRef player)
    {
        
        Runner.Spawn(playerPrefab, Utils.GetRandomPosition(), inputAuthority: player, onBeforeSpawned: (Runner, newOBJ) => 
        {
            Runner.SetPlayerObject(player, newOBJ);
        });
    }
}
