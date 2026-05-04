using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Axe") || other.gameObject.CompareTag("Blade"))
        {
            Debug.Log("Player hit a weapon!");
        }
    }
}
