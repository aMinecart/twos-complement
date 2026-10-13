using UnityEngine;
using BehaviorTree;
using System;

//If the player is in range, in fov, and is visible, set state to success.
//It will then move onto the next node in the sequencer node, like Attack
public class CheckCanSee : Node
{
    private GameObject target;
    private GameObject enemy;
    private float visionRange;
    private Boolean isNotHidden;

    public CheckCanSee(GameObject target, GameObject enemy, float visionRange)
    {
        this.target = target;
        this.enemy = enemy;
        this.visionRange = visionRange;
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
                state = NodeState.SUCCESS;
                return state;
            }
            else
            {
                //Debug.Log("Hidden");
                isNotHidden = false;
                state = NodeState.FAILURE;
                return state;
            }
        }
        state = NodeState.FAILURE;
        return state;



    }
}
