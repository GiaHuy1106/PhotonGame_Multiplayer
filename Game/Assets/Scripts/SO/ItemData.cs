using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Data/Item")]
public class ItemData : ScriptableObject
{
    public string itemID;
    public string itemName;
    public Sprite icon;

    [Header("Gem Info")]
    public GemColor gemColor;
}

public enum GemColor
{
    Red,
    Blue,
    Green,
    Yellow
}