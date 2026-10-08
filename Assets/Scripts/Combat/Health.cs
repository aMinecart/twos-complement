using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    private bool isDead = false;

    // Allows other scripts to read health without modifying it directly 
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

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

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        // Ignore invalid damage or additional hits after death
        if (isDead || damage <= 0f) return;
        currentHealth = MathF.Max(currentHealth - damage, 0f);

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
