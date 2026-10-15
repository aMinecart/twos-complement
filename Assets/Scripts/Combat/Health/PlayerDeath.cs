using UnityEngine;

[RequireComponent(typeof(HealthTracker))]
public class PlayerDeath : MonoBehaviour
{
    private HealthTracker health; 
    private bool isGameOver = false;

    void Awake()
    {
        // Get player health component
        health = GetComponent<HealthTracker>();
    }

    void OnEnable()
    {
        // Listen for player's death 
        health.OnDeath += HandleDeath;
    }

    void OnDisable()
    {
        health.OnDeath -= HandleDeath;
    }

    private void HandleDeath()
    {
        // Die processed only once
        if(isGameOver) return;

        isGameOver = true;

        Debug.Log("GAME OVER - Player has died!");

        if (GameManager.instance != null)
        {
            GameManager.instance.GameOver();
        }

        // Disable player movement and combat 
        // Display Game over screen 
        // Allow player to restart or respawn
    }
}
