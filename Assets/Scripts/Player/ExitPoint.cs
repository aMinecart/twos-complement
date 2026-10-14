using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class ExitPoint : MonoBehaviour
{
    public PlayerSpawnManager spawnManager; 
    private Collider exitCollider;
    private bool hasLeftExit = false; 
    private bool exitTriggered = false; 
    
    private void Awake()
    {
        exitCollider = GetComponent<Collider>();
    }

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player"); 
        Collider playerCollider = 
            player != null ? player.GetComponent<Collider>():null;

        //if player enters back the game on an exit point ->  ignore it until the player goes off it and then comes back on it
        hasLeftExit = playerCollider == null || exitCollider == null || !exitCollider.bounds.Intersects(playerCollider.bounds);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hasLeftExit = true; 
            Debug.Log("Player has left the exit area!"); 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Exit trigger deteected: " + other.name);
        if(!other.CompareTag("Player")|| !hasLeftExit || exitTriggered)
        {
            return; 
        }
        exitTriggered = true; 
        Debug.Log("Player has reached the exit!"); 
        if(spawnManager == null)
        {
            Debug.LogError("Spawn Manager isn't assigned!"); 
            exitTriggered = false; 
            return; 
        }
        spawnManager.SavePosition();
        Debug.Log("Position saved! Exiting the game :)"); 

        Application.Quit();

        //just for me when testing so the game stops testing inside the unity editor!
        #if UNITY_EDITOR
        EditorApplication.isPlaying = false; 
        #endif
    }
}