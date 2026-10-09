using UnityEngine;

[RequireComponent(typeof(UnityEngine.InputSystem.PlayerInput))]
[RequireComponent(typeof(MovementController))]
public class PlayerManager : MonoBehaviour
{
    private MovementController movementController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movementController = GetComponent<MovementController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}