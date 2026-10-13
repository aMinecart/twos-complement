using UnityEngine;

public static class VectorHelpers
{
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