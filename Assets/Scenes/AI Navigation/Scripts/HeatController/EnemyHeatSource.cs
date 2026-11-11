using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [Header("Player Reference")]
    [SerializeField] private Transform player; 

    [Header("Heat Settings")]
    [SerializeField] private float detectionRadius = 8f; 
    [SerializeField] private float heatPerSecond = 5f; 


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        if(player == null){
            GameObject playerObject = PlayerReference.Instance.gameObject;
            if(playerObject != null)
            {
                player = playerObject.transform; 
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if(player == null || HeatController.Instance == null)
        {
            return; 
        }

        float distance = Vector3.Distance(player.position, transform.position); 

        if(distance <= detectionRadius)
        {
            HeatController.Instance.AddHeat(heatPerSecond * Time.deltaTime); 
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red; 
        Gizmos.DrawWireSphere(transform.position, detectionRadius); 
    }

    /*public void EnemyDefeated()
    {
        
    }*/


    /*
    TODO: when player fires gun increase heat a little etc
    */

}

