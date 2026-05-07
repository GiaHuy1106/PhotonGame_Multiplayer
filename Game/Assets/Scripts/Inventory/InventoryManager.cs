using System.Collections;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("Inventory UI")]
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private RectTransform inventoryRect;

    [Header("Animation")]
    [SerializeField] private KeyCode toggleKey = KeyCode.I;
    [SerializeField] private Vector2 hiddenOffset = new Vector2(900f, 0f);
    [SerializeField] private float animationDuration = 0.25f;

    private Vector2 shownPosition;
    private Vector2 hiddenPosition;
    private Coroutine animationCoroutine;
    private bool isOpen;

    private void Awake()
    {
        if (inventoryUI == null)
            inventoryUI = gameObject;

        if (inventoryRect == null)
            inventoryRect = inventoryUI.GetComponent<RectTransform>();

        shownPosition = inventoryRect.anchoredPosition;
        hiddenPosition = shownPosition + hiddenOffset;
        inventoryRect.anchoredPosition = hiddenPosition;
        inventoryUI.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
            ToggleInventory();
    }

    public void ToggleInventory()
    {
        SetInventoryOpen(!isOpen);
    }

    public void SetInventoryOpen(bool open)
    {
        isOpen = open;

        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        inventoryUI.SetActive(true);
        Vector2 targetPosition = isOpen ? shownPosition : hiddenPosition;
        animationCoroutine = StartCoroutine(InventoryAnimation(targetPosition, !isOpen));

        Debug.Log("InventoryManager: " + (isOpen ? "Opening" : "Closing") + " inventory.");
    }

    private IEnumerator InventoryAnimation(Vector2 targetPosition, bool disableWhenDone)
    {
        Vector2 startPosition = inventoryRect.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / animationDuration);
            t = Mathf.SmoothStep(0f, 1f, t);

            inventoryRect.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        inventoryRect.anchoredPosition = targetPosition;

        if (disableWhenDone)
            inventoryUI.SetActive(false);

        animationCoroutine = null;
        Debug.Log("InventoryManager: Inventory animation finished.");
    }
}
