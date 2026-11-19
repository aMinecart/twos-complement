using UnityEngine;
using BehaviorTree;
using Unity.VisualScripting;

public class CanUseMist : Node
{
    GameObject player;
    GameObject boss;
    float meleeDistance = 5f;

    public CanUseMist(GameObject player, GameObject boss)
    {
        this.player = player;
        this.boss = boss;
    }

    public override NodeState Evaluate()
    {
        if(Vector3.Distance(player.transform.position, boss.transform.position) < meleeDistance)
        {
            state = NodeState.SUCCESS;
            return state;
        }
        state = NodeState.FAILURE;
        return state;
    }
}
