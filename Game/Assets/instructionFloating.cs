using UnityEngine;
using UnityEngine.UI;

public class FloatingInteractUI : MonoBehaviour
{
    public InstructionData data;

    public GameObject instructionUI;
    public Image instructionIcon;

    private bool isPlayerInRange = false;

    void Start()
    {
        instructionUI.SetActive(false);

        // set icon từ data
        if (data != null)
        {
            instructionIcon.sprite = data.icon;
        }
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(data.key))
        {
            Interact();
        }
    }

    void Interact()
    {
        Debug.Log("Interact with lever");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            instructionUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            instructionUI.SetActive(false);
        }
    }
}