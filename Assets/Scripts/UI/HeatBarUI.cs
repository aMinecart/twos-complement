using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HeatBarUI : MonoBehaviour
{
    [Header("UI Heat Reference")]
    [SerializeField] private Image heatBarFill; 
    [SerializeField] private TMP_Text heatPercentageText; 
    private HeatController heatController; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        heatController = HeatController.Instance; 
        
        if(heatController == null)
        {
            Debug.LogWarning("HeatBarUI: HeatController not found!!!"); 
            return; 
        }
        heatController.OnHeatChanged += UpdateHeatUI; 
        UpdateHeatUI(heatController. CurrentHeat, heatController.MaxHeat); 
    }

    private void OnDestroy()
    {
        if (heatController != null)
        {
            heatController.OnHeatChanged -= UpdateHeatUI; 
        }
    }
    // Update is called once per frame
    private void UpdateHeatUI(float currentHeat, float maxHeat)
    {
        float percentage = maxHeat > 0f ? Mathf.Clamp01(currentHeat / maxHeat) : 0f; 
        if(heatBarFill != null)
        {
            heatBarFill.fillAmount = percentage; 
            heatBarFill.color = new Color(239, 68, 68, 255); 
        }

        if(heatPercentageText != null)
        {
            int displayedPercentage = Mathf.RoundToInt(percentage * 100f); 
            heatPercentageText.text = displayedPercentage + "%"; 
        }
    }
}
