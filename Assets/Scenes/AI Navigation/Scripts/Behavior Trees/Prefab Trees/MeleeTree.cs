using BehaviorTree;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(PlaceOnNavMesh))]
public class MeleeTree : TreeSetup
{
    private GameObject player;
    public UnityEngine.Transform[] spawn;
    public float visionRange;
    public float attackRange;
    public float visionAngle = 45f;
    public float speed = 2f;

    protected override Node SetupTree()
    {
        player = PlayerReference.Instance.gameObject;
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

            new TaskReturnToSpawn(spawn, gameObject),
        });

        return root;
    }
}
