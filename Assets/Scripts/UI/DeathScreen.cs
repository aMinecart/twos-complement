using UnityEngine;
using System.Collections; 
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    [Header("Groups")]
    [SerializeField] private CanvasGroup background; //death panel 
    [SerializeField] private CanvasGroup titleGroup; //title group
    [SerializeField] private RectTransform titleRect; //title
    [SerializeField] private CanvasGroup buttonsGroup; //buttons group
     
    [Header("Timing")]
    [SerializeField] private float backgroundFade = 1.0f; 
    [SerializeField] private float titleFade = 0.8f;
    [SerializeField] private float buttonDelay = 0.5f; 
    [SerializeField] private float buttonFade = 0.5f; 
    [SerializeField] private float titleStartScale = 1.4f; 

    [Header("Run end behavior")]
    [SerializeField] private bool deleteSaveOnDeath = true;
    [SerializeField] private string mainMenuScene = "MainMenu";

    [SerializeField] private string restartScene = "";

    //can implement music later etc
    private bool shown; 
    private bool subscribed; 
    void Awake()
    {
        SetGroup(background, 0f, false); 
        SetGroup(titleGroup, 0f, false); 
        SetGroup(buttonsGroup, 0f, false); 
    }

    //GameManager may not be existing on enable -> subscribe in both places 
    void OnEnable() { Subscribe();}
    void Start() { Subscribe();}
    
    void OnDisable()
    {
        if (subscribed && GameManager.instance != null)
        {
            GameManager.instance.OnGameStateChanged -= OnGameStateChanged;
        }
        subscribed = false;
    }

    void Subscribe()
    {
        if(subscribed|| GameManager.instance == null)
        {
            return;
        }
        GameManager.instance.OnGameStateChanged += OnGameStateChanged; 
        subscribed = true; 
    }
    
    void OnGameStateChanged(GameManager.GameState state)
    {
        if(state == GameManager.GameState.GameOver)
        {
            Show();
        }
    }

    public void Show()
    {
        if (shown)
        {
            return; 
        }
        shown = true; 
        if(deleteSaveOnDeath && RunSaveManager.instance != null)
        {
            RunSaveManager.instance.DeleteSavedRun();
        }
        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = true; 
        StartCoroutine(Sequence()); 
    }

    IEnumerator Fade(CanvasGroup group, float duration, RectTransform scaleTarget)
    {
        float t = 0f; 
        while(t < duration)
        {
            t += Time.unscaledDeltaTime; 
            float k = Mathf.SmoothStep(0f, 1f, t / duration); 
            group.alpha = k;
            if (scaleTarget)
            {
                scaleTarget.localScale = Vector3.one * Mathf.Lerp(titleStartScale, 1f, k);
            }
            yield return null; 
        }
        group.alpha = 1f;
        if (scaleTarget)
        {
            scaleTarget.localScale = Vector3.one;
        }
    }

    IEnumerator Sequence()
    {
        Time.timeScale = 0f; 
        yield return Fade(background, backgroundFade, null); 
        titleRect.localScale = Vector3.one * titleStartScale; 
        yield return Fade(titleGroup, titleFade, titleRect); 

        yield return new WaitForSecondsRealtime(buttonDelay); 
        buttonsGroup.interactable = true; 
        buttonsGroup.blocksRaycasts = true; 
        yield return Fade(buttonsGroup, buttonFade, null); 
    }

    void SetGroup(CanvasGroup g, float alpha, bool clickable)
    {
        g.alpha = alpha; 
        g.interactable = clickable; 
        g.blocksRaycasts = clickable; 
    }

    public void Retry()
    {
        Time.timeScale = 1f; 
        if(GameManager.instance != null)
        {
            if (string.IsNullOrWhiteSpace(restartScene))
            {
                GameManager.instance.RestartGame();
            }
            else
            {
                GameManager.instance.RestartGame(restartScene); 
            }
        }
        else
        {
            SceneManager.LoadScene(string.IsNullOrWhiteSpace(restartScene) ? SceneManager.GetActiveScene().name : restartScene);
        }
    }
    public void QuitToMenu()
    {
        Time.timeScale = 1f; 
        if(GameManager.instance != null)
        {
            GameManager.instance.SetGameState(GameManager.GameState.Playing);
        }
        SceneManager.LoadScene(mainMenuScene);
    }
}
