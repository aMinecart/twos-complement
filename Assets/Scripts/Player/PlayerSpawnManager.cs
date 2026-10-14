using UnityEngine;

[DefaultExecutionOrder(-100)]
public class PlayerSpawnManager : MonoBehaviour
{
   [Header("Player Setup")]
   public Transform player; 
   public Transform spawnPoint; 

   [Header("Auto Save")]
   [Tooltip("Off = position is only saved by Save & Exit Button OR an Exit Point!")]
   public bool autoSave = true;
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
            Vector3 savedPosition = new Vector3(
                PlayerPrefs.GetFloat(XKey), 
                PlayerPrefs.GetFloat(YKey), 
                PlayerPrefs.GetFloat(ZKey)
            );
            MovePlayer(savedPosition, player.rotation);
            Debug.Log("Player has spawned at the last saved position: "+ savedPosition);
        } else {
            MovePlayer(spawnPoint.position, spawnPoint.rotation);
            Debug.Log("Player has spawned at the starting point!");
        }
        ready = true; 
    }

    private void MovePlayer(Vector3 position, Quaternion rotation)
    {
        player.SetPositionAndRotation(position, rotation); 
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if(rb !=null && !rb.isKinematic)
        {
            rb.linearVelocity = Vector3.zero; 
            rb.angularVelocity = Vector3.zero;
        }

        Physics.SyncTransforms(); //so exitpoint doesn't overlap when re-starting game
    }
    // Update is called once per frame
    void Update()
    {
        if(!ready || autoSave){
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

        PlayerPrefs.SetInt(SavedKey, 1);
        PlayerPrefs.SetFloat(XKey, player.position.x); 
        PlayerPrefs.SetFloat(YKey, player.position.y); 
        PlayerPrefs.SetFloat(ZKey, player.position.z); 
        PlayerPrefs.Save(); 
        Debug.Log("Player position has been saved: " + player.position); 
    }
    public void ClearSavedPosition()
    {
        PlayerPrefs.DeleteKey(SavedKey);
        PlayerPrefs.DeleteKey(XKey);
        PlayerPrefs.DeleteKey(YKey);
        PlayerPrefs.DeleteKey(ZKey);
        PlayerPrefs.Save();
    }
    private void OnApplicationQuit(){
        if(ready && autoSave){
            SavePosition();
        }
    }

    private void OnApplicationPause(bool paused){
        if(ready && paused && autoSave){
            SavePosition();
        }
    }
}