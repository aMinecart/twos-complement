using UnityEngine;

// eventually can be made abstract, allowing child classes to inherit all functionality from this class
// and only change stats values (at least for the basic test)
public class MeleeWeapon : Weapon
{
    // Called when player or anemy tries to use melee attack 
    public override bool Attack()
    {
        // Don't attack if game isn't playing or weapon on cooldown
        if (!CanAttack())
            return false;
        
        // Check if origin has been assigned 
        if (firingOriginTransform == null)
        {
            Debug.LogWarning("MeleeWeapon: Attack origin not assigned");
            return false;
        }

        nextAttackTime = Time.time + fireRate;
        Debug.Log("Melee attack!");

        // Find all objects hit by the attack within its range 
        RaycastHit[] hits = Physics.RaycastAll(
            firingOriginTransform.position,
            firingOriginTransform.forward,
            range
        );

        // Check hits in order, starting with the closest 
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            HealthTracker targetHealth = hit.collider.GetComponentInParent<HealthTracker>();

            // Ignore the character using the weapon 
            if (ownerHealth != null && targetHealth == ownerHealth)
                continue;

            // Damage first valid target
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damage);
                Debug.Log("Melee hit: " + hit.collider.name);
            } 

            // Stop at the first non-owner collider 
            // Prevents attacking through walls 
            break; 
        }

        return true; 
    }
}