using System;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerElementRoomNetwork : NetworkBehaviour
{
    [SerializeField] TextMeshProUGUI nickNametext;
    [SerializeField] Image status;
    [Networked, OnChangedRender(nameof(OnStatusChanged))]
    public bool isReady { get; set; }
    public event Action OnAllReady;
    [Networked, OnChangedRender(nameof(OnNickNameChanged))]
    public NetworkString<_32> nickName { get; set; }
    NetworkRoomManager roomManager;
   public void OnStatusChanged()
    {
        if (HasStateAuthority)
        {
            CheckAllReadyToPlay();
        }
        Debug.Log(gameObject.name + "OnStatus changed: " + isReady);
        if (isReady)
        {
            status.color = Color.green;
        }
        else
        {
            status.color = Color.red;
        }
    }
    public void OnNickNameChanged()
    {
        Debug.Log(gameObject.name + ": OnNickNameChanged");
        nickNametext.text = nickName.ToString();
    }

    public override void Spawned()
    {
        gameObject.name = $"Player: {Runner.LocalPlayer}";
        roomManager = FindAnyObjectByType<NetworkRoomManager>();
        transform.SetParent(roomManager.groupPlayer);
        if (!Object.HasStateAuthority) { 
            OnStatusChanged();
            OnNickNameChanged();
        }
        if (Runner.IsServer && Runner.LocalPlayer == Object.InputAuthority)
        {
            Debug.Log(gameObject.name + ": PlayerElementRoom spawned on server");
            isReady = true;
        }
        if (HasInputAuthority)
        {
            Debug.Log(gameObject.name + ": PlayerEelemntRoom Spawned on client");
            Utils.Delay1Frame(() => RPC_RequestUserName(NetworkRunnerHandler.Ins.nickNamePlayer));                      
        }
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    void RPC_RequestUserName(string userName)
    {
        Debug.Log(gameObject.name + ": RequestUserID called");
        this.nickName = userName;
    }
   

    void CheckAllReadyToPlay()
    {       
            int cnt = 0;
            foreach (var client in Runner.ActivePlayers)
            {
                if (roomManager.GetPlayers().TryGetValue(client, out var playerElement))
                {
                    if (!playerElement.isReady) continue;
                    cnt++;
                }
            }
            if (cnt == Runner.SessionInfo.MaxPlayers)
            {
                OnAllReady?.Invoke();
            }
        else
        {
            Button ready_play = roomManager.ready_play;
            ready_play.interactable = false;
            ready_play.image.color = Color.white;
            ready_play.GetComponentInChildren<TextMeshProUGUI>().text = "<color=black>Play</color>";
        }    
    }
}
