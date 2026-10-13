using UnityEngine;
public abstract class Weapon : MonoBehaviour
{
    // Shared weapon stats
    [SerializeField] protected float damage;
    [SerializeField] protected float range;
    [SerializeField] protected float fireRate;
    
    // Where attack starts
    [SerializeField] protected Transform firingOriginTransform;
    // Character using weapon
    [SerializeField] protected HealthTracker ownerHealth;

    protected float nextAttackTime = 0f;

    public void Initialize(HealthTracker owner, Transform origin)
    {
        ownerHealth = owner;
        firingOriginTransform = origin;
    }

    protected bool CanUseWeapon()
    {
        return isActiveAndEnabled && 
            GameManager.instance != null && 
            GameManager.instance.CurrentState == GameManager.GameState.Playing;
    }

    // Shared cooldown check 
    protected bool CanAttack()
    {
        return CanUseWeapon() && Time.time >= nextAttackTime;
    }

    // Called by WeaponManager or enemy AI
    public abstract bool Attack();

    // Optional for weapons that support reloading 
    public virtual void Reload()
    {
    }
}