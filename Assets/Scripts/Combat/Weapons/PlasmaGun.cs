using UnityEngine;

// Fires moving projectiles
public class PlasmaGun : RangedWeapon
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed;

    // Fire plasma projectile
    protected override void Shoot()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("PlasmaGun: Projectile prefab not assigned");
            return;
        }

        currentAmmo--;
        Debug.Log("Plasma Ammo: " + currentAmmo + "/" + magazineSize);

        WeaponComponents.FireProjectile(
            firingOriginTransform,
            projectilePrefab,
            ownerHealth,
            damage,
            projectileSpeed,
            range
        );
    }
}
