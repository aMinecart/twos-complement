using UnityEngine;
using BehaviorTree;

public class CheckInRange : Node
{
    private GameObject target;
    private GameObject enemy;
    private float attackRange;

    public CheckInRange(GameObject target, GameObject enemy, float attackRange)
    {
        this.target = target;
        this.enemy = enemy;
        this.attackRange = attackRange;
    }

    public override NodeState Evaluate()
    {
        if(Vector3.Distance(target.transform.position, enemy.transform.position) < attackRange)
        {
            state = NodeState.SUCCESS;
            return NodeState.SUCCESS;
        }
        state = NodeState.FAILURE;
        return state;
    }

}
