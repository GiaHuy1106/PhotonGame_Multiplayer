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
    [SerializeField] GameObject passwordInputPanel;
    [SerializeField] TMP_InputField inputPasswordRoom;
    [SerializeField] TMP_InputField nameRoomLookup;
    [SerializeField] GameObject Processing;
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
        Debug.Log("Join session failed" + obj);
    }

    void OnSessionListUpdate(List<SessionInfo> sessionListInfo) 
    {
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
        Processing.SetActive(true);
        if (!sessionInfo.IsValid)
        {
            DisPlayText(5f, "Invalid Room", Color.red);
            return;
        }                                                               
        if (!sessionInfo.IsOpen)
        {
            DisPlayText(5f, "Room is closed", Color.red);
            return;
        }
        if(sessionInfo.PlayerCount >= sessionInfo.MaxPlayers)
        {
            DisPlayText(5f, "Room is full", Color.red);
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
            NetworkRunnerHandler.Ins.JoinSession(sessionInfo.Name, callbackProcess: (value, shudown) => { if (!value) Processing.SetActive(false); });
        }

    }

     public void OnEndEdit(string password)
    {
        Debug.Log("OnEndEdit");
        passwordInputPanel.SetActive(false);
        NetworkRunnerHandler.Ins.JoinSession(temp.Name, password, (value, shutdownReason) => {
            if (value)
            {
                Debug.Log($"Join {temp.Name} success");
            }
            else
            {
                Processing.SetActive(false);
                if (shutdownReason == ShutdownReason.ConnectionRefused)
                    DisPlayText(5f, "!Wrong Password", Color.red);
                else
                    DisPlayText(5f, "Error connection", Color.red);
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
        if (!string.IsNullOrEmpty(nameRoomLookup.text)){
            SessionInfo result = NetworkRunnerHandler.Ins.LookingSession(nameRoomLookup.text);
            if(result != null)
            {
                ClearList();
                AddToList(result);
            }
        }
        else
            nameRoomLookup.ActivateInputField();
    }
    private void OnDestroy()
    {
        NetworkRunnerHandler.Ins.OnListSessionUpdate -= OnSessionListUpdate;
        NetworkRunnerHandler.Ins.OnJoinSessionFailed -= OnJoinSessionFailed;
    }
    public void Refesh()
    {
        Debug.Log("Refesh");
        NetworkRunnerHandler.Ins.RequestRefeshLobby((sessionList) => 
        {
            if(sessionList != null)
            {
                ClearList();
                foreach (var item in sessionList)
                {
                    AddToList(item);
                }
            }
        });
    }
    public void DisPlayText(float duration, string text, Color color)
    {
        if(statusText != null){
        statusText.gameObject.SetActive(true);
            statusText.text = text;
            statusText.color = color;
            Utils.DelayCall(duration, () => statusText.gameObject.SetActive(false));
        }

    }
}
