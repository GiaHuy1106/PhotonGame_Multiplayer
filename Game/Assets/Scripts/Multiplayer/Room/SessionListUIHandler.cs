using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SessionListUIHandler : MonoBehaviour
{
    public TextMeshProUGUI statusText;
    public GameObject sessionItemListPrefab;
    public RectTransform contentLayoutGroup;
    List<SessionInfo> lastUpdateList = new();
    [SerializeField] GameObject passwordInputPanel;
    [SerializeField] TMP_InputField inputPasswordRoom;
    Image image;
    private void Awake()
    {

    }
  
    private void Start()
    {
        NetworkRunnerHandler.Ins.OnListSessionUpdate += OnSessionListUpdate;
        NetworkRunnerHandler.Ins.OnJoinSessionFailed += OnJoinSessionFailed;
        passwordInputPanel.SetActive(false);
    }

    private void OnJoinSessionFailed(ShutdownReason obj)
    {
    }

    void OnSessionListUpdate(List<SessionInfo> sessionListInfo) 
    {
        lastUpdateList = sessionListInfo;
        ClearList();
        foreach (var item in sessionListInfo)
        {
            AddToList(item);
        }
    }
    public void OnClickBackGroundPassword()
    {
        Debug.Log("Testclick");
        passwordInputPanel.SetActive(false);
        temp = null;
    }

    public void ClearList()
    {
        foreach (Transform child in contentLayoutGroup.transform)
        {
            Destroy(child.gameObject);
        }
    }
    public void AddToList(SessionInfo sessionInfo)
    {
        SessionListInfoItem addedSessionInfoListItem = Instantiate(sessionItemListPrefab, contentLayoutGroup.transform).GetComponent<SessionListInfoItem>();
        addedSessionInfoListItem.SetInformation(sessionInfo);
        addedSessionInfoListItem.OnJoinSession += OnJoinedSessionClick;
    }
    SessionInfo temp;

    void OnJoinedSessionClick(SessionInfo sessionInfo)
    {
        if (!sessionInfo.IsValid)
        {
            statusText.text = "Invalid Room";
            return;
        }                                                               
        if (!sessionInfo.IsOpen)
        {
            statusText.text = "Room is closed";
            return;
        }
        if(sessionInfo.PlayerCount >= sessionInfo.MaxPlayers)
        {
            statusText.text = $"Room is full";
            return;
        }
        if (sessionInfo.Properties.TryGetValue("HasPassword", out var value))
        {
            if(value.Isbool && (bool)value)
            {
                passwordInputPanel.SetActive(true);
                inputPasswordRoom.text = null;
                inputPasswordRoom.ActivateInputField();
                temp = sessionInfo;
            }
        }
        else
        {
            NetworkRunnerHandler.Ins.JoinSession(sessionInfo.Name);
        }

    }

     void OnEndEdit(string password)
    {
        passwordInputPanel.SetActive(false);
        NetworkRunnerHandler.Ins.JoinSession(temp.Name, password, (value, shutdownReason) => {
            if (value)
            {
                Debug.Log($"Join {temp.Name} success");
            }
            else
            {
                statusText.text = $"{shutdownReason}";
                Debug.Log("JoinLobby has password Success");
            }
        });
    }
   


    public void OnNoSessionFound()
    {
        statusText.text = "No Game session found";
        statusText.color = Color.red;
        statusText.gameObject.SetActive(true);
    }
    public void OnLookingForGameSession()
    {
        statusText.text = "Looking up...";
        statusText.color = Color.white;
        statusText.gameObject.SetActive(true);
    }
    private void OnDestroy()
    {
        NetworkRunnerHandler.Ins.OnListSessionUpdate -= OnSessionListUpdate;
        NetworkRunnerHandler.Ins.OnJoinSessionFailed -= OnJoinSessionFailed;
    }
    public void Refesh()
    {
        Debug.Log("Refesh Button");
        if (lastUpdateList != null && lastUpdateList.Count != 0)
        {
            OnSessionListUpdate(lastUpdateList);
        }
    }
}
