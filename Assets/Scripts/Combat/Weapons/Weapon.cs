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
    [SerializeField] protected GameObject weaponModel; //Model for the weapon
    public GameObject WeaponModel => weaponModel;
    [SerializeField] protected Animation weaponAnimation; //Weapon animation
    public Animation WeaponAnimation => weaponAnimation;
    [SerializeField] protected AudioSource weaponSound; //Weapon animation
    public AudioSource WeaponSound => weaponSound;



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
    public virtual void Reload() { }
}