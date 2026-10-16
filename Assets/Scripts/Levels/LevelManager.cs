using System;
using System.Collections.Generic;
using UnityEngine;

// TODO:
// - Connect transit map to level selection and scene loading
// - Apply saved layout seeds when generating each level
// - Integrate level discovery and connections with transit station UI

// Manages transit routes, level progression, and saved run data
public class LevelManager : MonoBehaviour
{
    public static LevelManager instance { get; private set; }

    [Serializable]
    public class LevelData
    {
        public string levelId;
        public string sceneName;
        public Vector2 mapPosition;

        public bool discovered;
        public bool completed;

        public int layoutSeed;
        public List<string> connections = new();
    }

    // Available levels
    [SerializeField] private List<LevelData> levels = new();

    // Transit map
    [SerializeField] private string startingLevelId = "Jungle";
    [SerializeField] private string finalLevelId = "Spaceport";

    public string CurrentLevelId { get; private set; }
    public IReadOnlyList<LevelData> Levels => levels;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Initialize new run
    public void StartNewRun()
    {
        LevelData startingLevel = FindLevel(startingLevelId);
        if (startingLevel == null)
        {
            Debug.LogError("Starting level not found!");
            return;
        }

        CurrentLevelId = startingLevelId;
        foreach (LevelData level in levels)
        {
            level.discovered = false;
            level.completed = false;
            level.connections.Clear();
            level.layoutSeed = UnityEngine.Random.Range(1, int.MaxValue);
        }

        GenerateMapPositions();
        startingLevel.discovered = true;

        // Reveal first destination
        LevelData nextLevel = FindNextUndiscoveredLevel();
        if (nextLevel != null)
            nextLevel.discovered = true;

        GenerateConnections();
        Debug.Log("New run started at: " + CurrentLevelId);
        PrintTransitMap();
    }

    // Randomize positions while keeping Jungle left and Spaceport right
    private void GenerateMapPositions()
    {
        foreach (LevelData level in levels)
        {
            if (level.levelId == startingLevelId)
            {
                level.mapPosition = new Vector2(0f, 0.5f);
            }
            else if (level.levelId == finalLevelId)
            {
                level.mapPosition = new Vector2(1f, 0.5f);
            }
            else
            {
                level.mapPosition = new Vector2(
                    UnityEngine.Random.Range(0.15f, 0.85f),
                    UnityEngine.Random.Range(0.1f, 0.9f)
                );
            }
        }
    }

    // Find level by unique ID
    public LevelData FindLevel(string levelId)
    {
        return levels.Find(level => level.levelId == levelId);
    }

    // Connect two levels in both directions
    private void ConnectLevels(LevelData first, LevelData second)
    {
        if (first == null || second == null || first == second)
            return;

        if (!first.connections.Contains(second.levelId))
            first.connections.Add(second.levelId);

        if(!second.connections.Contains(first.levelId))
            second.connections.Add(first.levelId);
    }

    // Generate subways connections between discovered leevels
    private void GenerateConnections()
    {
        foreach (LevelData level in levels)
            level.connections.Clear();

        List<LevelData> available = levels.FindAll(level => level.discovered);

        if (available.Count < 2)
        {
            return;
        }

        // Connect discovered levels into traversable network
        available.Sort((a,b) => a.mapPosition.x.CompareTo(b.mapPosition.x));

        for (int i = 0; i < available.Count - 1; i++)
        {
            ConnectLevels(available[i], available[i + 1]);
        }

        // Add some random additional connections
        for (int i = 0; i < available.Count; i++)
        {
            if (UnityEngine.Random.value < 0.4f)
            {
                LevelData other = available[UnityEngine.Random.Range(0, available.Count)];
                ConnectLevels(available[i], other);
            }
        }
    }

    // Find an undiscovered level closer to the Spaceport
    private LevelData FindNextUndiscoveredLevel()
    {
        List<LevelData> undiscovered = levels.FindAll(level => !level.discovered);

        if (undiscovered.Count == 0)
            return null;

        undiscovered.Sort((a,b) => a.mapPosition.x.CompareTo(b.mapPosition.x));

        return undiscovered[0];
    }

    // Mark level complete and regenerate network
    public void CompleteLevel(string levelId)
    {
        LevelData level = FindLevel(levelId);
        if (level == null || level.completed)
            return;

        level.completed = true;

        LevelData nextLevel = FindNextUndiscoveredLevel();
        if (nextLevel != null)
            nextLevel.discovered = true;

        GenerateConnections();
        Debug.Log("Completed Level: " + levelId);
    }

    // Print discovered levels and their connections for testing
    public void PrintTransitMap()
    {
        foreach (LevelData level in levels)
        {
            if (!level.discovered)
                continue;

            Debug.Log(level.levelId + " connects to: " + string.Join(", ", level.connections));
        }
    }


    // Export the current subway map for saving
    public List<LevelSaveData> ExportLevelData()
    {
        List<LevelSaveData> savedLevels = new();

        foreach (LevelData level in levels)
        {
            LevelSaveData savedLevel = new LevelSaveData
            {
                levelId = level.levelId,
                mapPosition = level.mapPosition,
                discovered = level.discovered,
                completed = level.completed,
                layoutSeed = level.layoutSeed,
                connections = new List<string>(level.connections)
            };

            savedLevels.Add(savedLevel);
        }

        return savedLevels;
    }

    // Restore the subway map from a saved run
    public void ImportLevelData(List<LevelSaveData> savedLevels, string currentLevelId)
    {
        if (savedLevels == null)
        {
            Debug.LogWarning("Cannot restore levels: save data is missing");
            return;
        }

        if (FindLevel(currentLevelId) == null)
        {
            Debug.LogWarning("Saved current level is not configured");
            return;
        }

        foreach (LevelSaveData savedLevel in savedLevels)
        {
            LevelData level = FindLevel(savedLevel.levelId);

            if (level == null)
            {
                Debug.LogWarning("Unknown saved level: " + savedLevel.levelId);
                continue;
            }

            level.mapPosition = savedLevel.mapPosition;
            level.discovered = savedLevel.discovered;
            level.completed = savedLevel.completed;
            level.layoutSeed = savedLevel.layoutSeed;

            level.connections = savedLevel.connections != null
                ? new List<string>(savedLevel.connections)
                : new List<string>();
        }

        CurrentLevelId = currentLevelId;

        Debug.Log("Transit network restored!");
    }

}
