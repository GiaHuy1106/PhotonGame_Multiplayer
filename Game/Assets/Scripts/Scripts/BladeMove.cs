using UnityEngine;

public class BladeMove : MonoBehaviour
{
    public Transform blade;
    public float rotateSpeed = 360f;
    public float minX = -0.5f;
    public float maxX = 0.5f;
    public float moveTrapSpeed = 1f;

    private int moveDirection = 1;

    void Update()
    {
        RotateBlade();
        moveTrap();
    }

    public void moveTrap()
    {
        float nextX = transform.position.x + moveDirection * moveTrapSpeed * Time.deltaTime;

        if (nextX >= maxX)
        {
            nextX = maxX;
            moveDirection = -1;
        }
        else if (nextX <= minX)
        {
            nextX = minX;
            moveDirection = 1;
        }

        transform.position = new Vector3(nextX, transform.position.y, transform.position.z);
    }

    private void RotateBlade()
    {
        if (blade == null) return;

        blade.Rotate(0f, 0f, rotateSpeed * Time.deltaTime, Space.Self);
    }
}
