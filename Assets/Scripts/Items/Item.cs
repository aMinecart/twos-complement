using UnityEngine;

public interface IPickup
{
    
}

public enum BuffType
{
    FIRE_RATE,
    MAX_AMMO,
    COOLDOWN,
    DAMAGE,
    MAX_HEALTH,
    DAMAGE_RESIST,
    MOVE_SPEED
}



public struct PermanentBuff : IPickup
{
    public BuffType buffType;
    public float percentIncrease;

    public void applyBuff(PlayerStats playerStats)
    {
        
    }
}