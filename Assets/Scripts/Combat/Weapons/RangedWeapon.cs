using System.Collections;
using UnityEngine;

public abstract class RangedWeapon : Weapon
{
    // Ranged weapon specific stats 
    [SerializeField] protected int magazineSize;
    [SerializeField] protected float reloadTime;

    protected int currentAmmo;
    protected bool isReloading = false;

    // weapon specific implementation of firing and reload
    protected abstract void Shoot();
    protected abstract IEnumerator ReloadCoroutine();

    // set ammo to full when the script first stats
    void Start()
    {
        currentAmmo = magazineSize;
    }

    protected void OnDisable()
    {
        // Stop reloading if gun is unequipped or disabled 
        StopAllCoroutines();
        isReloading = false;
    }

    // Called when the player or enemy tries to fire
    public override bool Attack()
    {
        if (!CanAttack() || isReloading)
            return false;

        if (firingOriginTransform == null)
        {
            Debug.LogWarning("Gun: Firing origin not assigned");
            return false;
        }

        if (currentAmmo <= 0)
        {
            Debug.Log("Out of ammo! Press R to reload");
            return false;
        }

        Shoot();

        // Set time when gun can shoot again 
        nextAttackTime = Time.time + fireRate;
        return true;
    }

    // Reload the gun when requested 
    public override void Reload()
    {
        // Don;t reload if game isn't playing, already reloading or mag is full 
        if (!CanUseWeapon() || isReloading || currentAmmo == magazineSize)
            return;

        StartCoroutine(ReloadCoroutine());
    }
}