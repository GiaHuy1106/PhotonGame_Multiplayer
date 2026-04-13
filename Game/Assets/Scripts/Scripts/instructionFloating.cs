using UnityEngine;
using UnityEngine.UI;

public class instructionFloating : MonoBehaviour
{
    [Header("UI")]
    public GameObject instructionUI;   // panel chứa icon E
    public Image instructionIcon;      // hình nút E

    [Header("Target")]
    public GameObject lever;

    [Header("Floating")]
    [SerializeField] private float floatHeight = 0.3f;
    [SerializeField] private float floatSpeed = 2f;

    private Vector3 startPos;
    private bool isPlayerInRange = false;

    void Start()
    {
        instructionUI.SetActive(false);
        startPos = instructionUI.transform.localPosition;
    }

    void Update()
    {
        // Floating effect
        if (instructionUI.activeSelf)
        {
            float newY = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
            instructionUI.transform.localPosition = startPos + new Vector3(0, newY, 0);
        }

        // Input
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    void Interact()
    {
        Debug.Log("Interact with lever");

        // Ví dụ gọi lever
        // lever.GetComponent<LeverScript>().Activate();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            ShowUI();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            HideUI();
        }
    }

    void ShowUI()
    {
        instructionUI.SetActive(true);
    }

    void HideUI()
    {
        instructionUI.SetActive(false);
    }
}
