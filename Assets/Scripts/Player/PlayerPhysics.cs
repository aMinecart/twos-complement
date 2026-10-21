using UnityEngine;

public static class PlayerPhysics
{
    // base physics constants
    public const float BASE_ACCEL_RATE = 5.6f;
    public const float BASE_MAX_SPEED = 5.715f;
    public const float BASE_MIN_STOP_SPEED = 1.905f;
    public const float BASE_FRICTION_RATE = 4.8f;
    public const float BASE_GRAVITY_ACCEL = 15.24f;
    public const float BASE_GRAVITY_MULTIPLIER = 1.0f;
    public const float BASE_AIR_REDUCTION_FACTOR = 0.1f;
    public const float BASE_JUMP_HEIGHT = 0.1677050983f;
    public const float BASE_FLOAT_STRENGTH = 0.1f;
    public const float BASE_DASH_SPEED = 2.0f * BASE_MAX_SPEED;


    // basic physics functions
    public static Vector3 ApplyGravity(Vector3 currVelocity, float gravityAccel, float gravityMultiplier = 1.0f)
    {
        currVelocity.y -= gravityAccel * gravityMultiplier * Time.fixedDeltaTime;
        return currVelocity;
    }

    public static Vector3 ApplyFriction(Vector3 currVelocity, float frictionRate, float minStopSpeed)
    {
        float currSpeed = currVelocity.magnitude;

        if (currSpeed != 0)
        {
            float control = Mathf.Max(currSpeed, minStopSpeed);
            float speedReduction = control * frictionRate * Time.fixedDeltaTime;

            currVelocity *= Mathf.Max(currSpeed - speedReduction, 0) / currSpeed;
        }

        return currVelocity;
    }

    public static Vector3 Accelerate(Vector3 currVelocity, VectorNorm3 accelDir, float accelRate, float maxSpeed)
    {
        float projSpeed = Vector3.Dot(currVelocity, accelDir);
        float accelSpeed = accelRate * maxSpeed * Time.fixedDeltaTime;

        if (projSpeed + accelSpeed > maxSpeed)
        {
            accelSpeed = Mathf.Max(maxSpeed - projSpeed, 0);
        }

        return currVelocity + accelSpeed * (Vector3)accelDir;
    }

    public static Vector3 ApplyJumpForce(Vector3 currVelocity, float jumpHeight, float gravityAccel, float gravityMultiplier = 1.0f)
    {
        currVelocity.y = 2 * gravityAccel * gravityMultiplier * jumpHeight;
        return currVelocity;
    }

    public static Vector3 ApplyFloatForce(Vector3 currVelocity, float floatStrengh, float gravityAccel, float gravityMultiplier = 1.0f)
    {
        currVelocity.y = floatStrengh * gravityAccel * gravityMultiplier;
        return currVelocity;
    }

    public static Vector3 ApplyDashForce(Vector3 currVelocity, VectorNorm3 dashDir, float dashSpeed, float relativeDistance = 1.0f, bool asImpulse = false)
    {
        if (!asImpulse)
        {
            return dashSpeed * (Vector3)dashDir;
        }

        float projSpeed = Vector3.Dot(currVelocity, dashDir);
        if (projSpeed > dashSpeed)
        {
            projSpeed = Mathf.Max(dashSpeed - projSpeed, 0);
        }

        return currVelocity + projSpeed * (Vector3)dashDir;
    }
}