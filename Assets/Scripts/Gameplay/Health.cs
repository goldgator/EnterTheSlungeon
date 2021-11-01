using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public float maxHealth;
    private float currentHealth = 1;
    public float invinicibilityTime = 0.0f;
    private float invincTimer = 0;
    //public bool hasRegen = false;
    //public float regenTime = 2.0f;
    //private float currentRegTime;
    //public float regenAmount = 1.0f;

    //public Health parentHealth;
    //public bool parentAddHealth;

    private bool hasDied = false;
    public bool HasDied { get { return hasDied; } }
    public float HealthRatio { get => currentHealth / maxHealth; }

    //public UnityEvent deathEvent;

    public float subEventThreshold = 1f;
    public UnityEvent subtractEvent;


    void Start()
    {
        currentHealth = maxHealth;
    }

    public void SetInvincibleTimer(float time)
    {
        invincTimer = Mathf.Max(time, invincTimer);
    }

    public void AddHealth(float add)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + add);
    }

    public void SetHealth(float value)
    {
        currentHealth = Mathf.Min(maxHealth, value);
    }

    public float GetHealth()
    {
        return currentHealth;
    }
    public void SubtractHealth(float minus)
    {
        if (invincTimer > 0) return;

        currentHealth -= minus;


        if (minus >= subEventThreshold)
        {
            invincTimer = invinicibilityTime;
            subtractEvent?.Invoke();
        }
    }

    public void FullHeal()
    {
        currentHealth = maxHealth;
        hasDied = false;
    }

    public void Kill()
    {
        currentHealth = 0;
    }

    private void Update()
    {
        invincTimer -= Time.deltaTime;

        if (currentHealth <= 0 && !hasDied)
        {
            InvokeDeathMethods();
            hasDied = true;
        }
    }

    private void InvokeDeathMethods()
    {
        IHealthDeath[] healthDeaths = GetComponents<IHealthDeath>();

        foreach (IHealthDeath death in healthDeaths) death.OnDeath();
    }

}
