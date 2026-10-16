using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

// TODO:
// - Replace position only saving with RunSaveManager once fully integrated
public class GameMenuManager : MonoBehaviour
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
      if(pauseMenuPanel != null)
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
        if (GameManager.instance == null)
        {
            Debug.LogWarning("GameManager not found!");
            return;
        }

        if(paused == IsPaused)
        {
            return;
        }

        if (paused)
        {
            // only allow pausing while playing
            if (GameManager.instance.CurrentState != GameManager.GameState.Playing)
                return;

            previousLockState = Cursor.lockState;
            previousCursorVisible = Cursor.visible;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            GameManager.instance.PauseGame();
        }
        else
        {
            // only resume when paused
            if (GameManager.instance.CurrentState != GameManager.GameState.Paused)
                return;

            Cursor.lockState = previousLockState;
            Cursor.visible = previousCursorVisible;

            GameManager.instance.ResumeGame();
        }

        IsPaused = paused;
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(paused);
    }

    private void QuitGame()
    {
        if (GameManager.instance != null && GameManager.instance.CurrentState == GameManager.GameState.Paused)
        {
            GameManager.instance.ResumeGame();
        }

        Application.Quit();
    #if UNITY_EDITOR
        EditorApplication.isPlaying = false;
    #endif
    }

    private void OnDestroy()
    {
        if (IsPaused && GameManager.instance != null && GameManager.instance.CurrentState == GameManager.GameState.Paused)
        {
            GameManager.instance.ResumeGame();
        }

        IsPaused = false;
    }
}
