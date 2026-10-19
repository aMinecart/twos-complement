using System.Collections;
using UnityEngine;

// TODO:
// - Coordinate initial magazine setup with saved ammunition restoration
// so Start() doesnt overwrite ammunition restored earlier

// shared functionality for all weapons that use ammo and can reload
public abstract class RangedWeapon : Weapon
{
    // Ranged weapon specific stats
    [SerializeField] protected int magazineSize;
    [SerializeField] protected float reloadTime;

    protected int currentAmmo;
    protected bool isReloading = false;

    // Allow the UI and save system to read ammunition
    public int CurrentAmmo => currentAmmo;
    public int MagazineSize => magazineSize;

    // Restore ammunition without exposing internal variables
    public void SetAmmo(int amount)
    {
        currentAmmo = Mathf.Clamp(amount, 0, magazineSize);
    }

    // weapon specific implementation of firing and reload
    protected abstract void Shoot();

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
        base.Attack();

        // Set time when gun can shoot again
        nextAttackTime = Time.time + fireRate;
        return true;
    }

    // Reload the gun when requested
    public override void Reload()
    {
        // Don't reload if game isn't playing, already reloading or mag is full 
        if (!CanUseWeapon() || isReloading || currentAmmo == magazineSize)
            return;

        StartCoroutine(ReloadCoroutine());
    }

    // Shared realoding functionality
    protected virtual IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        float elapsedTime = 0f;
        while (true)
        {
            // Cancel reload if player dies or game stops
            if(!CanUseWeapon())
            {
                isReloading = false;
                Debug.Log("Reload canceled");
                yield break;
            }

            // Complete rest of reload once sufficent time has passed
            if (elapsedTime >= reloadTime)
            {
                break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        currentAmmo = magazineSize;
        isReloading = false;

        Debug.Log("Reload complete. Ammo: " + currentAmmo + "/" + magazineSize);
    }
}