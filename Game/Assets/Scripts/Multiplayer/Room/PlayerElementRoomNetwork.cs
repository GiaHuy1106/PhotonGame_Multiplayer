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
    RectTransform parent;

   public void OnStatusChanged()
    {
        if (HasStateAuthority)
        {
            int cnt = 0;
            foreach (var client in Runner.ActivePlayers)
            {
                if (Runner.GetPlayerObject(client).TryGetComponent<PlayerElementRoomNetwork>(out var playOBJ))
                {
                    if (!playOBJ.isReady) return;
                    cnt++;
                }
            }
            if (cnt == Runner.SessionInfo.MaxPlayers)
            {
                OnAllReady?.Invoke();
            }
        }
        Debug.Log("OnStatus changed: " + isReady);
        if (isReady)
        {
            status.color = Color.green;
        }
        else
        {
            status.color = Color.red;
        }
    }

    public void SetParent(RectTransform parent)
    {
        Debug.Log("PlayerElementRoom Setparent");
        this.parent = parent;
    }
    public override void Spawned()
    {
        transform.SetParent(parent);

        if (Runner.IsServer && Runner.LocalPlayer == Object.InputAuthority)
        {
            Debug.Log("PlayerElementRoom spawned on server");
            isReady = true;
        }
        if (HasInputAuthority)
        {
            Debug.Log("PlayerEelemntRoom Spawned on client");
            RPC_RequestUserID(NetworkRunnerHandler.Ins.nickNamePlayer);            
        }
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    void RPC_RequestUserID(string userID)
    {
        RPC_SetUserID(userID);
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_SetUserID(string userID)
    {
        nickNametext.text = userID;
    }
}
