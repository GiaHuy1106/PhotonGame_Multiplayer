using System.Linq;
using Fusion;
using UnityEditor.Rendering;
using UnityEngine;

public class GameManager : NetworkBehaviour, IPlayerLeft
{
    public NetworkChatSystem chatSystem;
    [SerializeField] NetworkPlayer playerPrefab;
    [SerializeField] Transform spawnPoint;
    [SerializeField] float range = 2f;
    [Header("Enemies")]
    [SerializeField] EnemyAI[] enemies;
    [SerializeField] BossDragonAI bossDragonAI;
    public Transform[] wayPoints;
    public Transform[] wayPointsBoss;
    public static GameManager Ins;
    [SerializeField] GameObject warningboss;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] AudioClip warningBoss;
    [SerializeField] AudioSource source;
    [SerializeField] AudioClip bossFight;
    public Vector3 SpawnPoint => spawnPoint.position;
    int spawnCount = 0;
    int enemyNextWave = 4;
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
            Runner.Spawn(enemies[Random.Range(0, spawnCount)], wayPoints[Random.Range(0, wayPoints.Length - 1)].position, onBeforeSpawned:(Runner, newEnemy) => {
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
            Runner.Spawn(bossDragonAI, onBeforeSpawned: (Runner, no) => { 
                BossDragonAI boss = no.GetBehaviour<BossDragonAI>();
                boss.OnBossDie += OnBossDie;
                boss.waypoints = wayPointsBoss;
            });
        }
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_RequestSpawn(PlayerRef player)
    {
        if (Runner.GetPlayerObject(player))
        {
            Debug.Log("Player has object");
            return;
        }
        Runner.Spawn(playerPrefab,Utils.GetRandomAroundPoint(spawnPoint.position), inputAuthority: player, onBeforeSpawned: (Runner, newOBJ) => 
        {
            HPHandler hp = newOBJ.GetBehaviour<HPHandler>();
            hp.OnDead += OnPlayerDead;
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
            if (HasStateAuthority)
                RPC_WarningBossUI();
            Utils.DelayCall(5f, () => {                
                SpawnBoss();

            }
            );
        }
        
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_WarningBossUI()
    {
        warningboss?.SetActive(true);
        if(warningboss != null && source != null)
        {
            source.volume = .5f;
            source.PlayOneShot(warningBoss);
        }
        Utils.DelayCall(4f, () => { 
            warningboss?.SetActive(false);
            Utils.DelayCall(1f, () => {
                if(source != null && bossFight != null)
                {
                    source.clip = bossFight;
                    source.loop = true;
                    source.volume = 0.4f;
                    source.Play();
                }
            });
        });
    }


    void OnBossDie()
    {
        if(HasStateAuthority)
            RPC_ActiveVictoryPanel();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_ActiveVictoryPanel()
    {
        Cursor.lockState = CursorLockMode.None;
        source?.Stop();
        Utils.DelayCall(2f, () => {
            gameOverPanel.SetActive(true);
            gameOverPanel.transform.GetChild(0).gameObject.SetActive(true);
        });
       
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (HasStateAuthority)
        {
            Debug.Log("Player: " + player + " has left the room");
            Runner.Despawn(Runner.GetPlayerObject(player));
        }
    }

    void OnPlayerDead()
    {
        if (!HasStateAuthority) return;
        int count = Runner.ActivePlayers.Select(x => Runner.GetPlayerObject(x)).Where(j => j.GetComponent<HPHandler>().IsDead).Count();
        if (count == Runner.ActivePlayers.Count())
        {
            RPC_ActiveDefeatPanel();
        }
    }


    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_ActiveDefeatPanel()
    {
        source?.Stop();
        Cursor.lockState = CursorLockMode.None;
        Utils.DelayCall(2f, () => {
            gameOverPanel.SetActive(true);
            gameOverPanel.transform.GetChild(1).gameObject.SetActive(true);
        });
       
    }


    public void OnContinueButton()
    {
        Runner.Shutdown();        
    }
}
