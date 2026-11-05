using UnityEngine;
using BehaviorTree;

public class CanFireLaser : Node
{
    float laserTimer;
    float currentTime;

    public CanFireLaser(float laserTimer)
    {
        this.laserTimer = laserTimer;
        currentTime = this.laserTimer;
    }

    public override NodeState Evaluate()
    {
        currentTime -= Time.deltaTime;
        //Debug.Log("wall timer");
        if(currentTime <= 0) //Can fire
        {
            state = NodeState.SUCCESS;
            currentTime = laserTimer;
            return state;
        }

        //Cant fire
        state = NodeState.FAILURE;
        return state;
    }
}
