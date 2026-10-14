using UnityEngine;

// reference for common patterns between weapons (e.g. fire a single hitscan ray,
// fire multiple hitscan rays at once, fire a projectile, reload one bullet, reload all bullets, etc.)
// allows for plug and play functionality

public static class WeaponComponents
{
    public static void FireSingleHitscan(Transform origin, HealthTracker ownerHealth, int damage)
    {
        // Shoot a ray forward from firing origin
        if (Physics.Raycast(
            origin.position,
            origin.forward,
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

    public static void FireSprayHitscan(Transform origin, HealthTracker ownerHealth, int damage)
    {
        // fires multiple rays in a cone shape to simulate a spray effect
    }

    public static void FireProjectile(Transform origin, GameObject projectile)
    {
        // spawns a projectile to simulate projectile-based attacks
    }
}