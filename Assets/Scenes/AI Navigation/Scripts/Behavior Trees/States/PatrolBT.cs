using BehaviorTree;

public class PatrolBT : BehaviorTree.Tree
{

    public UnityEngine.Transform[] waypoints;
    public Patrol patrolScript;

    public float speed = 2f;

    protected override Node SetupTree()
    {
        Node root = new PatrolTask();
        return root;
    }
}
