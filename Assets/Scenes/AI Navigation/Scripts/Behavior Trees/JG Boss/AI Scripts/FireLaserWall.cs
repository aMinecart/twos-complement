using UnityEngine;
using BehaviorTree;

public class FireLaserWall : Node
{
    GameObject wallSpawn;
    GameObject laserWall;

    public FireLaserWall(GameObject wallSpawn, GameObject laserWall)
    {
        this.wallSpawn = wallSpawn;
        this.laserWall = laserWall;
    }

    public override NodeState Evaluate()
    {
        Object.Instantiate(laserWall, wallSpawn.transform.position, wallSpawn.transform.rotation);
        state = NodeState.SUCCESS;
        return state;
    }
}
