using UnityEngine;

[DefaultExecutionOrder(-100)]
public class PlayerSpawnManager : MonoBehaviour
{
   [Header("Player Setup")]
   public Transform player;
   public Transform spawnPoint;

   [Header("Procedural Levels")]
   public bool waitForGeneratedSpawn = true;
   public bool useSeedCheck = true; 
   
   [Header("Auto Save")]
   [Tooltip("Off = position is only saved by Save & Exit Button OR an Exit Point!")]
    public bool autoSave = true;
    public float autoSaveInterval = 2f;

   [Header("Fall Respawn")]
   public float fallY = -20f;
   
   private float saveTimer = 0f;
   private bool ready = false;
   private Rigidbody rb;

   private const string SavedKey = "HasSavedPlayerPosition";
   private const string XKey = "PlayerPositionX";
   private const string YKey = "PlayerPositionY";
   private const string ZKey = "PlayerPositionZ";
   private const string SeedKey = "PlayerPositionSeed";


    private void Awake()
    {
        if(player == null)
        {
            return; 
        }
        rb = player.GetComponent<Rigidbody>();

        if(waitForGeneratedSpawn && rb != null)
        {
            rb.isKinematic = true;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        //non-procedural scenes- will spawn right away 
        if (!waitForGeneratedSpawn)
        {
            BeginSpawn();
        }
    }

    public void BeginSpawn()
    {
        //called by PlayerSpawnPoint.SetSpawn once the spawnPoint's position is finalized 
        if(spawnPoint == null || player == null)
        {
            Debug.LogWarning("Assign the player & spawn point!"); 
            return;
        }

        //unfreeze before moving so move player can zero the vel!
        if(rb != null)
        {
            rb.isKinematic = false; 
        }

        if (HasValidSavedPosition())
        {
            Vector3 savedPosition = new Vector3(PlayerPrefs.GetFloat(XKey), PlayerPrefs.GetFloat(YKey), PlayerPrefs.GetFloat(ZKey)); 
            MovePlayer(savedPosition, player.rotation); 
            Debug.Log("Player has spawned at the last saved position: " + savedPosition);
        }
        else
        {
           MovePlayer(spawnPoint.position, spawnPoint.rotation);
           Debug.Log("Player has spawned at the starting point!"); 
        }
        saveTimer = 0f; 
        ready = true; 
    }
    
    private bool HasValidSavedPosition()
    {
        if (PlayerPrefs.GetInt(SavedKey, 0) != 1) return false;
 
        if (useSeedCheck)
        {
            //saved position belongs to a different generated world -> ignore it!!!
            if (!PlayerPrefs.HasKey(SeedKey) || PlayerPrefs.GetInt(SeedKey) != GameManager.runSeed)
            {
                return false;
            }
        }
        return true;
    }

    private void MovePlayer(Vector3 position, Quaternion rotation)
    {
        player.SetPositionAndRotation(position, rotation);
        if(rb !=null && !rb.isKinematic)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        //Debug.Log("Moved");
        Physics.SyncTransforms(); //so exitpoint doesn't overlap when re-starting game
    }

    public void RespawnAtSpawnPoint()
    {
        if(player == null || spawnPoint == null)
        {
            return;
        }
        MovePlayer(spawnPoint.position, spawnPoint.rotation);
        Debug.Log("Player fell off the board and respawned at the spawn point!");
    }
    // Update is called once per frame
    void Update()
    {
        if(!ready){
            return;
        }
        //player fell off "world" (just for testing for now purposes will probs comment out later)
        if(player != null && player.position.y < fallY)
        {
            RespawnAtSpawnPoint();
            return;
        }

        if (autoSave)
        {
            saveTimer += Time.deltaTime;
            if(saveTimer >= autoSaveInterval)
            {
                SavePosition();
                saveTimer = 0f;
            }
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

        if (useSeedCheck)
        {
            PlayerPrefs.SetInt(SeedKey, GameManager.runSeed);
        }

        PlayerPrefs.Save();
        Debug.Log("Player position has been saved: " + player.position);
    }
    public void ClearSavedPosition()
    {
        PlayerPrefs.DeleteKey(SavedKey);
        PlayerPrefs.DeleteKey(XKey);
        PlayerPrefs.DeleteKey(YKey);
        PlayerPrefs.DeleteKey(ZKey);
        PlayerPrefs.DeleteKey(SeedKey);
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