using UnityEngine;

[RequireComponent(typeof(HealthTracker))]
public class HealthTester : MonoBehaviour
{
    private HealthTracker health;
    
    void Awake()
    {
        // Get health component 
        health = GetComponent<HealthTracker>();
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
