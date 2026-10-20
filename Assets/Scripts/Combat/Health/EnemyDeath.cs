using UnityEngine;

[RequireComponent(typeof(HealthTracker))]
public class EnemyDeath : MonoBehaviour
{
    private HealthTracker health;
    [SerializeField] private BuffPickup basePickupPrefab;

    void Awake()
    {
        // Get health component 
        health = GetComponent<HealthTracker>();
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
        // spawn random buff at enemy location
        BuffPickup.CreateRandBuffPickup(GameManager.rng, basePickupPrefab, transform.position);

        // Remove enemy from scene when it dies
        Destroy(gameObject);
    }
}
