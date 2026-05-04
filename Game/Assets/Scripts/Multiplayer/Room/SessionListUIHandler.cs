using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;

public class SessionListUIHandler : MonoBehaviour
{
    public TextMeshProUGUI statusText;
    public GameObject sessionItemListPrefab;
    public RectTransform contentLayoutGroup;
    List<SessionInfo> lastUpdateList = new();
    private void Start()
    {
        NetworkRunnerHandler.Ins.OnListSessionUpdate += OnSessionListUpdate;
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

            }
        }

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
