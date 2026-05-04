using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider mainBar;
    public Slider ghostBar;
    public TextMeshProUGUI nameText;
    private CanvasGroup canvasGroup;
    [Header("Settings")]
    public float lerpSpeed = 2f; 
    private float targetHealth;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void ShowUI()
    {
        canvasGroup.alpha = 1f;
    }

    public void HideUI()
    {
        canvasGroup.alpha = 0f;
    }
    public void SetupBoss(string name, float maxHealth)
    {
        nameText.text = name;
        mainBar.maxValue = maxHealth;
        ghostBar.maxValue = maxHealth;

        mainBar.value = maxHealth;
        ghostBar.value = maxHealth;
        targetHealth = maxHealth;
    }

    public void UpdateHealth(float currentHealth)
    {
        mainBar.value = currentHealth;
        targetHealth = currentHealth;
    }

    void Update()
    {
        if (ghostBar.value > targetHealth)
        {
            ghostBar.value = Mathf.Lerp(ghostBar.value, targetHealth, Time.deltaTime * lerpSpeed);
        }
    }
}