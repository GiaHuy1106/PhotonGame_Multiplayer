using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("UI References")]
    public Image healthBar;
    [SerializeField]
    HPHandler hPHandler;


    private void Awake()
    {
        if (hPHandler != null)
        {
            hPHandler.OnHealthChanged += OnHealthChanged;
        }
    }

    private void OnHealthChanged(float hP, float maxHP)
    {
        UpdateHealth(hP, maxHP);
    }

    

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = currentHealth/maxHealth;
        }
    }
}