using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(HealthTracker))]
public class EnemyDeath : MonoBehaviour
{
    [Header("For Bosses")]
    public GameObject BossArenaWalls;
    public GameObject BossArenaColliders;
    public GameObject teleporter;
    private HealthTracker health;
    [Header("Other")]
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
            if(BossArenaWalls != null) BossArenaWalls.SetActive(false);
            if(BossArenaColliders != null) BossArenaColliders.SetActive(false);
            if(teleporter != null) teleporter.GetComponent<TeleporterCode>().enabled = true;
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
