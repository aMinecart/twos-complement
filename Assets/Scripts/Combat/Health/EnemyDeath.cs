using UnityEngine;

[RequireComponent(typeof(HealthTracker))]
public class EnemyDeath : MonoBehaviour
{
    public GameObject BossArenaWalls; //Optional
    public GameObject BossArenaColliders; //Optional
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
        Debug.Log("GameObject name: " + gameObject.name);
        health.OnDeath += HandleDeath;
    }

    void OnDisable()
    {
        health.OnDeath -= HandleDeath;
    }

    private void HandleDeath()
    {
        if(gameObject.tag == "Boss")
        {
            BossArenaWalls.SetActive(false);
            BossArenaColliders.SetActive(false);
            //TODO: Spawn Weapon Here
            Destroy(gameObject);
            return;
        }
        // spawn random buff at enemy location
        BuffPickup.CreateRandBuffPickup(GameManager.rng, basePickupPrefab, transform.position);

        // increase global heat value
        if (HeatController.Instance != null)
        {
            HeatController.Instance.currentHeat += 5;
        }

        // Remove enemy from scene when it dies
        Destroy(gameObject);
    }
}
