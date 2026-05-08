using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Slot : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{
    public bool hover;

    private ItemData heldItem;
    private int itemCount;

    private Image itemIcon;
    private TextMeshProUGUI itemCountText;

    public void Awake()
    {
        itemIcon = transform.GetChild(0).GetComponent<Image>();
        itemCountText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hover = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hover = false;
    }

    public ItemData GetItem()
    {
        return heldItem;
    }

    public int GetItemCount()
    {
        return itemCount;
    }

    public void setItem(ItemData newItem, int count)
    {
        heldItem = newItem;
        itemCount = count;
        UpdateSlot();
    }

    public void UpdateSlot()
    {
        if (heldItem != null)
        {
            itemIcon.enabled = true;
            itemIcon.sprite = heldItem.itemIcon;
            itemCountText.text = itemCount.ToString();
        }
        else
        {
            itemIcon.enabled = false;
            itemCountText.text = "";
        }
    }

    public int addAmount (int amount)
    {
        itemCount += amount;
        UpdateSlot();
        return itemCount;
    }

    public int removeAmount (int amount)
    {
        itemCount -= amount;
        if (itemCount < 0) itemCount = 0;
        UpdateSlot();
        return itemCount;
    }

    public bool hasItem()
    {
        return heldItem != null;
    }
}
