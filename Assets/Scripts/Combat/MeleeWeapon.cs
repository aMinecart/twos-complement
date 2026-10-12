using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    // Weapon stats
    [SerializeField] private float damage = 35f;
    [SerializeField] private float range = 2f;
    [SerializeField] private float attackCooldown = 0.7f;

    // Attack setup
    // Where attack starts and which layers it can hit
    [SerializeField] private Transform attackOrigin;

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

        // Cast ray forward to check if it hits smth within range
        if (Physics.Raycast(
            attackOrigin.position,
            attackOrigin.forward,
            out RaycastHit hit,
            range))
        {
            Health targetHealth = hit.collider.GetComponentInParent<Health>();
            Health ownerHealth = GetComponentInParent<Health>();

            // Apply damage only if target has health and is not the attacker
            if (targetHealth != null && targetHealth != ownerHealth)
            {
                targetHealth.TakeDamage(damage);
                Debug.Log("Melee hit: " + hit.collider.name);
            }
        }
    }
}
