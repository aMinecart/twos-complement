using UnityEngine;

[RequireComponent(typeof(HealthTracker))]
public class EnemyDeath : MonoBehaviour
{
    private HealthTracker health;

    void Awake()
    {
        // Get health component 
        health = GetComponent<HealthTracker>();
    }

    void OnEnable()
    {
        // Listen for death event 
        Debug.Log("GameObject name: " + gameObject.name);
        health.OnDeath += HandleDeath;
    }

    void OnDisable()
    {
        health.OnDeath -= HandleDeath;
    }

    private void HandleDeath()
    {
        // Remove enemy from scene when it dies
        if(HeatController.Instance != null) HeatController.Instance.currentHeat += 5;
        Destroy(gameObject);
    }
}
