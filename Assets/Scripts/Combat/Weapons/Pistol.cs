using UnityEngine;

// the actual weapon class only needs to override the Shoot and ReloadCoroutine methods,
// making creating new weapons much easier
public class Pistol : RangedWeapon
{
    // Fire gun check if it hits smth 
    protected override void Shoot()
    {
        // can eventually be replaced with calls to WeaponsComponents --> done
        currentAmmo--;
        Debug.Log("Ammo: " + currentAmmo + "/" + magazineSize);

        // Shoot a ray forward from firing origin
        WeaponComponents.FireSingleHitscan(
            firingOriginTransform,
            ownerHealth,
            damage,
            range
        );
    }
}