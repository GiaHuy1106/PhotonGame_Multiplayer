using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateSession : MonoBehaviour
{
    [SerializeField] Button createSession;
    [SerializeField] TMP_InputField roomName;
    [SerializeField] Toggle hasPassword;

    public void Create()
    {
        NetworkRunnerHandler.Ins.CreateSession(roomName.text, new());
    }
}
