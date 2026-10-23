using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class MovementController : MonoBehaviour
{
    // player input management variables
    public Vector2 moveInputDir { get; private set; } = Vector2.zero;
    public Vector2 lookInputDir { get; private set; } = Vector2.zero;
    private bool attemptJump = false;
    private bool attemptDash = false;

    // player status variables
    public bool grounded { get; private set; } = false;
    public bool inCrouch { get; private set; } = false;
    public bool inDash { get; private set; } = false;

    // inputs to collision detection system
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float maxGroundDistance = 0.1f;

    // relevant player components
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private StatsManager playerStats;
    [SerializeField] private GameObject itemOrigin;

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInputDir = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Vector2 readVal = context.ReadValue<Vector2>();
        lookInputDir = new Vector2(
            lookInputDir.x + playerStats.look_sensitivity * readVal.x,
            Mathf.Clamp(
                lookInputDir.y + playerStats.look_sensitivity * readVal.y,
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

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            attemptDash = true;
        }

        if (context.canceled)
        {
            attemptDash = false;
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
        rb.rotation = Quaternion.Euler(0.0f, lookInputDir.x, 0.0f);
        itemOrigin.transform.rotation = Quaternion.Euler(-lookInputDir.y, lookInputDir.x, 0.0f);

        // check if player is grounded by protuding a sphere below the player's collider
        Bounds bounds = capsuleCollider.bounds;
        grounded = Physics.CheckSphere(position: bounds.center - new Vector3(0, bounds.extents.y, 0),
                                       radius: maxGroundDistance,
                                       layerMask: groundMask);
        
        // skip physics calculations if player cannot move
        if (rb.isKinematic || inDash)
        {
            return;
        }

        // apply friction if player is grounded
        if (grounded)
        {
            rb.linearVelocity = PlayerPhysics.ApplyFriction(
                currVelocity: rb.linearVelocity,
                frictionRate: playerStats.friction_rate,
                minStopSpeed: playerStats.min_stop_speed
            );
        }

        // rotate inputted move direction by rigidbody's current rotation
        VectorNorm3 wishDir = new VectorNorm3(rb.rotation * moveInputDir.ToXZVector3());

        // apply accleration based on player's desired acceleration and if airborne
        rb.linearVelocity = PlayerPhysics.Accelerate(
            currVelocity: rb.linearVelocity,
            accelDir: wishDir,
            accelRate: (grounded ? playerStats.accel_rate : playerStats.accel_rate * (1 / playerStats.air_reduction_factor)),
            maxSpeed: (grounded ? playerStats.max_move_speed : playerStats.max_move_speed * playerStats.air_reduction_factor)
        );

        // apply jump force if player meets jump requirements
        if (attemptJump && grounded && !inCrouch)
        {
            rb.linearVelocity = PlayerPhysics.ApplyJumpForce(
                currVelocity: rb.linearVelocity,
                jumpHeight: playerStats.jump_height,
                gravityAccel: playerStats.gravity_accel,
                gravityMultiplier: playerStats.gravity_multiplier
            );            
        }
        else if (attemptDash && !inDash)
        {
            if (!grounded)
            {
                // remove any vertical speed accumulated by the player
                rb.linearVelocity = PlayerPhysics.ApplyFloatForce(
                    currVelocity: rb.linearVelocity,
                    floatStrengh: playerStats.float_strength,
                    gravityAccel: playerStats.gravity_accel,
                    gravityMultiplier: playerStats.gravity_multiplier
                );
            }

            // apply dash force if player is not already dashing
            rb.linearVelocity = PlayerPhysics.ApplyDashForce(
                currVelocity: rb.linearVelocity,
                dashDir: wishDir,
                dashSpeed: playerStats.dash_speed,
                maxDashSpeed: playerStats.max_dash_speed,
                asImpulse: !grounded
            );

            if (grounded)
            {
                // lock player in ground dash until timer expires
                inDash = true;
                StartCoroutine(HandleDashTimer(Time.fixedTime, playerStats.dash_distance / playerStats.dash_speed));
            }
        }

        // apply gravity to the player unconditionally
        rb.linearVelocity = PlayerPhysics.ApplyGravity(
            currVelocity: rb.linearVelocity,
            gravityAccel: playerStats.gravity_accel,
            gravityMultiplier: playerStats.gravity_multiplier
        );

        attemptJump = false; // prevent player from jumping until they release and press the button again
        attemptDash = false; // prevent player from dashing until they release and press the button again

        // display current movement information
        // print($"input vector: {moveInputDir}");
        // print($"grounded: {grounded}, velocity: {rb.linearVelocity}");
        // print($"in crouch: {inCrouch}, in dash: {inDash}");
    }

    private IEnumerator HandleDashTimer(float startTime, float duration)
    {
        float endTime = startTime + duration;
        while (Time.fixedTime + 10 * Mathf.Epsilon < endTime)
        {
            print($"Time to next dash: {startTime + duration - Time.fixedTime}");
            yield return new WaitForFixedUpdate();
        }

        inDash = false;
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