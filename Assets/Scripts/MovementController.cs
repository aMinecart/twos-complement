using UnityEngine;
using UnityEngine.InputSystem;
using static PhysicsFuncs;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class MovementController : MonoBehaviour
{
    public float accel_rate { get; } = 5.6f;
    public float max_speed { get; } = 5.715f;

    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;

    private Vector2 moveDir = Vector2.zero;

    public void OnMove(InputAction.CallbackContext context)
    {
        moveDir = context.ReadValue<Vector2>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    // FixedUpdate is called once per fixed framerate frame
    void FixedUpdate()
    {
        rb.linearVelocity = accelerate(
            rb.linearVelocity,
            new VectorNorm3(moveDir.ToXZVector3()),
            accel_rate,
            max_speed
        );

        // print($"input vector: {moveDir}");
    }
}