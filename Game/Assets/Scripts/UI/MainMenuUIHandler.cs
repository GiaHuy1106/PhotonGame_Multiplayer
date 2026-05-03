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
    [SerializeField] Button Back;
    [Header("CreateSession")]
    [SerializeField] Button backLobbyPanel;
    private void Awake()
    {
        Play.onClick.AddListener(OnPlayClick);
        backLobbyPanel.onClick.AddListener(BackLobbyPanel);

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

    public void OnPlayClick()
    {
        if (string.IsNullOrEmpty(playerNameInput.text))
        {
            playerNameInput.ActivateInputField();
            return;
        }
        Play.interactable = false;
        NetworkRunnerHandler.Ins.JoinLobby( () => 
        {
            HideAll();
            LobbyPanel.SetActive (true);
        }, 
        () => 
        {
            Play.interactable = true;
        });

    }
    public void BackLobbyPanel()
    {
        HideAll();
        LobbyPanel.SetActive(true);
    }
    public void CreateSessionButton()
    {
        HideAll();
        CreateSessionPanel.SetActive(true);
    }

}
