 using UnityEngine;
public class instructionFloating : MonoBehaviour
{
    public static instructionFloating Instance { get; private set; }

    [Header("Lever")]
    public Transform target; // object (lever)
    public Transform leverHandle; // handle của lever để xoay

    [Header("Door")]
    public Transform door; // cửa để mở
    public float doorOpenHeight = 3f; // độ cao cửa sẽ chạy lên
    public float doorOpenSpeed = 2f; // tốc độ mở cửa

    [Header("UI")]
    public GameObject instructionUI;
    public Vector3 offset = new Vector3(0, 2f, 0); // cao hơn đầu object

    private Camera cam;
    private bool isDoorOpening = false;
    private bool hasInteracted = false;
    private Vector3 doorClosedPosition;
    private Vector3 doorOpenPosition;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        cam = Camera.main;

        if (instructionUI != null)
        {
            instructionUI.SetActive(false);
        }

        if (door != null)
        {
            doorClosedPosition = door.position;
            doorOpenPosition = doorClosedPosition + new Vector3(0f, doorOpenHeight, 0f);
        }
    }

    void Update()
    {
        if (triggerManager.instance != null && triggerManager.instance.isPlayerInRange)
        {
            UpdateUIPosition();

            if (Input.GetKeyDown(KeyCode.E))
            {
                Interact();
            }
        }

        if (isDoorOpening && door != null)
        {
            door.position = Vector3.MoveTowards(
                door.position,
                doorOpenPosition,
                doorOpenSpeed * Time.deltaTime
            );

            if (door.position == doorOpenPosition)
            {
                isDoorOpening = false;
            }
        }
    }

    public void UpdateUIPosition()
    {
        if (target == null || instructionUI == null || cam == null)
        {
            return;
        }

        Vector3 worldPos = target.position + offset;
        Vector3 screenPos = cam.WorldToScreenPoint(worldPos);

        instructionUI.transform.position = screenPos;
    }

    public void Interact() //animation lever
    {
        if (hasInteracted)
        {
            return;
        }

        if (leverHandle != null)
        {
            leverHandle.Rotate(90f, 0, 0);
        }

        if (door != null)
        {
            doorClosedPosition = door.position;
            doorOpenPosition = doorClosedPosition + new Vector3(0f, doorOpenHeight, 0f);
            isDoorOpening = true;
        }

        hasInteracted = true;

        if (instructionUI != null)
        {
            instructionUI.SetActive(false);
        }
    }
}
