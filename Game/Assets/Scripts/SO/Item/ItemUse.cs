using UnityEngine;
using System.Collections;

public class ItemUse : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
    public Inventory inventory;

    [Header("Test Items")]
    [SerializeField] private ItemData redPotionTest;
    [SerializeField] private ItemData bluePotionTest;
    [SerializeField] private ItemData greenPotionTest;

    [Header("Potion Effects")]
    [SerializeField] private float redHealAmount = 30f;
    [SerializeField] private float blueSpeedMultiplier = 1.5f;
    [SerializeField] private float blueDuration = 8f;
    [SerializeField] private float greenDamageMultiplier = 1.5f;
    [SerializeField] private float greenDuration = 8f;

    private Coroutine speedCoroutine;
    private Coroutine damageCoroutine;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            checkInventoryForItem(redPotionTest);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            checkInventoryForItem(bluePotionTest);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            checkInventoryForItem(greenPotionTest);
        }
    }

    public void checkInventoryForItem(ItemData item)
    {
        if (item == null)
        {
            Debug.LogWarning("ItemUse: checkInventoryForItem called with null item.");
            return;
        }

        Debug.Log("ItemUse: Checking inventory for " + item.itemName + ".");

        if (inventory == null)
        {
            Debug.LogWarning("ItemUse: Inventory not found in scene.");
            return;
        }

        foreach (Slot slot in inventory.GetComponentsInChildren<Slot>())
        {
            if (slot.GetItem() == item && slot.GetItemCount() > 0)
            {
                Debug.Log("ItemUse: Found " + slot.GetItemCount() + " of " + item.itemName + " in inventory. Using one.");
                slot.setItem(item, slot.GetItemCount() - 1);
                UseItem(item);
                return;
            }
        }

        Debug.Log("ItemUse: " + item.itemName + " not found in inventory.");
    }

    public void UseAssignedItem()
    {
        UseItem(itemData);
    }

    public void UseItem(ItemData item)
    {
        UseItem(item, gameObject);
    }

    public void UseItem(ItemData item, GameObject targetPlayer)
    {
        if (item == null || targetPlayer == null)
        {
            Debug.LogWarning("ItemUse: Missing item data or target player.");
            return;
        }

        Debug.Log("ItemUse: Using " + item.itemName + " (" + item.potionType + ") on " + targetPlayer.name + ".");

        switch (item.potionType)
        {
            case PotionType.Red:
                ApplyRedPotion(targetPlayer);
                break;

            case PotionType.Blue:
                ApplyBluePotion(targetPlayer);
                break;

            case PotionType.Green:
                ApplyGreenPotion(targetPlayer);
                break;
        }
    }

    private void ApplyRedPotion(GameObject targetPlayer)
    {
        //PlayerHealth health = targetPlayer.GetComponent<PlayerHealth>();
        //if (health == null)
        //{
        //    Debug.LogWarning("ItemUse: PlayerHealth not found on target player.");
        //    return;
        //}

        //health.Heal(redHealAmount);
        Debug.Log("ItemUse: Red Potion healed " + redHealAmount + " HP.");
    }

    private void ApplyBluePotion(GameObject targetPlayer)
    {
        PlayerMovement movement = targetPlayer.GetComponent<PlayerMovement>();
        if (movement == null)
        {
            Debug.LogWarning("ItemUse: PlayerMovement not found on target player.");
            return;
        }

        if (speedCoroutine != null)
        {
            StopCoroutine(speedCoroutine);
            ResetSpeed(movement);
            Debug.Log("ItemUse: Restarting Blue Potion speed boost.");
        }

        speedCoroutine = StartCoroutine(SpeedBoostRoutine(movement));
    }

    private void ApplyGreenPotion(GameObject targetPlayer)
    {
        SwordDamage swordDamage = targetPlayer.GetComponentInChildren<SwordDamage>();
        if (swordDamage == null)
        {
            Debug.LogWarning("ItemUse: SwordDamage not found in target player children.");
            return;
        }

        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            ResetDamage(swordDamage);
            Debug.Log("ItemUse: Restarting Green Potion damage boost.");
        }

        damageCoroutine = StartCoroutine(DamageBoostRoutine(swordDamage));
    }

    private IEnumerator SpeedBoostRoutine(PlayerMovement movement)
    {
        movement.walkSpeed *= blueSpeedMultiplier;
        movement.sprintSpeed *= blueSpeedMultiplier;
        Debug.Log("ItemUse: Blue Potion active. Walk speed = " + movement.walkSpeed + ", sprint speed = " + movement.sprintSpeed + ".");

        yield return new WaitForSeconds(blueDuration);

        ResetSpeed(movement);
        Debug.Log("ItemUse: Blue Potion ended. Walk speed = " + movement.walkSpeed + ", sprint speed = " + movement.sprintSpeed + ".");
        speedCoroutine = null;
    }

    private IEnumerator DamageBoostRoutine(SwordDamage swordDamage)
    {
        swordDamage.damageAmount *= greenDamageMultiplier;
        Debug.Log("ItemUse: Green Potion active. Sword damage = " + swordDamage.damageAmount + ".");

        yield return new WaitForSeconds(greenDuration);

        ResetDamage(swordDamage);
        Debug.Log("ItemUse: Green Potion ended. Sword damage = " + swordDamage.damageAmount + ".");
        damageCoroutine = null;
    }

    private void ResetSpeed(PlayerMovement movement)
    {
        movement.walkSpeed /= blueSpeedMultiplier;
        movement.sprintSpeed /= blueSpeedMultiplier;
    }

    private void ResetDamage(SwordDamage swordDamage)
    {
        swordDamage.damageAmount /= greenDamageMultiplier;
    }
}
