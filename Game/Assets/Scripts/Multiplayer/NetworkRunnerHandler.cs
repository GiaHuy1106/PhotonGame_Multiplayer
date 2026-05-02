using System.Linq;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkRunnerHandler : MonoBehaviour
{
    public NetworkRunner networkRunner;
    NetworkRunner _runner;
    [SerializeField] Button joinLobby;

    private void Awake()
    {
        _runner = Instantiate(networkRunner);
        joinLobby.onClick.AddListener(() => JoinGame());
    }

    public void JoinGame()
    {
        _runner.JoinSessionLobby(SessionLobby.Shared);

    }

    public async Task StartGame()
    {
       if(_runner == null)
        {
            _runner = Instantiate(networkRunner);
        }   
        var task = await InitializeNetworkRunner(_runner, GameMode.Host, NetAddress.Any(), SceneManager.GetActiveScene(), (runner) =>
        {
            Debug.Log("ConnectSuccess");
        });
        if(task.Ok)
        {
            Debug.Log("Game Started");
        }
        else
        {
            Debug.Log("Failed to start game");
        }
    }

    protected virtual Task<StartGameResult> InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode, NetAddress address, Scene scene, System.Action<NetworkRunner> initialized)
    {
        var sceneManager = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();
        if (sceneManager == null)
        {

            sceneManager = runner.AddComponent<NetworkSceneManagerDefault>();
        }

        return runner.StartGame(
            new StartGameArgs()
            {
                GameMode = gameMode,
                Address = address,
                Scene = SceneRef.FromIndex(scene.buildIndex),
                SessionName = "TestRoom",
                SceneManager = sceneManager,
                OnGameStarted = initialized,

            }
            );
    }

}
