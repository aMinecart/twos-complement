using UnityEngine;

// Shotgun fires multiple pellets in a spread 
public class Shotgun : RangedWeapon
{
    [SerializeField] private int pelletCount;
    [SerializeField] private float spreadAngle;

    public override float CrosshairSpreadAngle => spreadAngle;
    public override float CrosshairFireKick => 16f;
    
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
            pelletCount,
            spreadAngle
        );
    }
}
