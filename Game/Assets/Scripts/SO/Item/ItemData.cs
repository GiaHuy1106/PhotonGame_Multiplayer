using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ItemData", menuName = "Data/Item")]
public class ItemData : ScriptableObject
{
    public string itemID;
    public string itemName;
    public Sprite itemIcon;
    public string itemDescription;
    public GameObject ItemPrefab;
    public int maxStackSize;

    [Header("Pill Info")]
    public PotionType potionType;
}

public enum PotionType
{
    Red,
    Blue,
    Green,
    Yellow
}