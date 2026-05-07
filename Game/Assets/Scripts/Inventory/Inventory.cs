using UnityEngine;
using System.Collections.Generic;
public class Inventory : MonoBehaviour
{
    [Header("Test Item")]
    [SerializeField] private ItemData redPotion;
    [SerializeField] private ItemData bluePotion;
    [SerializeField] private ItemData greenPotion;

    [Header("Inventory Slots")]
    public GameObject inventorySlotParent;
    private List<Slot> inventorySlots = new List<Slot>();
    private List<Slot> allSlots = new List<Slot>();

    void Awake()
    {
        inventorySlots.AddRange(inventorySlotParent.GetComponentsInChildren<Slot>());
        allSlots.AddRange(inventorySlots);
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddItem(ItemData itemtoAdd, int amount)
    {
        int remaining = amount;

        foreach (Slot slot in inventorySlots)
        {
            if (slot.hasItem() && slot.GetItem() == itemtoAdd)
            {
                int currentAmount = slot.GetItemCount();
                int maxStack = itemtoAdd.maxStackSize;
                
                if (currentAmount < maxStack)
                {
                    int spaceLeft = maxStack - currentAmount;
                    int amounttoAdd = Mathf.Min(spaceLeft, remaining);

                    slot.setItem(itemtoAdd, currentAmount + amounttoAdd);
                    remaining -= amounttoAdd;

                    if (remaining <= 0)
                        return;
                }
            }
        }

        foreach (Slot slot in allSlots)
        {
            if (!slot.hasItem())
            {
                int amounttoPlace = Mathf.Min(itemtoAdd.maxStackSize, remaining);
                slot.setItem(itemtoAdd, amounttoPlace);
                remaining -= amounttoPlace;

                if (remaining <= 0)
                    return;
            }
        }

        if(remaining > 0)
        {
            Debug.Log("Not enough space in inventory for " + remaining + " of " + itemtoAdd.itemName);
        }
    }
}
