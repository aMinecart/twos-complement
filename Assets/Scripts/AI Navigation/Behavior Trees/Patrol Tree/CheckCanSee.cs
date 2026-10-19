using UnityEngine;
using BehaviorTree;
using System;

//If the player is in range, in fov, and is visible, set state to success.
//It will then move onto the next node in the sequencer node, like Attack
public class CheckCanSee : Node
{
    private GameObject target;
    private GameObject enemy;
    private float visionAngle;
    private float visionRange;
    private Boolean isNotHidden = false;
    private Boolean isInAngle = false;

    public CheckCanSee(GameObject target, GameObject enemy, float visionRange, float visionAngle)
    {
        this.target = target;
        this.enemy = enemy;
        this.visionRange = visionRange;
        this.visionAngle = visionAngle;
    }

    public override NodeState Evaluate()
    {
        RaycastHit hit; //Info on object hit
        //Determines if the player is behind cover
        if(Physics.Raycast(enemy.transform.position, (target.transform.position - enemy.transform.position), out hit, visionRange))
        {
            //Debug.Log("here");
            if(hit.transform == target.transform)
            {
                //Debug.Log("Is not hidden");
                isNotHidden = true;
            }
            else
            {
                state = NodeState.FAILURE;
                return state;
            }
        }

        //Determines if the player is within the enemy's FOV
        Vector3 playerAngle = target.transform.position - enemy.transform.position;
        Vector3 enemyAngle = enemy.transform.forward;
        float angle = Vector3.SignedAngle(playerAngle, enemyAngle, Vector3.up);
        if(angle <= visionAngle && angle >= (-1 * visionAngle))
        {
            isInAngle = true;
        }
        else isInAngle = false;

        //If not hidden and in angle, walk towards target
        if(isNotHidden && isInAngle)
        {
            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}
