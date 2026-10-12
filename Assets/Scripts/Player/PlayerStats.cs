using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    // player physics constants
    public float gravity_accel { get; set; } = PlayerPhysics.BASE_GRAVITY_ACCEL;
    public float friction_rate { get; set; } = PlayerPhysics.BASE_FRICTION_RATE;
    public float accel_rate { get; set; } = PlayerPhysics.BASE_ACCEL_RATE;
    public float max_move_speed { get; set; } = PlayerPhysics.BASE_MAX_SPEED;
    public float min_stop_speed { get; set; } = PlayerPhysics.BASE_MIN_STOP_SPEED;
    public float air_reduction_factor { get; set; } = PlayerPhysics.BASE_AIR_REDUCTION_FACTOR;
    public float gravity_multiplier { get; set; } = PlayerPhysics.BASE_GRAVITY_MULTIPLIER;
}