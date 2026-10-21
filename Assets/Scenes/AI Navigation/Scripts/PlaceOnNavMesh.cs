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
        if(NavMesh.SamplePosition(gameObject.transform.position, out hit, 500f, NavMesh.AllAreas))
        {
            gameObject.transform.position = hit.position;
            agent.enabled = true;
        }
    }

}
