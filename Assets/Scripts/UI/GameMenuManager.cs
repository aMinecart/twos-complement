using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private PlayerSpawnManager spawnManager; 

    public static bool IsPaused {get; private set; }

    private CursorLockMode previousLockState; 
    private bool previousCursorVisible; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
      IsPaused = false; 
      Time.timeScale = 1f; 
      pauseMenuPanel.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        if(Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            SetPaused(!IsPaused);
        }
    }

    public void OpenMenu() => SetPaused(true);
    public void Resume() => SetPaused(false);

    public void SaveAndQuit()
    {
        if(spawnManager == null)
        {
            Debug.LogError("Spawn Manager isn't assigned!");
            return;
        }
        spawnManager.SavePosition();
        QuitGame();
    }
    
    private void SetPaused(bool paused)
    {
        if(paused == IsPaused)
        {
            return;
        }
        if (paused)
        {
            previousLockState = Cursor.lockState;
            previousCursorVisible = Cursor.visible;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = previousLockState;
            Cursor.visible = previousCursorVisible;
        }
        IsPaused = paused; 
        Time.timeScale = paused ? 0f : 1f; 
        pauseMenuPanel.SetActive(paused);
    }

    private void QuitGame()
    {
        Time.timeScale = 1f; 
        Application.Quit(); 
        #if UNITY_EDITOR
        EditorApplication.isPlaying = false;
        #endif
    }

    private void OnDestroy()
    {
        IsPaused = false; 
        Time.timeScale = 1f; 
    }
}
