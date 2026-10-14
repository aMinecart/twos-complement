using UnityEngine;
using System.Collections;

// the actual weapon class only needs to override the Shoot and ReloadCoroutine methods,
// making creating new weapons much easier
public class Gun : RangedWeapon
{
    // Fire gun check if it hits smth 
    protected override void Shoot()
    {
        // can eventually be replaced with calls to WeaponsComponents
        currentAmmo--;
        Debug.Log("Ammo: " + currentAmmo + "/" + magazineSize);

        // Shoot a ray forward from firing origin
        if (Physics.Raycast(
            firingOriginTransform.position,
            firingOriginTransform.forward,
            out RaycastHit hit))
        {
            Debug.Log("Hit: " + hit.collider.name);

            // Check if object hit has a health component 
            HealthTracker targetHealth = hit.collider.GetComponentInParent<HealthTracker>();
            // Damage the target, but not character using weapon 
            if (targetHealth != null && targetHealth != ownerHealth)
            {
                targetHealth.TakeDamage(damage);
            }
        }
    }

    // Wait for reload time before refilling mag 
    protected override IEnumerator ReloadCoroutine()
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
                Debug.Log("Reload canceled.");
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