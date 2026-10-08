using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    private bool isDead = false;

    // Notifies other scripts when this object dies
    // Players and enemies will be able to have different death behaviours 
    public event Action OnDeath;

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

        if (currentHealth <= 0f)
        {
            Die();
        }
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
