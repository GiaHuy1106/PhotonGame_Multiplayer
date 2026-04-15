using UnityEngine;

public class triggerManager : MonoBehaviour
{
    public static triggerManager instance;
    public GameObject triggerZone; // khu vực trigger
    public bool isPlayerInRange = false;

    void Awake()
    {
        instance = this;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            instructionFloating.Instance.UpdateUIPosition();
            instructionFloating.Instance.instructionUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            instructionFloating.Instance.instructionUI.SetActive(false);
        }
    }

}
