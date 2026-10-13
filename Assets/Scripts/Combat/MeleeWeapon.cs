using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    // Weapon stats
    [SerializeField] private float damage = 35f;
    [SerializeField] private float range = 2f;
    [SerializeField] private float attackCooldown = 0.7f;

    // Attack setup
    // Where attack starts and its direction
    [SerializeField] private Transform attackOrigin;

    // Character using this weapon 
    [SerializeField] private HealthTracker ownerHealth;

    private float nextAttackTime = 0f;

    public void Attack()
    {
        // Don't attack when game is over or not in Playing state 
        if (GameManager.instance != null && GameManager.instance.CurrentState != GameManager.GameState.Playing)
            return;
        
        if (Time.time < nextAttackTime)
            return;
        
        // Check if origin has been assigned 
        if (attackOrigin == null)
        {
            Debug.LogWarning("MeleeWeapon: Attack origin not assigned");
            return;
        }

        nextAttackTime = Time.time + attackCooldown;
        Debug.Log("Melee attack!");

        // Find all objects hit by the attack within its range 
        RaycastHit[] hits = Physics.RaycastAll(
            attackOrigin.position,
            attackOrigin.forward,
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

            // Damage first valud target
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damage);
                Debug.Log("Melee hit: " + hit.collider.name);
            } 

            // Stop at the first non-owner collider 
            break; 
        }
        
    }
}
