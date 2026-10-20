using UnityEngine;
using BehaviorTree;
using UnityEngine.AI;

public class TaskRunAway : Node
{
    private GameObject target;
    private GameObject enemy;
    private float runDistance;
    public TaskRunAway(GameObject target, GameObject enemy, float runDistance)
    {
        this.target = target;
        this.enemy = enemy;
        this.runDistance = runDistance + 1;
    }

    public override NodeState Evaluate()
    {
        //run away
        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
        Vector3 targetDirection = (enemy.transform.position - target.transform.position).normalized;
        agent.destination = target.transform.position + (targetDirection * runDistance);
        state = NodeState.RUNNING;
        return state;
    }
}
