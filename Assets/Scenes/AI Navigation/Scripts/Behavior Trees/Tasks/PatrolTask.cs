using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using BehaviorTree;
using UnityEngine.Rendering;
using UnityEditor.PackageManager;

public class PatrolTask : Node
{
    private GameObject entity;
    private NavMeshAgent agent;
    private Vector3 destination;
    private float patrolDistance = 50f;


    public PatrolTask(GameObject entity)
    {
        this.entity = entity;
        agent = entity.GetComponent<NavMeshAgent>();
    }

    //Will constantly eval, as its always running, and tree is always evaluating from the root.
    public override NodeState Evaluate()
    {
        if(agent.remainingDistance < 5f)
        {
            FindDestination();
            agent.destination = destination;
        }

        state = NodeState.RUNNING;
        return state;
    }

    void FindDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolDistance;
        randomDirection = entity.transform.position + randomDirection;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, 25f, 1);
        destination = hit.position;
    }

}

