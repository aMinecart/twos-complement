using UnityEngine;

// Shotgun fires multiple pallets in a spread 
public class Shotgun : RangedWeapon
{
    [SerializeField] private int palletCount;
    [SerializeField] private float spreadAngle;

    // Fire pallets
    protected override void Shoot()
    {
        currentAmmo--;
        Debug.Log("Shotgun Ammo: " + currentAmmo + "/" + magazineSize);

        WeaponComponents.FireSprayHitscan(
            firingOriginTransform,
            ownerHealth,
            damage,
            range,
            palletCount,
            spreadAngle
        );
    }
}
