using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class MovementController : MonoBehaviour
{
    // player status variables
    public Vector2 moveDir { get; private set; } = Vector2.zero;
    public bool grounded { get; private set; } = false;

    // inputs to collision detection system
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float maxGroundDistance = 0.1f;

    // relevant player components
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private PlayerStats playerStats;

    public void OnMove(InputAction.CallbackContext context)
    {
        moveDir = context.ReadValue<Vector2>();
    }

    void Awake()
    {
        groundMask = LayerMask.GetMask("Terrain");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        playerStats = GetComponent<PlayerStats>();
    }

    // FixedUpdate is called once per fixed framerate frame
    void FixedUpdate()
    {
        if (GameManager.instance.CurrentState == GameManager.GameState.GameOver)
        {
            rb.linearVelocity = Vector3.zero;
            this.enabled = false;
            return;
        }

        // check if player is grounded by protuding a sphere below the player's collider
        Bounds bounds = capsuleCollider.bounds;
        grounded = Physics.CheckSphere(bounds.center - new Vector3(0, bounds.extents.y, 0),
                                       maxGroundDistance,
                                       groundMask);

        // choose which acceleration to use based on if the player is airborne
        /*
        float accelRate = this.accel_rate;
        float maxSpeed = this.max_speed;
        if (!grounded)
        {
            accelRate *= air_reduction_factor;
            maxSpeed *= air_reduction_factor;
        }
        */

        // apply friction if player is grounded
        if (grounded)
        {
            rb.linearVelocity = PlayerPhysics.applyFriction(
                rb.linearVelocity,
                playerStats.friction_rate,
                playerStats.min_stop_speed
            );
        }

        // apply accleration based on player's desired acceleration and if airborne
        rb.linearVelocity = PlayerPhysics.accelerate(
            rb.linearVelocity,
            new VectorNorm3(moveDir.ToXZVector3()),
            (grounded ? playerStats.accel_rate : playerStats.accel_rate * playerStats.air_reduction_factor),
            (grounded ? playerStats.max_move_speed : playerStats.max_move_speed * playerStats.air_reduction_factor)
        );

        // apply gravity to the player unconditionally
        rb.linearVelocity = PlayerPhysics.applyGravity(
            rb.linearVelocity,
            playerStats.gravity_accel
        );

        // display current movement information
        // print($"input vector: {moveDir}");
        // print($"grounded: {grounded}, velocity: {rb.linearVelocity}");
    }

    /*
    void OnDrawGizmosSelected()
    {
        if (capsuleCollider == null) { return; }

        Bounds bounds = capsuleCollider.bounds;
        Gizmos.color = Color.azure;
        Gizmos.DrawSphere(bounds.center - new Vector3(0, bounds.extents.y, 0), maxGroundDistance);
    }
    */
}