using UnityEngine;

public class HeatController : MonoBehaviour
{
    public float currentHeat = 0f;

    public static HeatController Instance { get; private set; } = null;

    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple heat instances!!! Deleting the new instance");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(currentHeat);
        currentHeat += Time.deltaTime * 0.33f;
    }
}
