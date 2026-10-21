using System;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    // Stable identifier used by inventory and save system
    [SerializeField] private string weaponId;
    public string WeaponId => weaponId;

    // Shared weapon stats
    [SerializeField] protected float damage;
    [SerializeField] protected float fireRate;
    [SerializeField] protected float range;

    [SerializeField] protected Transform firingOriginTransform; // Where attack starts
    [SerializeField] protected HealthTracker ownerHealth; // Character using weapon

    protected float nextAttackTime = 0f;

    public static event Action AttackSuccess;
    public static event Action EnemyHit;

    public void Initialize(HealthTracker owner, Transform origin)
    {
        ownerHealth = owner;
        firingOriginTransform = origin;
    }

    public void ModifyFireRate(float multiplier)
    {
        Debug.Log($"Old weapon fire rate: {fireRate}");
        fireRate *= multiplier;
        Debug.Log($"New weapon fire rate: {fireRate}");
    }

    public void ModifyDamage(float multiplier)
    {
        Debug.Log($"Old weapon damage: {damage}");
        damage *= multiplier;
        Debug.Log($"New weapon damage: {damage}");
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
    public virtual bool Attack()
    {
        if (CanAttack())
        {
            InvokeAttackSuccess();
            return true;
        }
        else
        {
            return false;
        }
    }

    // Optional for weapons that support reloading
    public virtual void Reload() { }

    protected virtual void InvokeAttackSuccess()
    {
        AttackSuccess?.Invoke();
    }

    protected virtual void InvokeEnemyHit()
    {
        EnemyHit?.Invoke();
    }
}