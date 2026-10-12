using UnityEngine;

[RequireComponent(typeof(UnityEngine.InputSystem.PlayerInput))]
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(PlayerStats))]
public class PlayerManager : MonoBehaviour
{
    private MovementController movementController;
    private PlayerStats playerStats;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movementController = GetComponent<MovementController>();
        playerStats = GetComponent<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}