using UnityEngine;

public class BladeMove : MonoBehaviour
{
    public Transform blade;
    public float rotateSpeed = 360f;
    public float minX = -0.5f;
    public float maxX = 0.5f;
    public float moveTrapSpeed = 1f;
    public GameObject pointA;
    public GameObject pointB;

    void Update()
    {
        RotateBlade();
        moveTrap();
    }

    public void moveTrap()
    {
        if (pointA == null || pointB == null) return;

        transform.position = Vector3.Lerp(pointA.transform.position, pointB.transform.position, (Mathf.Sin(Time.time * moveTrapSpeed) + 1f) / 2f);
    }

    private void RotateBlade()
    {
        if (blade == null) return;

        blade.Rotate(0f, 0f, rotateSpeed * Time.deltaTime, Space.Self);
    }
}
