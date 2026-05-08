using System;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SessionListInfoItem : MonoBehaviour
{
    public TextMeshProUGUI sessionNameText;
    public TextMeshProUGUI playerCountText;
    public Button joinButton;
    SessionInfo sessionInfo;
    public GameObject lockImage;
    public event Action<SessionInfo> OnJoinSession;
    public void SetInformation(SessionInfo sessionInfo)
    {
        if(sessionInfo.Properties.TryGetValue("HasPassword", out var hasPass))
        {
            lockImage.SetActive(true);
        }
        this.sessionInfo = sessionInfo;
        sessionNameText.text = sessionInfo.Name;
        playerCountText.text = $"{sessionInfo.PlayerCount}/{sessionInfo.MaxPlayers}";
        bool isJoinButtonActive = true;
        if (sessionInfo.PlayerCount>= sessionInfo.MaxPlayers) 
        { 
            isJoinButtonActive = false;
        }
        joinButton.gameObject.SetActive(isJoinButtonActive);
    }

    public void OnClick()
    {
        OnJoinSession?.Invoke(sessionInfo);
    }
}
