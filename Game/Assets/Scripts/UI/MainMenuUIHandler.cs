using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIHandler : MonoBehaviour
{
    [SerializeField] GameObject SetUpPlayerPanel;
    [SerializeField] GameObject LobbyPanel;
    [SerializeField] GameObject CreateSessionPanel;
    [Header("SetUpPlayer")]
    [SerializeField] TMP_InputField playerNameInput;
    [SerializeField] Button Play;
    [Header("LobbyPanel")]
    [SerializeField] Button CreateSession;
    [SerializeField] Button BackSetupButton;
    [Header("CreateSession")]
    [SerializeField] Button backLobbyPanel;
    private void Awake()
    {
        Play.onClick.AddListener(OnPlayClick);
        backLobbyPanel.onClick.AddListener(BackLobbyPanel);
        CreateSession.onClick.AddListener(OnCreateSession);
        BackSetupButton.onClick.AddListener(BackSetupPanel);
    }
    private void Start()
    {
        HideAll();
        SetUpPlayerPanel.SetActive(true);
    }
    public void HideAll()
    {
        SetUpPlayerPanel.SetActive(false);
        LobbyPanel.SetActive(false);
        CreateSessionPanel.SetActive(false);
    }


    public void BackSetupPanel()
    {
        HideAll();
        SetUpPlayerPanel.SetActive(true);
    }
    public void OnPlayClick()
    {
        if (string.IsNullOrEmpty(playerNameInput.text))
        {
            playerNameInput.ActivateInputField();
            return;
        }
        NetworkRunnerHandler.Ins.nickNamePlayer = playerNameInput.text;
        Play.interactable = false;
        NetworkRunnerHandler.Ins.JoinLobby( (value) => 
        {
            if (value)
            {
                HideAll();
                LobbyPanel.SetActive(true);
                Play.interactable = true;
            }
            else
            {
                Play.interactable = true;
            }
        });

    }
    public void BackLobbyPanel()
    {
        Debug.Log("BackLobbyPanel");
        HideAll();
        LobbyPanel.SetActive(true);
    }
    public void OnCreateSession()
    {
        Debug.Log("Button Create Session");
        HideAll();
        CreateSessionPanel.SetActive(true);
    }

}
