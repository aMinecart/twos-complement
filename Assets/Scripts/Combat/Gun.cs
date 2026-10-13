using UnityEngine;
using System.Collections;

public class Gun : Weapon
{
    // Gun specific stats 
    [SerializeField] private int magazineSize;
    [SerializeField] private float reloadTime;

    private int currentAmmo;
    private bool isReloading = false;

    void Start()
    {
        currentAmmo = magazineSize;
    }

    // Called when the player or enemy tries to fire 
    public override bool Attack()
    {
        if(!CanAttack() || isReloading)
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

    // Fire gun check if it hits smth 
    public void Shoot()
    {
        currentAmmo--;
        Debug.Log("Ammo: " + currentAmmo + "/" + magazineSize);

        // Shoot a ray forward from firing origin
        if (Physics.Raycast(
            firingOriginTransform.position,
            firingOriginTransform.forward,
            out RaycastHit hit,
            range))
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

    // Reload the gun when requested 
    public override void Reload()
    {
        // Don;t reload if game isn't playing, already reloading or mag is full 
        if (!CanUseWeapon() || isReloading || currentAmmo == magazineSize)
            return;

        StartCoroutine(ReloadCoroutine());
    }

    // Wait for reload time before refilling mag 
    private IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        Debug.Log("Reloading...");
        float elapsedTime = 0f;

        while (elapsedTime < reloadTime)
        {
            // Cancel reload if player dies or game stops 
            if(!CanUseWeapon())
            {
                isReloading = false;
                Debug.Log("Reload canceled.");
                yield break;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (!CanUseWeapon())
        {
            isReloading = false;
            Debug.Log("Reload canceled.");
            yield break;
        }
        
        currentAmmo = magazineSize;
        isReloading = false;

        Debug.Log("Reload complete. Ammo: " + currentAmmo + "/" + magazineSize);
    }

    private void OnDisable()
    {
        // Stop reloading if gun is unequipped or disabled 
        StopAllCoroutines();
        isReloading = false;
    }
}
