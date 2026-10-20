using UnityEngine;
using BehaviorTree;
using Mono.Cecil;

//If too close to target, RUNNNN
public class CheckRangedTooClose : Node
{
    GameObject target;
    GameObject enemy;
    float runDistance;

    public CheckRangedTooClose(GameObject target, GameObject enemy, float runDistance)
    {
        this.target = target;
        this.enemy = enemy;
        this.runDistance = runDistance;
    }

    public override NodeState Evaluate()
    {
        if(Vector3.Distance(target.transform.position, enemy.transform.position) < runDistance)
        {
            state = NodeState.SUCCESS;
            return state;
        }
        else
        {
            state = NodeState.FAILURE;
            return state;
        }
    }
}
