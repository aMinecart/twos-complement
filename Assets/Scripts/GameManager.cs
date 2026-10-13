using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public enum GameState
    {
        Playing,
        GameOver, 
        Victory
    }

    public GameState CurrentState { get; private set; } = GameState.Playing;
    public event Action<GameState> OnGameStateChanged;
    [SerializeField] private string startingScene = "SampleScene";
    public int CurrentLevel { get; private set; } = 1;


    [RuntimeInitializeOnLoadMethod]
    private static void Initialization()
    {
        if (FindAnyObjectByType<GameManager>() != null)
        {
            return;
        }

        GameObject parent = new() { name = "AutoSingleton" };
        instance = parent.AddComponent<GameManager>();
        DontDestroyOnLoad(parent);
    }

    private void Awake()
    {
        if (FindFirstObjectByType<GameManager>() != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        // DontDestroyOnLoad(gameObject);
    }

    public void SetGameState(GameState newState)
    {
        if (CurrentState == newState)
            return;

        CurrentState = newState;
        Debug.Log("Game state changed to: " + CurrentState);
        OnGameStateChanged?.Invoke(CurrentState);
    }

    public void GameOver()
    {
        SetGameState(GameState.GameOver);
    }

    public void WinGame()
    {
        SetGameState(GameState.Victory);
    }

    public void LoadNextLevel(string sceneName)
    {
        if(CurrentState != GameState.Playing)
            return;
        if(string.IsNullOrWhiteSpace(sceneName))
            return;

        CurrentLevel++;
        SceneManager.LoadScene(sceneName);
    }

    public void RestartGame()
    {
        CurrentLevel = 1;
        SetGameState(GameState.Playing);
        SceneManager.LoadScene(startingScene);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}