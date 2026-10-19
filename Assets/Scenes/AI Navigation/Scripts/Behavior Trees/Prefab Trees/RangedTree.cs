using BehaviorTree;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(PlaceOnNavMesh))]
public class RangedTree : TreeSetup
{
    private GameObject player;
    public UnityEngine.Transform[] spawn;
    public float visionRange = 35f;
    public float attackRange = 15f;
    public float visionAngle = 45f;
    public float runDistance = 15f;
    public float speed = 2f;

    protected override Node SetupTree()
    {
        player = PlayerReference.Instance.gameObject;
        Node root = new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new CheckRangedTooClose(player, gameObject, runDistance),
                new TaskRunAway(player, gameObject, runDistance),
            }),

            new Sequence(new List<Node>
            {
                new CheckCanShoot(player, gameObject, attackRange),
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
