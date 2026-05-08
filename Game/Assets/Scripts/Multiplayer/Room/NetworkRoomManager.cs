using System;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkRoomManager : NetworkBehaviour, IPlayerJoined, IPlayerLeft
{
    public Button ready_play;
    public Button leaveButton;
    public RectTransform groupPlayer;
    public TextMeshProUGUI NameRoomText;
    [SerializeField] PlayerElementRoomNetwork PlayerElementPrefab;
    Dictionary<PlayerRef, PlayerElementRoomNetwork> players = new();

    private void Awake()
    {
        leaveButton.onClick.AddListener(Leave);
        ready_play.onClick.AddListener(Ready_Play);
        
    }


    void OnAllReady()
    {
        // Certainly is server 
        ready_play.image.color = Color.black;
        ready_play.GetComponentInChildren<TextMeshProUGUI>().text = "<color=green>Play</color>";
        ready_play.interactable = true;
    }

    public override void Spawned()
    {
        NameRoomText.text = Runner.SessionInfo.Name;
        if (HasStateAuthority)
        {
            Debug.Log("NetworkRoom Spawned in server");
            ready_play.GetComponentInChildren<TextMeshProUGUI>().text = "<color=black>Play</color>";
            ready_play.image.color = Color.white;
            ready_play.interactable = false;
        }
        else
        {

            Debug.Log("NetworkRoom Spawned in client");
            ready_play.GetComponentInChildren<TextMeshProUGUI>().text = "<Color=green>Ready</color>";
        }
    }

    public void Leave()
    {
      if(Runner != null)
        {
            Runner.Shutdown();
        }
    }
    public void Ready_Play()
    {
        if (Runner.IsServer)
        {
            Runner.LoadScene("GameScene");
        }
        else
        {
            var obj = Runner.GetPlayerObject(Runner.LocalPlayer);
            if (obj.TryGetComponent<PlayerElementRoomNetwork>(out var play)) 
            {
                if (play.isReady)
                {

                    Debug.Log("Client do not ready");
                    RPC_OnReadyChange(Runner.LocalPlayer, false);
                    ready_play.GetComponentInChildren<TextMeshProUGUI>().text = "<color=green>Ready</color>";
                    
                }
                else
                {
                    Debug.Log("Client Ready");
                    RPC_OnReadyChange(Runner.LocalPlayer, true);
                    ready_play.GetComponentInChildren<TextMeshProUGUI>().text = "<color=black>Cancel</color>";
                }
            }

        }
    }


    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_OnReadyChange(PlayerRef player, bool isReady)
    {
        Debug.Log("Client Request status changed");
        RPC_UpdateStatusChangeClient(player, isReady);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_UpdateStatusChangeClient(PlayerRef player, bool isReady)
    {
        Debug.Log("Server receive request status change");
        var obj = Runner.GetPlayerObject(player);
        if(obj.TryGetComponent<PlayerElementRoomNetwork>(out var element))
        {
            element.isReady = isReady;
        }
    }


    public void PlayerJoined(PlayerRef player)
    {
        Debug.Log("PlayerJoin callback from NetworkRoomManager");
        if (HasStateAuthority)
        {
            var obj = Runner.Spawn(PlayerElementPrefab, inputAuthority: player, onBeforeSpawned: (Runner, netObj) => {
                if (netObj.TryGetComponent<PlayerElementRoomNetwork>(out var play))
                {
                    Runner.SetPlayerObject(player, netObj);
                    play.OnAllReady += OnAllReady;                    
                }
            });
            players.Add(player, obj);
        }

    }
    
    public void PlayerLeft(PlayerRef player)
    {
        Debug.Log("Player Left");
        if (HasStateAuthority)
        {
            Debug.Log("PlayerLeft callback from NetworkRoomManager");
          if(players.TryGetValue(player, out var playerLeft))
           {
                Runner.Despawn(playerLeft.GetComponent<NetworkObject>());
                players.Remove(player);
            }
        }
    }   

    public Dictionary<PlayerRef, PlayerElementRoomNetwork> GetPlayers() => players;


}
