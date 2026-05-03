using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkRunnerHandler : MonoBehaviour
{
    public NetworkRunner networkRunnerPrefab;
    NetworkRunner _runner;
    public static NetworkRunnerHandler Ins;
    public string nickNamePlayer;
    public event Action<List<SessionInfo>> OnListSessionUpdate;
    public bool isJoinLobby { get; private set; } = false;

    private void Awake()
    {
        if(Ins != null && Ins != this)
        {
            Destroy(gameObject);
            return;
        }
        Ins = this;
        DontDestroyOnLoad(gameObject);
        _runner = Instantiate(networkRunnerPrefab);
        
    }

    
    public async void JoinLobby(Action onJoinSuccess, Action onJoinFailed)
    {


        if (isJoinLobby)
        {
            onJoinSuccess?.Invoke();
            return;
        }
        if(_runner == null)
        {
            _runner = Instantiate(networkRunnerPrefab);
        }
        var clientTask = await _runner.JoinSessionLobby(SessionLobby.Custom, "OurLobbyID");
        if (clientTask.Ok)
        {
            isJoinLobby = true;
            Debug.Log("JoinLobby successfull");
            onJoinSuccess?.Invoke();
        }
        else
        {
            Debug.Log($"JoinLobby not successfull");
            onJoinFailed?.Invoke();
        }
    }
    public void CreateSession(string nameRoom, Dictionary<string, SessionProperty> properties = null)
    {
        
    }

    public void UpdateSession(List<SessionInfo> listSession)
    {
        OnListSessionUpdate?.Invoke(listSession);
    }
    INetworkSceneManager GetSceneManager(NetworkRunner runner)
    {
      var sceneManager = runner.GetComponent<INetworkSceneManager>();
        if(sceneManager == null)
        {
           sceneManager = runner.AddComponent<NetworkSceneManagerDefault>();
        }
        return sceneManager;
    }

    protected virtual Task<StartGameResult> InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode,string sessionName, byte[] connectionToken, NetAddress address , Scene scene, System.Action<NetworkRunner> initialized = null)
    {
        var sceneManager = GetSceneManager(runner);
        
        return runner.StartGame(
            new StartGameArgs()
            {
                GameMode = gameMode,
                Address = address,
                Scene = SceneRef.FromIndex(scene.buildIndex),
                SessionName = sessionName,
                SceneManager = sceneManager,
                OnGameStarted = initialized,
                ConnectionToken = connectionToken,
                CustomLobbyName = "OurLobbyID",
                
                
            }
            );
    }

    public void CleanUpOnNetworkRunnerShutdown()
    {
        _runner = null;
    }
}
