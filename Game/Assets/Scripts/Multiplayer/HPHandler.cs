using System;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class HPHandler : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] private float maxHP = 100f;
    [SerializeField] Image healthBar;
    [SerializeField] NetworkPlayer player;
    [Networked, OnChangedRender(nameof(OnHPChanged))]
    public float HP { get; private set; }

    [Networked]
    public NetworkBool IsDead { get; private set; }
    Transform hpUI;
    public event Action<bool> OnHealthChanged;
    public event Action OnDead;
    public event Action OnHealth;
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

    public void Heal(float amount)
    {
        if (!Object.HasStateAuthority)
            return;

        if (IsDead)
            return;

        HP = Mathf.Clamp(HP + amount, 0, maxHP);
        RPC_Recover();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_Recover()
    {
        OnHealth?.Invoke();
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
        player.OnGetHit();
    }

    private void Die()
    {
        if (IsDead)
            return;

        IsDead = true;

        OnDead?.Invoke();
        hpUI.gameObject.SetActive(false);
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
        OnHealthChanged?.Invoke(true);
        healthBar.fillAmount = HP / maxHP;
    }    

  
}