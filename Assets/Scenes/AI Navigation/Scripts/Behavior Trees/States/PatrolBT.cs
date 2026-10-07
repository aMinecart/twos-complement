using BehaviorTree;
using UnityEngine;

public class PatrolBT : BehaviorTree.Tree
{

    public UnityEngine.Transform[] waypoints;

    public float speed = 2f;

    protected override Node SetupTree()
    {
        Node root = new PatrolTask(gameObject, waypoints);
        return root;
    }
}
