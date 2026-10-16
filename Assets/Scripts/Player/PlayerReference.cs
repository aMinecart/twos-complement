using UnityEngine;

public class PlayerReference : MonoBehaviour
{
    public static PlayerReference Instance { get; private set; } = null;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple player instances detected. Deleting new instance...");
            Destroy(this);
            return;
        }

        Instance = this;
    }
}
