using UnityEngine;

[RequireComponent(typeof(UnityEngine.InputSystem.PlayerInput))]
[RequireComponent(typeof(PlayerReference))]
[RequireComponent(typeof(PlayerInventory))]
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(HealthTracker))]

public class PlayerManager : MonoBehaviour
{
    public PlayerInventory inventory { get; private set; }
    public MovementController movementController { get; private set; }
    public HealthTracker healthTracker { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventory = GetComponent<PlayerInventory>();
        movementController = GetComponent<MovementController>();
        healthTracker = GetComponent<HealthTracker>();
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: move script toggles to OnEnable/OnDisable and handle script disabling here
    }
}