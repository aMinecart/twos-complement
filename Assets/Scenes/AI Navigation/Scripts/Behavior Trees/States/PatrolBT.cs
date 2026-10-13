using BehaviorTree;
using UnityEngine;
using System.Collections.Generic;

public class PatrolBT : BehaviorTree.Tree
{
    public GameObject player;
    public UnityEngine.Transform[] waypoints;
    public float visionRange;
    public float attackRange;
    public float visionAngle = 45f;

    public float speed = 2f;

    protected override Node SetupTree()
    {

        Node root = new Selector(new List<Node>
        {

            new Sequence(new List<Node>
            {
                new CheckInRange(player, gameObject, attackRange),
                new TaskAttack(gameObject, player),
            }),

            new Sequence(new List<Node>
            {
                new CheckCanSee(player, gameObject, visionRange, visionAngle),
                new TaskWalkTowards(gameObject, player),
            }),
            new PatrolTask(gameObject, waypoints),
        });

        //Node root = new PatrolTask(gameObject, waypoints);
        return root;
    }
}
