using System;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class HPHandler : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] private float maxHP = 100f;
    [SerializeField] Image healthBar;
    [Networked, OnChangedRender(nameof(OnHPChanged))]
    public float HP { get; private set; }

    [Networked]
    public NetworkBool IsDead { get; private set; }
    Transform hpUI;
    public event Action<float, float> OnHealthChanged;
    public event Action OnDead;
    private void Awake()
    {
        hpUI = healthBar.transform.parent;
    }
    private void LateUpdate()
    {      
        if(Camera.main != null) 
            hpUI.forward = Camera.main.transform.forward;        
    }

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            HP = maxHP;
            IsDead = false;
        }
        OnHPChanged();
    }

    public bool CanTakeDamage()
    {
        return !IsDead;
    }

    public void Heal(int amount)
    {
        if (!Object.HasStateAuthority)
            return;

        if (IsDead)
            return;

        HP = Mathf.Clamp(HP + amount, 0, maxHP);
    }

    public void TakeDamage(float damage)
    {       

        if (IsDead)
            return;

        HP -= damage;

        if (HP <= 0)
        {
            HP = 0;
            Die();
        }
    }

    private void Die()
    {
        if (IsDead)
            return;

        IsDead = true;

        OnDead?.Invoke();

        Debug.Log($"{Object.InputAuthority} died");
    }

    public void Revive()
    {
        if (!Object.HasStateAuthority)
            return;

        HP = maxHP;
        IsDead = false;
    }

    private void OnHPChanged()
    {
        OnHealthChanged?.Invoke(HP, maxHP);
        healthBar.fillAmount = HP / maxHP;
    }    

  
}