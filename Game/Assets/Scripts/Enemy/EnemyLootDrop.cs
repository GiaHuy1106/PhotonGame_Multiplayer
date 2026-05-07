using UnityEngine;

public class EnemyLootDrop : MonoBehaviour
{
    [Header("Loot Data")]
    public MonsterData monsterData;

    [Header("Drop Polish")]
    public float dropHeightOffset = 1f; // Rớt từ độ cao bụng quái thay vì dưới gầm chân
    public float scatterRadius = 2f;  // Độ văng xa của item
    public float bounceForce = 7f;
    public void DropLoot()
    {
        if(monsterData == null || monsterData.dropTable == null || monsterData.dropTable.drops.Length == 0)
        {
            Debug.LogWarning("MonsterData or DropTable is not assigned for " + gameObject.name);
            return;
        }
        float randomValue = Random.value;
        float cumulativeChance = 0f;
        foreach (DropItem drop in monsterData.dropTable.drops)
        {
            cumulativeChance += drop.dropChance;
            if (randomValue <= cumulativeChance)
            {
                SpawnItem(drop.item);
                break;
            }
        }
    }

    private void SpawnItem(ItemData item)
    {
        if(item == null || item.ItemPrefab == null)
        {
            Debug.Log("Item {itemToDrop?.itemName} bị thiếu ItemPrefab 3D");
            return;
        }
        Vector2 randomCircle = Random.insideUnitCircle * scatterRadius;
        Vector3 spawnPosition = transform.position + new Vector3(0f, dropHeightOffset, 0f);
        GameObject droppedItem = Instantiate(item.ItemPrefab, spawnPosition, Quaternion.identity);
    }
}
