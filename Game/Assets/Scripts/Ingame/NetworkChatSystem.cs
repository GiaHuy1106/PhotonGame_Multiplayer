using System;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkChatSystem : NetworkBehaviour, IPlayerLeft, IPlayerJoined
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Queue<string> messageQueue = new Queue<string>();
    [SerializeField] TextMeshProUGUI[] textMessages;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] GameObject Panel;
    public InputActionAsset inputActionAsset;
    public bool isChatting = false;
    private void Awake()
    {
        inputActionAsset.FindActionMap("Player").FindAction("Chat").performed += OnEnter;
        Hide();
    }
    private void OnDestroy()
    {
        inputActionAsset.FindActionMap("Player").FindAction("Chat").performed -= OnEnter;
    }
    private void OnEnter(InputAction.CallbackContext context)
    {
        if (isChatting) return;
        Show();
    }


   

    public void OnEndEdit(string message)
    {
        Debug.Log("OnEnter press");
        if (string.IsNullOrEmpty(message))
        {
            Hide();
            Debug.Log("string is empty");
            return;
        }
        RPC_RequireSendMessage($"[{NetworkRunnerHandler.Ins.nickNamePlayer}]: {message}");
        inputField.ActivateInputField();
        inputField.text = "";
        Debug.Log("OnEnter press end");
    }

    public void OnGameMessageReceived(string message)
    {
        messageQueue.Enqueue(message);
        if (messageQueue.Count > 4)
        {
            messageQueue.Dequeue();
        }
        int queueIndex = 0;
        foreach (string element in messageQueue)
        {
            textMessages[queueIndex].text = element;
            queueIndex++;
        }
    }
    public void Show()
    {
        Debug.Log("Show");
        isChatting = true;
        Panel.SetActive(true);
        inputField.ActivateInputField();
    }
    public void Hide()
    {
        Debug.Log("Hide");
        isChatting = false;
        Panel.SetActive(false);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequireSendMessage(string message)
    {
        RPC_SendMessage(message);
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SendMessage(string message)
    {
        OnGameMessageReceived(message);

    }

    public void PlayerLeft(PlayerRef player)
    {
        if (Object.HasStateAuthority)
        {
            if (Runner.TryGetPlayerObject(player, out var playerObject))
            {
                var nickName = playerObject.GetComponent<NetworkPlayer>().nickName.ToString();
                RPC_SendMessage($"[{nickName}]: has left the game");
            }
        }
    }

    public void PlayerJoined(PlayerRef player)
    {
        Utils.DelayCall(.5f, () =>
        {
            if (Object.HasStateAuthority)
            {
                if (Runner.TryGetPlayerObject(player, out var playerObject))
                {
                    var nickName = playerObject.GetComponent<NetworkPlayer>().nickName.ToString();
                    RPC_SendMessage($"[{nickName}]: has joined the game");
                }
            }
        });

    }
    public override void Spawned()
    {
        if (Runner.IsClient)
        {
            Utils.DelayCall(1f, () =>
            {
                GameObject canvas = GameObject.FindWithTag("MainUI");
                if (canvas != null)
                {
                    this.transform.parent = canvas.transform;
                }
            });
        }
    }
}
