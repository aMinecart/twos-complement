using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;

//Places enemy on the navmesh so it runs properly.
public class PlaceOnNavMesh : MonoBehaviour
{
    void Start()
    {
        NavMeshHit hit;
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if(NavMesh.SamplePosition(gameObject.transform.position, out hit, 12f, NavMesh.AllAreas))
        {
            gameObject.transform.position = hit.position; // + new Vector3(0f, 0.5f, 0f);
            agent.enabled = true;
            agent.Warp(hit.position);
        }
    }

}
