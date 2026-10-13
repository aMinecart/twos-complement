using UnityEngine;
using BehaviorTree;
using System.Threading.Tasks;

public class TaskAttack : Node
{
    private GameObject target;
    private GameObject enemy;
    private float attackTime = 2f;

    public TaskAttack(GameObject target, GameObject enemy)
    {
        this.target = target;
        this.enemy = enemy;
    }

    public override NodeState Evaluate()
    {
        if(attackTime < 0)
        {
            attackTime = 2f;
            Debug.Log("Attacking");
        }
        attackTime -= Time.deltaTime;
        state = NodeState.RUNNING;
        return state;
    }

}
