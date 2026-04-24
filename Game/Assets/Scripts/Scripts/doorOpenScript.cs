using UnityEngine;

public class doorOpenScript : MonoBehaviour
{
    public enum DoorType
    {
        Move,
        Slide,
        Animation, // 👉 thêm mới
        Rotate,
    }

    [Header("Lever")]
    public Transform leverHandle;

    [Header("Door")]
    public Transform door;
    public DoorType doorType;

    [Header("Move Settings")]
    public Vector3 openDirection = Vector3.up;
    public float moveDistance = 3f;

    [Header("Rotate Settings")]
    public Vector3 rotationAxis = Vector3.up;
    public float rotationAngle = 90f;

    [Header("")]

    [Header("Animation Settings")]
    public Animator doorAnimator;
    public string openTriggerName = "Open";

    public float speed = 2f;

    private bool isOpening = false;
    private bool hasInteracted = false;
    private bool isPlayerNear = false;

    private Vector3 closedPos;
    private Vector3 openPos;

    private Quaternion closedRot;
    private Quaternion openRot;

    void Start()
    {
        if (doorType == DoorType.Animation && doorAnimator != null)
        {
            doorAnimator.enabled = false;
        }

        if (door != null)
        {
            closedPos = door.position;
            closedRot = door.rotation;

            if (doorType == DoorType.Move || doorType == DoorType.Slide)
            {
                Vector3 dir = door.TransformDirection(openDirection.normalized);
                openPos = closedPos + dir * moveDistance;
            }
            
        }
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }

        if (!isOpening || door == null) return;

        // 👉 MOVE / SLIDE
        if (doorType == DoorType.Move || doorType == DoorType.Slide)
        {
            door.position = Vector3.MoveTowards(
                door.position,
                openPos,
                speed * Time.deltaTime
            );

            if (Vector3.Distance(door.position, openPos) < 0.01f)
            {
                door.position = openPos;
                isOpening = false;
            }
        }

        //Rotate
        else if (doorType == DoorType.Rotate)
        {
            Quaternion targetRot = closedRot * Quaternion.Euler(rotationAxis * rotationAngle);
            door.rotation = Quaternion.RotateTowards(
                door.rotation,
                targetRot,
                speed * Time.deltaTime
            );

            if (Quaternion.Angle(door.rotation, targetRot) < 0.5f)
            {
                door.rotation = targetRot;
                isOpening = false;
            }
        }


        // 👉 ANIMATION → không cần xử lý trong Update
        else if (doorType == DoorType.Animation)
        {
            isOpening = false; // Animator tự xử lý
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerNear = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerNear = false;
    }

    public void Interact()
    {
        if (hasInteracted) return;

        // 👉 Lever
        if (leverHandle != null)
        {
            leverHandle.Rotate(90f, 0f, 0f);
        }

        // 👉 Door logic
        if (doorType == DoorType.Animation)
        {
            if (doorAnimator != null)
            {
                doorAnimator.enabled = true;
                doorAnimator.SetTrigger(openTriggerName);
                Debug.Log("Triggering animation: " + openTriggerName);
            }
        }
        else
        {
            isOpening = true;
        }

        hasInteracted = true;
    }
}
