using UnityEngine;

// reference for common patterns between weapons (e.g. fire a single hitscan ray,
// fire multiple hitscan rays at once, fire a projectile, reload one bullet, reload all bullets, etc.)
// allows for plug and play functionality

public static class WeaponComponents
{
    public static void FireSingleHitscan(Transform origin, HealthTracker ownerHealth, float damage, float range)
    {
        if (origin == null)
            return;

        // Shoot a ray forward from firing origin
        RaycastHit[] hits = Physics.RaycastAll(
            origin.position,
            origin.forward,
            range
        );

        // Check closest hits first 
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            Debug.Log("Hit: " + hit.collider.name);

            // Check if has health component
            HealthTracker targetHealth = hit.collider.GetComponentInParent<HealthTracker>();

            // Ignore the character using the weapon
            if (targetHealth != null && targetHealth == ownerHealth)
                continue;

            // Damge only the target 
            if (targetHealth != null)
                targetHealth.TakeDamage(damage);

            // Stop at first non-owner collider 
            break; 
        }
    }

    public static void FireSprayHitscan(Transform origin, HealthTracker ownerHealth, float damage, float range, int palletCount, float spreadAngle)
    {
        if (origin == null || range <= 0f || palletCount <= 0)
            return;
        
        // fires multiple rays in a cone shape to simulate a spray effect
        for (int i = 0; i < palletCount; i++)
        {
            // Genrate random direction within spread angle
            Vector2 spread = Random.insideUnitCircle *
                Mathf.Tan(Mathf.Clamp(spreadAngle, 0f, 89f) * Mathf.Deg2Rad);

            Vector3 direction = (
                origin.forward +
                origin.right * spread.x +
                origin.up * spread.y
            ).normalized;

            // Shoot ray forward from firing origin
            RaycastHit[] hits = Physics.RaycastAll(
                origin.position,
                direction,
                range
            );

            // Check closest hits 
            System.Array.Sort(hits, (a,b) => a.distance.CompareTo(b.distance));

            foreach (RaycastHit hit in hits)
            {
                // Check if has health component 
                HealthTracker targetHealth = hit.collider.GetComponentInParent<HealthTracker>();

                // Ignore character using weapon 
                if (targetHealth != null && targetHealth == ownerHealth)
                    continue;
                
                // Damage only the target 
                if (targetHealth != null)
                    targetHealth.TakeDamage(damage);

                // Stop at first non-owner collider
                break;
            }
        }
    }

    public static void FireProjectile(Transform origin, GameObject projectilePrefab, HealthTracker ownerHealth, float damage, float speed, float range)
    {
        if (origin == null || projectilePrefab == null)
            return;
        
        // spawns a projectile to simulate projectile-based attacks
        GameObject projectile = Object.Instantiate(
            projectilePrefab,
            origin.position,
            origin.rotation
        );

        // Initialize projectile damage, owner, and movement
        if (projectile.TryGetComponent<Projectile>(out Projectile projectileScript))
        {
            projectileScript.Initialize(ownerHealth, damage, speed, range);
        }
        else
        {
            Debug.LogWarning("Projectile prefab is missing Projectile.cs");
            Object.Destroy(projectile);
        }
    }
}