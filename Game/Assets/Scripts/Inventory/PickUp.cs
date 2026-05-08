using UnityEngine;

public class PickUp : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Inventory inventory;

    [Header("Potion Item Data")]
    [SerializeField] private ItemData redPotion;
    [SerializeField] private ItemData bluePotion;
    [SerializeField] private ItemData greenPotion;

    [SerializeField] private int amount = 1;

    private void Awake()
    {
        if (inventory == null)
            inventory = GetComponent<Inventory>();
    }

    private void OnTriggerEnter(Collider other)
    {
        ItemData itemData = GetItemData(other);
        if (itemData == null)
            return;

        if (inventory == null)
        {
            Debug.LogWarning("PickUp: Inventory is missing on " + gameObject.name + ".");
            return;
        }

        inventory.AddItem(itemData, amount);
        Debug.Log("PickUp: Added " + amount + " " + itemData.itemName + " to inventory.");
        Destroy(other.gameObject);
    }

    private ItemData GetItemData(Collider other)
    {
        if (other.CompareTag("redPotion"))
            return redPotion;

        if (other.CompareTag("bluePotion"))
            return bluePotion;

        if (other.CompareTag("greenPotion"))
            return greenPotion;

        return null;
    }
}
