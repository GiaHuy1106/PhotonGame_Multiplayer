using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SessionListUIHandler : MonoBehaviour
{
    public TextMeshProUGUI statusText;
    public GameObject sessionItemListPrefab;
    public VerticalLayoutGroup verticalLayoutGroup;
    public void ClearList()
    {
        foreach (Transform child in verticalLayoutGroup.transform)
        {
            Destroy(child.gameObject);
        }
    }
    public void AddToList(SessionInfo sessionInfo)
    {
        SessionListInfoItem addedSessionInfoListItem = Instantiate(sessionItemListPrefab, verticalLayoutGroup.transform).GetComponent<SessionListInfoItem>();
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

    }
}
