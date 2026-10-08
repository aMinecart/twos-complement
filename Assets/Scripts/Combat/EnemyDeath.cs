using UnityEngine;

[RequireComponent(typeof(Health))]
public class EnemyDeath : MonoBehaviour
{
    private Health health;

    void Awake()
    {
        // Get health component 
        health = GetComponent<Health>();
    }

    void OnEnable()
    {
        // Listen for death event 
        health.OnDeath += HandleDeath;
    }

    void OnDisable()
    {
        health.OnDeath -= HandleDeath;
    }

    private void HandleDeath()
    {
        // Remove enemy from scene when it dies 
        Destroy(gameObject);
    }
}
