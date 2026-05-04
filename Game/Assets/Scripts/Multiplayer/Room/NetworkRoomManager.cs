using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class NetworkRoomManager : NetworkBehaviour, IPlayerJoined, IPlayerLeft
{
    [SerializeField] Button ready_play;
    [SerializeField] Button Leave;
    [SerializeField] GameObject PlayerElementPrefab;
    public void PlayerJoined(PlayerRef player)
    {

    }

    public void PlayerLeft(PlayerRef player)
    {

    }
}
