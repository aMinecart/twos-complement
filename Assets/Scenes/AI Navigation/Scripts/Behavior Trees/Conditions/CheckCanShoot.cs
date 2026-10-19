using UnityEngine;
using BehaviorTree;

public class CheckCanShoot : Node
{
    private GameObject target;
    private GameObject enemy;
    private float attackRange;

    public CheckCanShoot(GameObject target, GameObject enemy, float attackRange)
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
            //Debug.Log(Vector3.Distance(target.transform.position, enemy.transform.position));
            return NodeState.SUCCESS;
        }
        state = NodeState.FAILURE;
        return state;
    }

}
