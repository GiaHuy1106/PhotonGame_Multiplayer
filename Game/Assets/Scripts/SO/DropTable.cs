using UnityEngine;

[CreateAssetMenu(fileName = "DropTable", menuName = "Data/DropTable")]
public class DropTable : ScriptableObject
{
    public DropItem[] drops;
}

[System.Serializable]
public class DropItem
{
    public ItemData item;
    public float dropChance; // 0 → 1
}