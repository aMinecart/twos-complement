using UnityEngine;

[RequireComponent(typeof(Health))]
public class HealthTester : MonoBehaviour
{
    private Health health;
    
    void Awake()
    {
        // Get health component 
        health = GetComponent<Health>();
    }

    void OnEnable()
    {
        // Subscribe to health changes
        health.OnHealthChanged += HandleHealthChanged;
    }

    void OnDisable()
    {
        health.OnHealthChanged -= HandleHealthChanged;
    }

    private void HandleHealthChanged(float currentHealth, float maxHealth)
    {
        Debug.Log("Health event: " + currentHealth + "/" + maxHealth);
    }
}
