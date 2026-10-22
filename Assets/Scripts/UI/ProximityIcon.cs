using UnityEngine;

public class ProximityIcon : MonoBehaviour
{
    private Transform player;

    [Header("Warning Icon")]
    public GameObject warningIcon; 

    [Header("Detection Settings")]
    public float warningDistance = 10f; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = PlayerReference.Instance.transform; 
        if(warningIcon != null){
            warningIcon.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //makes sure that the player and warning icon are assigned 
        if(player == null || warningIcon == null){
            return; 
        }

        //calculates the distance between the player and the enemy 
        float distance = Vector3.Distance(player.position,transform.position); 

        //shows the warning when the player gets close 
        if(distance <= warningDistance){
            warningIcon.SetActive(true);
        } else{
            warningIcon.SetActive(false);
        }
    }
}
