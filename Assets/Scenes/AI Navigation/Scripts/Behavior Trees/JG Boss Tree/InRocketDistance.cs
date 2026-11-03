using UnityEngine;
using BehaviorTree;

public class InRocketDistance : Node
{
    float minRocketRange;
    GameObject boss;
    public InRocketDistance(float minRocketRange, GameObject boss)
    {
        this.minRocketRange = minRocketRange;
        this.boss = boss;
    }

    public override NodeState Evaluate()
    {
        //If in rocket range, success
        if(Vector3.Distance(PlayerReference.Instance.transform.position, boss.transform.position) < minRocketRange)
        {
            state = NodeState.SUCCESS;
            return state;
        }
        state = NodeState.FAILURE;
        return state;
    }
}
