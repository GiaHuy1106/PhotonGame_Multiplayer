using Fusion;
using UnityEditor.Rendering;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public NetworkChatSystem chatSystem;
    [SerializeField] NetworkPlayer playerPrefab;
    [SerializeField] Transform spawnPoint;
    [SerializeField] float range = 2f;
    [Header("Enemies")]
    [SerializeField] EnemyAI[] enemies;
    [SerializeField] BossDragonAI bossDragonAI;
    public Transform[] wayPoints;
    public static GameManager Ins;
    public Vector3 SpawnPoint => spawnPoint.position;
    int spawnCount = 0;
    int enemyNextWave = 3;
    int enemyOfWave = 0;

    public override void Spawned()
    {
        Ins = this;
    }


    public override void FixedUpdateNetwork()
    {
        /// test SpawnEnemy
        if (GetInput(out NetworkInputData inputData))
        {
            //if (inputData.isJump)
            //{
            //    Debug.Log("Runner Spawn enemy");
            //    if (HasStateAuthority)
            //    {
            //        Runner.Spawn(enemies[Random.Range(0, enemies.Length - 1)], wayPoint[Random.Range(0, wayPoint.Length - 1)].position);
            //    }
            //}
        }
    }
    public void SpawnPlayer(PlayerRef player)
    {
        RPC_RequestSpawn(player);
    }

    public void SpawndEnenmy()
    {
        if (!HasStateAuthority)
        {
            return;
        }
        spawnCount++;
        for (int i = 0; i < enemyNextWave; i ++)
        {
            Runner.Spawn(enemies[Random.Range(0, enemies.Length - 1)], wayPoints[Random.Range(0, wayPoints.Length - 1)].position, onBeforeSpawned:(Runner, newEnemy) => {
                EnemyAI agent = newEnemy.GetBehaviour<EnemyAI>();
                agent.OnEnemyDie += OnEnemyDie;
                agent.waypoints = wayPoints;
            });
            enemyOfWave++;
        }
        enemyNextWave += 1;
    }


    public void SpawnBoss()
    {
        if (Object.HasStateAuthority)
        {
            Runner.Spawn(bossDragonAI);
        }
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_RequestSpawn(PlayerRef player)
    {        
        Runner.Spawn(playerPrefab,Utils.GetRandomAroundPoint(spawnPoint.position), inputAuthority: player, onBeforeSpawned: (Runner, newOBJ) => 
        {
            Runner.SetPlayerObject(player, newOBJ);
        });
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(spawnPoint.position, range);
    }

    void OnEnemyDie()
    {
        enemyOfWave--;
        if(enemyOfWave == 0 && spawnCount == 3)
        {
            Utils.DelayCall(5f, () => SpawnBoss());
        }
    }
}
