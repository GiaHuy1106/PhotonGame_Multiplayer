using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateSession : MonoBehaviour
{
    [SerializeField] Button createSession;
    [SerializeField] TMP_InputField roomName;
    [SerializeField] Toggle hasPassword;
    [SerializeField] TMP_InputField password;
    private void Awake()
    {
        createSession.onClick.AddListener(Create);
    }

    public void Create()
    {
        if (string.IsNullOrEmpty(roomName.text))
        {
            roomName.ActivateInputField();
            return;
        }
        createSession.interactable = false;
        Dictionary<string, SessionProperty> pros = new Dictionary<string, SessionProperty>();
        if (hasPassword.isOn && !string.IsNullOrEmpty(password.text))
        {
            pros.Add("HasPassword", (SessionProperty)true);
            pros.Add("Password", (SessionProperty)password.text);
        }
        else
        {

        }
            NetworkRunnerHandler.Ins.CreateSession("Empty", pros, CallbackProcess);
    }

    void CallbackProcess(bool oke)
    {
        if (oke)
        {

        }
        else
        {
            createSession.interactable = true;
        }
    }
}
