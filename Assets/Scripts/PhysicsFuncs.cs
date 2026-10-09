using UnityEngine;

public static class PhysicsFuncs
{
    public static Vector3 accelerate(Vector3 currVelocity, VectorNorm3 accelDir, float accelRate, float maxSpeed)
    {
        float projSpeed = Vector3.Dot(currVelocity, accelDir);
        float accelSpeed = accelRate * maxSpeed * Time.fixedDeltaTime;

        if (projSpeed + accelSpeed > maxSpeed)
        {
            accelSpeed = Mathf.Max(maxSpeed - projSpeed, 0);
        }

        return currVelocity + accelSpeed * (Vector3)accelDir;
    }

    public static Vector3 ToXZVector3(this Vector2 v, float y = 0.0f)
    {
        return new Vector3(v.x, y, v.y);
    }
}

public struct VectorNorm3
{
    public Vector3 vec { get; }

    public VectorNorm3(Vector3 v)
    {
        vec = v.normalized;
    }

    public static implicit operator Vector3(VectorNorm3 vn)
    {
        return vn.vec;
    }
}