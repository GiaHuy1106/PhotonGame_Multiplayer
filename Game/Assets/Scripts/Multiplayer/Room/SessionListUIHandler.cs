using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;

public class SessionListUIHandler : MonoBehaviour
{
    public TextMeshProUGUI statusText;
    public GameObject sessionItemListPrefab;
    public RectTransform contentLayoutGroup;
    
    private void Start()
    {
        NetworkRunnerHandler.Ins.OnListSessionUpdate += OnSessionListUpdate;
    }

    void OnSessionListUpdate(List<SessionInfo> sessionListInfo) 
    {
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
        addedSessionInfoListItem.OnJoinSession += OnJoinedSession;
    }

    void OnJoinedSession(SessionInfo sessionInfo)
    {
        if (sessionInfo.IsOpen)
        {

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
}
