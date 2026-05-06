using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Fusion;
using Fusion.Photon.Realtime;
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
    public event Action<ShutdownReason> OnJoinSessionFailed;
    public bool isJoinLobby { get; private set; } = false;
    public string passwordRoom;
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

    
    
    public async void JoinLobby(Action<bool> OnProcess)
    {

        if (_runner == null)
        {
            _runner = Instantiate(networkRunnerPrefab);
        }
        if (isJoinLobby)
        {
            OnProcess?.Invoke(true);
            return;
        }
      
        var clientTask = await _runner.JoinSessionLobby(SessionLobby.Custom, "OurLobbyID");
        if (clientTask.Ok)
        {
            isJoinLobby = true;
            Debug.Log("JoinLobby successfull");
            OnProcess?.Invoke(true);
        }
        else
        {
            Debug.Log($"JoinLobby not successfull");
            OnProcess?.Invoke(false);
        }
    }
    public async void CreateSession(string nameRoom, Dictionary<string, SessionProperty> properties = null, Action<bool> callbackProcess = null)
    {
        
        ConnectToken token = new ConnectToken
        {
            ID = StartUp.IDtoken,
            NickName = nickNamePlayer,
            password = passwordRoom,
        };
         var clientTask = await InitializeNetworkRunner(_runner, GameMode.Host, nameRoom,  NetAddress.Any(), Encoding.UTF8.GetBytes(JsonUtility.ToJson(token)), 2, 1, properties );
        if (clientTask.Ok)
        {
            Debug.Log("CreateSession ok");
            callbackProcess?.Invoke(true);
        }
        
        else
        {
            callbackProcess?.Invoke(false);
            Debug.Log("Failded CreateSession");
            Debug.Log(clientTask.ShutdownReason);
        }
    }
    List<SessionInfo> temp;
    public SessionInfo LookingSession(string name)
    {
        if(temp != null)
        {
            return temp.Find(x => x.Name == name);
        }
        return null;
    }
    public void UpdateSession(List<SessionInfo> listSession)
    {
        temp = listSession;
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
    public async void JoinSession(string name, string password = null, Action<bool, ShutdownReason> callbackProcess = null)
    {
        ConnectToken token = new ConnectToken { 
            ID = StartUp.IDtoken,
            password = password,
            NickName = nickNamePlayer
        };
        byte[] connectToken = Encoding.UTF8.GetBytes(JsonUtility.ToJson(token));
        var clientTask = await InitializeNetworkRunner(_runner, GameMode.Client, name, NetAddress.Any(), connectToken);
        if (clientTask.Ok)
        {
            callbackProcess?.Invoke(true, clientTask.ShutdownReason);
            Debug.Log($"JoinSession: {name}");
        }
        else
        {
            Debug.Log($"Error while join session {name} - Reaason: {clientTask.ShutdownReason}");
            callbackProcess?.Invoke(false, clientTask.ShutdownReason);
        }
    }
    protected virtual Task<StartGameResult> InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode,string sessionName,  NetAddress address, byte[] connectionToken, int playerCount = 2,  int sceneIndex = 0,Dictionary<string, SessionProperty> pros = null, System.Action<NetworkRunner> initialized = null)
    {
        if(runner == null)
        {
            runner = Instantiate(networkRunnerPrefab);
        }
        var sceneManager = GetSceneManager(runner);
        var appsetting = new FusionAppSettings();
        var clone = PhotonAppSettings.Global.AppSettings.CopyTo(appsetting);
        appsetting.FixedRegion = "hk";
        return runner.StartGame(
            new StartGameArgs()
            {
                
                GameMode = gameMode,
                Address = address,
                Scene = SceneRef.FromIndex(sceneIndex),
                SessionName = sessionName,
                SceneManager = sceneManager,
                OnGameStarted = initialized,
                ConnectionToken = connectionToken,
                CustomLobbyName = "OurLobbyID",
                PlayerCount = playerCount,
                SessionProperties = pros,
                CustomPhotonAppSettings = appsetting
            }
            );
    }

    public void JoinSessionFailed(ShutdownReason reason)
    {
        Debug.Log("JoinSessionFaild");
        isJoinLobby = false;
        OnJoinSessionFailed?.Invoke(reason);
        
    }
    public void RequestRefeshLobby(Action<List<SessionInfo>> callback)
    {
        callback?.Invoke(temp);
    }
    public void OnShutdown()
    {
        CleanUp();
        SceneManager.LoadScene("SetupScene");
        _runner = Instantiate(networkRunnerPrefab);
    }
    void CleanUp()
    {
        passwordRoom = null;
        _runner = null;
        temp = null;
    }
    public void LeaveRoomScene(NetDisconnectReason reason)
    {

    }
}
