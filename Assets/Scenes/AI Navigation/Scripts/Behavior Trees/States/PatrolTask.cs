using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using BehaviorTree;

public class PatrolTask : Node
{
    private Transform[] waypoints;
    private GameObject entity;
    private int currentWaypoint = 0;
    private float patrolSpeed = 2f;
    private float waitTime = 1f;
    private Coroutine prevCoroutine;

    private NavMeshAgent agent;


    public PatrolTask(GameObject entity, Transform[] waypoints)
    {
        this.entity = entity;
        agent = entity.GetComponent<NavMeshAgent>();
        this.waypoints = waypoints;
    }

    //Will constantly eval, as its always running, and tree is always evaluating from the root.
    public override NodeState Evaluate()
    {
        agent.destination = waypoints[currentWaypoint].transform.position;
        if(Vector3.Distance(waypoints[currentWaypoint].transform.position, entity.transform.position) < 1f)
        {
            currentWaypoint++;
            if(currentWaypoint == waypoints.Length) currentWaypoint = 0;
        }


        state = NodeState.RUNNING;
        return state;
    }

}

