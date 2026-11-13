using UnityEngine;
using BehaviorTree;

public class FireRockets : Node
{
    GameObject[] rocketSpawns;
    GameObject rocket;
    float rocketFireRate;
    float timeSinceAttack;
    public FireRockets(GameObject[] rocketSpawns, GameObject rocket, float rocketFireRate)
    {
        this.rocketSpawns = rocketSpawns;
        this.rocketFireRate = rocketFireRate;
        this.rocket = rocket;
    }

    public override NodeState Evaluate()
    {
        timeSinceAttack -= Time.deltaTime;
        if(timeSinceAttack <= 0)
        {
            Object.Instantiate(rocket, rocketSpawns[0].transform.position, rocketSpawns[0].transform.rotation);
            timeSinceAttack = rocketFireRate;
        }
        return NodeState.RUNNING;
    }

}