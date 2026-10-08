using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

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

        // DontDestroyOnLoad(gameObject);
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