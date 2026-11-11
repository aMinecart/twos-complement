using UnityEngine;
using System; 

public class HeatController : MonoBehaviour
{
    
    [Header("Heat Settings")]
    [SerializeField] private float maxHeat = 100f; 
    public float currentHeat = 0f;

    [Header("Cooling")]
    [SerializeField] private float coolingRate = 5f; 

    public static HeatController Instance { get; private set; }
    public event Action<float, float> OnHeatChanged; 

    public float CurrentHeat => currentHeat;
    public float MaxHeat => maxHeat; 

    public float HeatPercentage => maxHeat > 0f ? Mathf.Clamp01(currentHeat / maxHeat) : 0f;  
    private float heatAddedThisFrame; 
    
    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple heat instances!!! Deleting the new instance");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        currentHeat = Mathf.Clamp(currentHeat, 0f, maxHeat);
    }

    private void Start()
    {
        NotifyHeatChanged();
    }

    // Update is called once per frame
    private void Update()
    {
        currentHeat = Mathf.Clamp(currentHeat, 0f, maxHeat); 

        if(heatAddedThisFrame <= 0f)
        {
            ChangeHeat(-coolingRate * Time.deltaTime);
        }
        heatAddedThisFrame = 0f; 
    }

    //when player is near an enemy 
    public void AddHeat(float amount)
    {
        if(amount <= 0f)
        {
            return;
        }
        heatAddedThisFrame += amount; 
        ChangeHeat(amount); 
    }

    public void CoolDown(float amount)
    {
        if(amount <= 0f)
        {
            return; 
        }
        ChangeHeat(-amount); 
    }

    public void SetHeat(float amount)
    {
        ChangeHeat(amount - currentHeat);
    }

    private void ChangeHeat(float amount)
    {
        float previousHeat = currentHeat; 
        currentHeat = Mathf.Clamp(currentHeat + amount, 0f, maxHeat);

        if(!Mathf.Approximately(previousHeat, currentHeat))
        {
            NotifyHeatChanged(); 
        }
    }

    private void NotifyHeatChanged()
    {
        //Debug.Log("Here");
        OnHeatChanged?.Invoke(currentHeat, maxHeat); 
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
