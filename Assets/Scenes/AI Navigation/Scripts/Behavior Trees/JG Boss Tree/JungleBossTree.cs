using UnityEngine;
using BehaviorTree;
using System.Collections.Generic;

[RequireComponent(typeof(PlaceOnNavMesh))]
public class JungleBossTree : TreeSetup
{
    private GameObject player;
    public float visionRange;
    public float rocketMinRange;
    public float rocketFireRate;
    public float laserWallTimer;
    public GameObject[] rocketSpawns;
    public GameObject rocket;
    public GameObject laserWallSpawn;
    public GameObject laserWall;

    public float speed = 2f;

    protected override Node SetupTree()
    {
        player = PlayerReference.Instance.gameObject;
        
        //If player is in range, perform melee
        //Else, if certain distance, perform missile attack
        //Else, walk towards player and perform laser layer attack every so often
        Node root = new Selector(new List<Node>
        {
            // //In range? Melee
            // new Sequence(new List<Node>
            // {

            // }),
            //Far away? ROCKET
            new Sequence(new List<Node>
            {
                new InRocketDistance(rocketMinRange, gameObject),
                new FireRockets(rocketSpawns, rocket, rocketFireRate),
                new TaskWalkTowards(gameObject, player),
            }),

            // //Laser attack timer gone? BOOM
            new Sequence(new List<Node>
            {
                new CanFireLaser(laserWallTimer),
                new FireLaserWall(laserWallSpawn, laserWall),
            }),
            
            //Walk towards player
            new TaskWalkTowards(gameObject, player),
        });
        
        return root;
    }
}
