using System;
using System.Collections.Generic;
using System.Text;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

public class Spawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] PlayerElementRoomNetwork PlayerElementRoomNetworkPrefab;
   
    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("Connected to server");
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        Debug.Log("OnClient Request");
        string jsonToken = Encoding.UTF8.GetString(token);
        ConnectToken tokenClient = JsonUtility.FromJson<ConnectToken>(jsonToken);
        if (runner.SessionInfo.Properties.TryGetValue("HasPassword", out var hasPassword))
        {
            if(NetworkRunnerHandler.Ins.passwordRoom == tokenClient.password)
            {
                request.Accept();
            }
            else
            {
                request.Refuse();
            }
        }
        else
        {
            request.Accept();
        }
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        Debug.Log("DisconnectFromServer: "+ runner.UserId + "\t" + reason);
        NetworkRunnerHandler.Ins.LeaveRoomScene(reason);
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {

    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("OnPlayerJoind callback from spawner");
     
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("Playerleft callback from spawner");
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {

    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {        
            NetworkRunnerHandler.Ins.UpdateSession(sessionList);
        Debug.Log("SesionListUPdate");
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        NetworkRunnerHandler.Ins.OnShutdown();
        NetworkRunnerHandler.Ins.JoinSessionFailed(shutdownReason);
        Debug.Log("OnShutdown: " + shutdownReason);
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {

    }
    
}
