using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))] //Creates NavMeshAgent if DNE
public class EnemyAI : MonoBehaviour
{
    public GameObject target;
    public float speed = 10f;
    private NavMeshAgent agent;

    bool isInAngle = false, isInRange = false, isNotHidden = false;
    public float visionRange = 100f;
    public float detectionAngle = 45f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
    }

    // Update is called once per frame
    void Update()
    {

        //Moves enemy toward player if seen
        if(CanTrackPlayer()) agent.destination = target.transform.position;

    }

    //Determines if the enemy can see the player
    bool CanTrackPlayer()
    {

        //Determines if the player is in range
        if(Vector3.Distance(target.transform.position, transform.position) < visionRange)
        {
            isInRange = true;
        }
        else
        {
            isInRange = false;
        }

        RaycastHit hit; //Info on object hit
        //Determines if the player is behind cover
        if(Physics.Raycast(transform.position, target.transform.position, out hit, visionRange))
        {
            if(hit.transform == target.transform)
            {
                isNotHidden = true;
            }
        }
        else
        {
            isNotHidden = false;
        }

        //Determines if the player is within the enemy's vision cone
        Vector3 playerAngle = target.transform.position - transform.position;
        Vector3 enemyAngle = transform.forward;
        float angle = Vector3.SignedAngle(playerAngle, enemyAngle, Vector3.up);
        if(angle <= detectionAngle) isInAngle = true;

        if(isNotHidden && isInRange && isInAngle) return true;
        return false;
    }




}
