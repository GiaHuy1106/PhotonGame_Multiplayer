using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 15f;
    public float damage = 20f;
    public float lifetime = 5f;

    void Start()
    {
        // Tự động hủy sau 5 giây để không rác bộ nhớ nếu đạn bay ra ngoài map
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Bay thẳng về phía trước
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
            // Trúng mục tiêu thì nổ tung (Xóa đạn)
            Destroy(gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            // Chạm đất hoặc tường cũng nổ
            Destroy(gameObject);
        }
    }
}