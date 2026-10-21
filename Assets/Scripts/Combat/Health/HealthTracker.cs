using System;
using UnityEngine;

public class HealthTracker : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    private float damageResist = 0.0f;
    public bool isDead { get; private set; } = false;

    // Allows other scripts to read health without modifying it directly
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public float DamageResist => damageResist;

    // Returns health as a value between 0 and 1 for UI health bars
    public float HealthPercentage => maxHealth > 0f
        ? currentHealth / maxHealth
        : 0f;

    // Notifies other scripts when this object dies
    // Players and enemies will be able to have different death behaviours
    public event Action OnDeath;
    // Notifies other scripts whenever health changes
    // Sends current health and max health
    public event Action<float, float> OnHealthChanged;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        // Ignore invalid damage or additional hits after death
        if (isDead || damage <= 0f) return;
        currentHealth = MathF.Max(currentHealth - damage * (1 - damageResist), 0f);

        Debug.Log("Took " + damage + " damage. Health: " + currentHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        // Prevent healing dead characters / invalid amounts
        if (isDead || amount <= 0f) return;

        // Heal without going over max health
        currentHealth = MathF.Min(currentHealth + amount, maxHealth);

        Debug.Log(gameObject.name + " healed. Health: " + currentHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void AddMaxHealth(int amount, bool healForAmount = true)
    {
        // Prevent healing dead characters / invalid amounts
        if (isDead || amount <= 0f)
            return;

        maxHealth += amount;
        Debug.Log(gameObject.name + " max health increased. Max health: " + maxHealth);

        if (healForAmount)
        {
            currentHealth += amount; // can always heal for the amount of max health gained
            Debug.Log(gameObject.name + " healed. Health: " + currentHealth);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }
    }

    public void AddResistance(float amount)
    {
        // Prevent giving resistance for dead characters / invalid amounts
        if (isDead || amount < -60f || amount > 60f)
            return;

        damageResist += amount;
        Debug.Log(gameObject.name + " damage resist changed. Damage resist: " + damageResist * 100 + "%");
    }

    // Set health values when initializing or restoring an entity
    public void InitializeHealth(float health, float maximumHealth, float damageResistance)
    {
        maxHealth = Mathf.Max(1f, maximumHealth);
        currentHealth = Mathf.Clamp(health, 0f, maxHealth);
        damageResist = Mathf.Clamp(damageResistance, -60f, 60f);

        isDead = currentHealth <= 0f;

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        // Death is processed only once
        if (isDead) return;

        isDead = true;
        Debug.Log(gameObject.name + " died.");

        // Notifies subscribed scripts
        OnDeath?.Invoke();
    }
}