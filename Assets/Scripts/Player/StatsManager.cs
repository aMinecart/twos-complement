using UnityEngine;

public class StatsManager : MonoBehaviour
{
    private PlayerManager playerManager;

    // player physics constants
    public float gravity_accel { get; set; } = PlayerPhysics.BASE_GRAVITY_ACCEL;
    public float friction_rate { get; set; } = PlayerPhysics.BASE_FRICTION_RATE;
    public float accel_rate { get; set; } = PlayerPhysics.BASE_ACCEL_RATE;
    public float max_move_speed { get; set; } = PlayerPhysics.BASE_MAX_SPEED;
    public float min_stop_speed { get; set; } = PlayerPhysics.BASE_MIN_STOP_SPEED;
    public float air_reduction_factor { get; set; } = PlayerPhysics.BASE_AIR_REDUCTION_FACTOR;
    public float gravity_multiplier { get; set; } = PlayerPhysics.BASE_GRAVITY_MULTIPLIER;
    public float jump_height { get; set; } = PlayerPhysics.BASE_JUMP_HEIGHT;

    // player control constants
    public float look_sensitivity { get; set; } = 0.1f;
    public float vert_look_range_degrees { get; set; } = 115.0f;

    // multiplicative player bonus stats
    public float fire_rate_multiplier { get; set; } = 1.0f;
    public float global_cooldown_multiplier { get; set; } = 1.0f;
    public float max_ammo_multiplier { get; set; } = 1.0f;
    public float weapon_damage_multiplier { get; set; } = 1.0f;

    // additive player bonus stats
    public int max_health_bonus { get; set; } = 0;
    public float move_speed_bonus { get; set; } = 0.0f;

    // percentage-based player bonus stats
    public float damage_resist_percent { get; set; } = 0.0f;

    void Start()
    {
        playerManager = GetComponent<PlayerManager>();
    }

    // applies the given buff type and value to the player
    public void ApplyBuff(BuffType buffType, float buffValue)
    {
        print($"Applying {buffType} of amount {buffValue} to player");

        switch (buffType)
        {
            case BuffType.FIRE_RATE:
                fire_rate_multiplier -= buffValue;
                break;

            case BuffType.COOLDOWN:
                global_cooldown_multiplier -= buffValue;
                break;

            case BuffType.MAX_AMMO:
                max_ammo_multiplier += buffValue;
                break;

            case BuffType.DAMAGE:
                weapon_damage_multiplier += buffValue;
                break;

            case BuffType.MAX_HEALTH:
                max_health_bonus += (int)buffValue; // record player health bonus
                playerManager.healthTracker.AddMaxHealth((int)buffValue); // increase player max health
                break;

            case BuffType.MOVE_SPEED:
                move_speed_bonus += buffValue; // record player speed bonus
                max_move_speed += buffValue; // increase player move speed
                break;

            case BuffType.DAMAGE_RESIST:
                damage_resist_percent += buffValue;
                break;

            default:
                Debug.LogError($"Stats manager unable to handle buff of type {buffType}");
                break;
        }
    }
}