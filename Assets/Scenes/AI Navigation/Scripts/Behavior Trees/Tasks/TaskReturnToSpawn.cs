using UnityEngine;
using BehaviorTree;
using UnityEngine.AI;

//If player is not visible for RETURN_TIME, return to spawn
public class TaskReturnToSpawn : Node
{
    private Transform[] spawn;
    private static float RETURN_TIME = 15f;
    private float returnTimer = RETURN_TIME;
    private GameObject entity;
    private NavMeshAgent agent;

    public TaskReturnToSpawn(Transform[] spawn, GameObject entity)
    {
        this.spawn = spawn;
        agent = entity.GetComponent<NavMeshAgent>();
        this.entity = entity;
    }

    public override NodeState Evaluate()
    {
        returnTimer -= Time.deltaTime;
        if(returnTimer < 0)
        {
            agent.destination = spawn[0].position;
            returnTimer = RETURN_TIME;
            state = NodeState.SUCCESS;
            return state;
        }
        state = NodeState.RUNNING;
        return state;
    }
}
