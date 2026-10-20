using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class MovementController : MonoBehaviour
{
    // player input management variables
    public Vector2 moveDir { get; private set; } = Vector2.zero;
    public Vector2 lookDir { get; private set; } = Vector2.zero;
    private bool attemptJump = false;

    // player status variables
    public bool grounded { get; private set; } = false;
    public bool inCrouch { get; private set; } = false;

    // inputs to collision detection system
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float maxGroundDistance = 0.1f;

    // relevant player components
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private StatsManager playerStats;

    public void OnMove(InputAction.CallbackContext context)
    {
        moveDir = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Vector2 readVal = context.ReadValue<Vector2>();
        lookDir = new Vector2(
            lookDir.x + playerStats.look_sensitivity * readVal.x,
            Mathf.Clamp(
                lookDir.y + playerStats.look_sensitivity * readVal.y,
                -playerStats.vert_look_range_degrees / 2,
                playerStats.vert_look_range_degrees / 2
            )
        );
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            attemptJump = true;
        }

        if (context.canceled)
        {
            attemptJump = false;
        }
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
        playerStats = GetComponent<StatsManager>();
    }

    // FixedUpdate is called once per fixed framerate frame
    void FixedUpdate()
    {
        // disable movement if the game is not active
        if (GameManager.instance.CurrentState != GameManager.GameState.Playing)
        {
            rb.linearVelocity = Vector3.zero;
            this.enabled = false;
            return;
        }

        // update player rotation
        rb.rotation = Quaternion.Euler(0.0f, lookDir.x, 0.0f);

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
            rb.linearVelocity = PlayerPhysics.ApplyFriction(
                rb.linearVelocity,
                playerStats.friction_rate,
                playerStats.min_stop_speed
            );
        }

        // apply accleration based on player's desired acceleration and if airborne
        rb.linearVelocity = PlayerPhysics.Accelerate(
            rb.linearVelocity,
            new VectorNorm3(rb.rotation * moveDir.ToXZVector3()), // rotate desired direction by rigidbody's current rotation
            (grounded ? playerStats.accel_rate : playerStats.accel_rate * playerStats.air_reduction_factor),
            (grounded ? playerStats.max_move_speed : playerStats.max_move_speed * playerStats.air_reduction_factor)
        );

        // apply gravity to the player unconditionally
        rb.linearVelocity = PlayerPhysics.ApplyGravity(
            rb.linearVelocity,
            playerStats.gravity_accel,
            playerStats.gravity_multiplier
        );

        // apply jump force if player meets jump requirements
        if (attemptJump && grounded && !inCrouch)
        {
            rb.linearVelocity = PlayerPhysics.ApplyJumpForce(
                rb.linearVelocity,
                playerStats.jump_height,
                playerStats.gravity_accel,
                playerStats.gravity_multiplier
            );            
        }

        attemptJump = false; // prevent player from jumping until they release and press the button again

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