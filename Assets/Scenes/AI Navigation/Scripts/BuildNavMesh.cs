using UnityEngine;
using Unity.AI.Navigation;

[RequireComponent(typeof(NavMeshSurface))] //Adds component to GameObject
public class BuildNavMesh : MonoBehaviour
{
    
    private NavMeshSurface navMesh;
    public GameObject[] characters;
    public Transform[] SpawnPoints;

    void Awake()
    {
        navMesh = GetComponent<NavMeshSurface>(); //Get NavMesh component for baking
        navMesh.BuildNavMesh(); //Builds NavMesh on runtime
    }

    void OnEnable()
    {
        //navMesh.BuildNavMesh(); //Builds NavMesh on runtime
    }
}
