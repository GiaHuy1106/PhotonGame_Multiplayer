using UnityEngine;

public class trap1Move : MonoBehaviour
{
    public Transform trap1;
    //public doorOpenScript doorOpen;
    public float minZ = -50f;
    public float maxZ = 50f;
    public float moveSpeed = 5f;

    private int moveDirection = 1;
    private float currentZRotation;

    void Start()
    {
        currentZRotation = trap1 != null ? NormalizeAngle(trap1.localEulerAngles.z) : 0f; //đây là điểm khởi đầu của trap1, nếu trap1 không được gán thì sẽ bắt đầu ở 0 độ
        currentZRotation = Mathf.Clamp(currentZRotation, minZ, maxZ);
    }

    void Update()
    {
        if (trap1 == null) return;
        //if (doorOpen != null && doorOpen.HasInteracted) return;

        float nextZ = currentZRotation + moveDirection * moveSpeed * Time.deltaTime;

        if (nextZ >= maxZ)
        {
            nextZ = maxZ;
            moveDirection = -1;
        }
        else if (nextZ <= minZ)
        {
            nextZ = minZ;
            moveDirection = 1;
        }

        currentZRotation = nextZ;
        Vector3 eulerAngles = trap1.localEulerAngles;
        trap1.localRotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, currentZRotation);
    }

    public void moveTrap()
    {
        if (trap1 != null)
        {
            Vector3 eulerAngles = trap1.localEulerAngles;
            trap1.localRotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, currentZRotation);
        }
    }

    private float NormalizeAngle(float angle)
    {
        return angle > 180f ? angle - 360f : angle;
    }
}
