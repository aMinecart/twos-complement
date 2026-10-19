using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

//If the enemy can see the player, move towards the player.
public class TaskWalkTowards : Node
{
    private GameObject entity;
    private GameObject player;
    private NavMeshAgent agent;

    public TaskWalkTowards(GameObject entity, GameObject player)
    {
        this.entity = entity;
        this.player = player;
        agent = entity.GetComponent<NavMeshAgent>();
    }

    public override NodeState Evaluate()
    {
        agent.destination = player.transform.position;
        state = NodeState.RUNNING;
        return state;
    }
}
