using UnityEngine;

public class EnemyAttackerTester : MonoBehaviour
{
    [SerializeField] private Weapon weapon;
    [SerializeField] private HealthTracker enemyHealth;
    [SerializeField] private Transform attackOrigin;
    [SerializeField] private Transform target;
    [SerializeField] private float detectionRange = 2f;

    void Start()
    {
        // Set up the enemy's weapon
        if (weapon != null)
            weapon.Initialize(enemyHealth, attackOrigin);
    }

    void Update()
    {
        if (weapon == null || target == null || attackOrigin == null)
            return;

        // Only attack when the player is close enough
        if (Vector3.Distance(attackOrigin.position, target.position) > detectionRange)
            return;

        // Face the player before attacking
        Vector3 direction = target.position - attackOrigin.position;

        if (direction.sqrMagnitude > 0.001f)
            attackOrigin.rotation = Quaternion.LookRotation(direction);

        // Request an attack; the weapon handles cooldown and damage
        weapon.Attack();
    }
}
