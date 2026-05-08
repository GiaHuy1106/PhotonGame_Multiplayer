using Fusion;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public NetworkChatSystem chatSystem;
    [SerializeField] NetworkPlayer playerPrefab;
    [SerializeField] Transform spawnPoint;
    public static GameManager Ins;


    public override void Spawned()
    {
        Ins = this;

    }
    public void SpawnPlayer(PlayerRef player)
    {
        RPC_RequestSpawn(player);
    }


    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_RequestSpawn(PlayerRef player)
    {        
        Runner.Spawn(playerPrefab,spawnPoint.position, inputAuthority: player, onBeforeSpawned: (Runner, newOBJ) => 
        {
            Runner.SetPlayerObject(player, newOBJ);
        });
    }




}
