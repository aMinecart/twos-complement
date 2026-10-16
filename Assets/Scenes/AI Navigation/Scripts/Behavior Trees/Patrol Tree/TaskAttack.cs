using UnityEngine;
using BehaviorTree;

// TODO:
// - Support selecting a specific weapon when an enemy has multiple weapons
// - Improve aiming for enemies and players at different elevations if needed
public class TaskAttack : Node
{
    private GameObject enemy;
    private GameObject target;
    private Weapon weapon;

    public TaskAttack(GameObject enemy, GameObject target)
    {
        this.enemy = enemy;
        this.target = target;

        // Find enemy's weapon
        weapon = enemy.GetComponentInChildren<Weapon>();

        if (weapon != null)
        {
            // Use enemy's health to prevent self damage
            HealthTracker enemyHealth = enemy.GetComponent<HealthTracker>();

            Transform attackOrigin = enemy.transform.Find("attackOrigin");
            if (attackOrigin == null)
            {
                Debug.LogWarning(enemy.name + " has no attackOrigin. Using enemy transform.");
                attackOrigin = enemy.transform;
            }

            weapon.Initialize(enemyHealth, attackOrigin);
        }
        else
        {
            Debug.LogWarning(enemy.name + " does not have a Weapon component");
        }
    }

    public override NodeState Evaluate()
    {
        if (weapon != null && target != null)
        {
            // Aim toward player without rotating the whole enemy
            Vector3 direction = target.transform.position - enemy.transform.position;
            direction.y = 0f;
            if(direction.sqrMagnitude > 0.001f)
            {
                enemy.transform.rotation = Quaternion.LookRotation(direction);
            }

            // Automatically reload enemy ranged weapons when empty
            if (weapon is RangedWeapon rangedWeapon && rangedWeapon.CurrentAmmo <= 0)
            {
                rangedWeapon.Reload();
            }
            else
            {
                // Handles cooldowns, hits, and damage
                weapon.Attack();
            }
        }

        state = NodeState.RUNNING;
        return state;
    }

}
