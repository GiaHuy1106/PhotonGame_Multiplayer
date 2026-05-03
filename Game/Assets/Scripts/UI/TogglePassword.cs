using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TogglePassword : MonoBehaviour
{
    [SerializeField] TMP_InputField password;
    [SerializeField] Toggle toggle;
    private void Awake()
    {
        toggle.onValueChanged.AddListener((value) => {
            if (value)
            {
                HidePass();
            }
            else
            {
                ShowPass();
            }
        });
        
    }
    public void ShowPass()
    {
        if(password != null)
        {
            password.contentType = TMP_InputField.ContentType.Standard;
            password.ForceLabelUpdate();   
        }
    }
    public void HidePass() 
    {
        if(password != null)
        {
            password.contentType = TMP_InputField.ContentType.Password;
            password.ForceLabelUpdate();
        }
    }
}
