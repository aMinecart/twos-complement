using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
   [Header("Player Setup")]
   public Transform player; 
   public Transform spawnPoint; 

   [Header("Auto Save")]
   public float autoSaveInterval = 2f;
   private float saveTimer = 0f; 
   private bool ready = false; 

   private const string SavedKey = "HasSavedPlayerPosition"; 
   private const string XKey = "PlayerPositionX"; 
   private const string YKey = "PlayerPositionY"; 
   private const string ZKey = "PlayerPositionZ"; 


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        if(spawnPoint == null || player == null){
            Debug.LogWarning("Assign the player & spawn point!");
            return;
        }

        if(PlayerPrefs.GetInt(SavedKey, 0)== 1){
            Vector3 savedPosition = savedPosition = new Vector3(
                PlayerPrefs.GetFloat(XKey), 
                PlayerPrefs.GetFloat(YKey), 
                PlayerPrefs.GetFloat(ZKey)
            );
        } else {
            player.position = spawnPoint.position; 
            player.rotation = spawnPoint.rotation; 
            Debug.Log("Player has spawned at the starting point!");
        }
        ready = true; 
    }

    // Update is called once per frame
    void Update()
    {
        if(!ready){
            return;
        }
        saveTimer += Time.deltaTime; 
        if(saveTimer >= autoSaveInterval){
            SavePosition(); 
            saveTimer = 0f; 
        }
    }

    //saves players position
    public void SavePosition(){
        if(player == null){
            return;
        }

        PlayerPrefs.SetFloat(XKey, player.position.x); 
        PlayerPrefs.SetFloat(YKey, player.position.y); 
        PlayerPrefs.SetFloat(ZKey, player.position.z); 
        PlayerPrefs.Save(); 
        Debug.Log("Player position has been saved: " + player.position); 
    }
    private void onApplicationQuit(){
        if(ready){
            SavePosition();
        }
    }

    private void onApplicationPause(bool paused){
        if(ready && paused){
            SavePosition();
        }
    }
}