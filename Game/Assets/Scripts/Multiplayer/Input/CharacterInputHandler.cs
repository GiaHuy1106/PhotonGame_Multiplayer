using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInputHandler : MonoBehaviour
{
    NetworkInputData inputData = new NetworkInputData();
    [SerializeField] Transform localCamera;
    [SerializeField] NetworkPlayer player;
    Vector3 direction = Vector3.zero;
    bool isJump = false;
    bool isAttack = false;
    bool isLeftShift = false;
    bool isDefend = false;
    float rotationCamera;
    Vector2 pitchYaw = Vector2.zero;
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;

    }

    private void Update()
    {
        if (Keyboard.current.leftCtrlKey.isPressed)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        if (Keyboard.current.leftCtrlKey.wasReleasedThisFrame)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        if (!player.Object.HasInputAuthority)
        {
            return;
        }
        direction.x = Input.GetAxisRaw("Horizontal");
        direction.y = Input.GetAxisRaw("Vertical");
        isJump = Input.GetKeyDown(KeyCode.Space);
        isAttack = Input.GetMouseButtonDown(0);
        isLeftShift = Input.GetKey(KeyCode.LeftShift);
        isDefend = Input.GetKey(KeyCode.F);
        rotationCamera = localCamera.eulerAngles.y;
        pitchYaw = Input.mousePosition;
    }
    public NetworkInputData GetInputData() {
        inputData.direction = direction.normalized;
        inputData.rotationCamera = rotationCamera;
        inputData.pitchYaw = pitchYaw;
        inputData.isAttack = isAttack;
        inputData.isDefend = isDefend;
        inputData.isJump = isJump;
        inputData.isLeftShift = isLeftShift;
        isAttack = false;
        isLeftShift = false;
        isDefend = false;
        isJump = false;
        return inputData;
    }
}
