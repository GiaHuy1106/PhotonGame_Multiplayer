using System;
using Fusion;
using UnityEngine;

public class HPHandler : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] private float maxHP = 100;
    [SerializeField] PlayerHealthUI healthUI;
    [Networked, OnChangedRender(nameof(OnHPChanged))]
    public float HP { get; private set; }

    [Networked]
    public NetworkBool IsDead { get; private set; }

    public event Action<float, float> OnHealthChanged;
    public event Action OnDead;

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            HP = maxHP;
            IsDead = false;
        }
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
        if (!Object.HasStateAuthority)
            return;

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

        Debug.Log($"HP Changed: {HP}/{maxHP}");
    }    

    public float GetHPRatio()
    {
        return HP / maxHP;
    }
}