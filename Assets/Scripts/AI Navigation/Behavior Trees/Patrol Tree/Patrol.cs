using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Patrol : MonoBehaviour
{
    public Transform[] waypoints;
    private int currentWaypoint = 1;
    private float patrolSpeed = 2f;
    private float waitTime = 1f;
    private Coroutine prevCoroutine;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = patrolSpeed;
        agent.autoBraking = false; //Doesn't slow down as it approaches
        prevCoroutine = StartCoroutine(PatrolPath());
    }

    private IEnumerator PatrolPath()
    {
        agent.destination = waypoints[currentWaypoint].position;
        while(Vector3.Distance(transform.position, waypoints[currentWaypoint].position) > 1f)
        {
            yield return null;
        }
        yield return new WaitForSeconds(waitTime);
        currentWaypoint++;
        if(currentWaypoint == waypoints.Length) currentWaypoint = 0;
        
        StopCoroutine(prevCoroutine);
        prevCoroutine = StartCoroutine(PatrolPath());
    }

}

