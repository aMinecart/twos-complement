using UnityEngine;
using UnityEngine.UI; 

public class DamagaeToPlayerUI : MonoBehaviour
{
   //will flash to show that the player has been hit 
   [Header("Player (found at runtime if empty)")]
   [SerializeField] private HealthTracker playerHealth; 

   [Header("Hit flash")]
   [SerializeField] private Color flashColor = Color.white; 
   [SerializeField, Range(0f, 1f)] private float minFlashAlpha = 0.35f;
   [SerializeField, Range(0f, 1f)] private float maxFlashAlpha = 0.9f; 
   [SerializeField] private float damageForMaxFlash = 30f;  
   [SerializeField] private float fadeOutTime = 0.5f;    
   [Header("Low health pulse")]
   [SerializeField] private bool lowHealthPulse = true;
   [SerializeField, Range(0f, 1f)] private float lowHealthThreshold = 0.3f;
   [SerializeField] private float pulseSpeed = 3f;
   [SerializeField, Range(0f, 1f)] private float pulseMaxAlpha = 0.35f;
   
   private Image image;
    private float previousHealth = -1f;
    private float healthFraction = 1f;
    private float flashAlpha;
 
    private void Awake()
    {
        image = GetComponent<Image>();
        image.raycastTarget = false;//can't block other UI clicks
        SetAlpha(0f);
    }

    private void OnEnable()
    {
        Debug.Log("DamageToPlayerUI: OnEnable"); 
        Subscribe(); 
    }

    private void Start()
    {
        //will find the player's health at runtime
        if(playerHealth == null && PlayerReference.Instance != null)
        {
            playerHealth = PlayerReference.Instance.GetComponentInChildren<HealthTracker>();
            if(playerHealth == null)
            {
                playerHealth = PlayerReference.Instance.GetComponentInParent<HealthTracker>(); 
            }
        }
        if (playerHealth == null)
        {
            Debug.LogWarning("DamageVignette: Player HealthTracker not found!");
            return;
        }

        previousHealth = playerHealth.CurrentHealth;
        healthFraction = playerHealth.HealthPercentage;

        Subscribe();
        Debug.Log($"DamageToPlayerUI: Start, player health = {playerHealth.CurrentHealth}");
    }

    private void OnDisable()
    {
        Debug.Log("DamageToPlayerUI: OnDisable");
        if(playerHealth != null)
        {
            playerHealth.OnHealthChanged -= OnHealthChanged; 
        }
    }

    private void Subscribe()
    {
        if(playerHealth == null)
        {
            return;
        }
        playerHealth.OnHealthChanged -= OnHealthChanged;
        playerHealth.OnHealthChanged += OnHealthChanged;
    }

    private void OnHealthChanged(float current, float max)
    {
        Debug.Log($"DamageVignette: health {previousHealth} -> {current}");
        if (previousHealth >= 0f && current < previousHealth)
        {
            float damage = previousHealth - current;
            float strength = Mathf.Clamp01(damage / Mathf.Max(0.01f, damageForMaxFlash));
            float target = Mathf.Lerp(minFlashAlpha, maxFlashAlpha, strength);
 
            flashAlpha = Mathf.Max(flashAlpha, target);
        }
 
        previousHealth = current;
        healthFraction = max > 0f ? Mathf.Clamp01(current / max) : 0f;
    }

    private void Update()
    {
        float fadePerSecond = maxFlashAlpha / Mathf.Max(0.01f, fadeOutTime);
        flashAlpha = Mathf.MoveTowards(flashAlpha, 0f, fadePerSecond * Time.unscaledDeltaTime);

        float pulseAlpha = 0f; 
        bool alive = playerHealth != null && !playerHealth.isDead;
 
        if (lowHealthPulse && alive && healthFraction < lowHealthThreshold)
        {
            float lowness = 1f - healthFraction / Mathf.Max(0.01f, lowHealthThreshold);
            float wave = Mathf.Sin(Time.unscaledTime * pulseSpeed) * 0.5f + 0.5f;
            pulseAlpha = wave * pulseMaxAlpha * lowness;
        }
 
        SetAlpha(Mathf.Max(flashAlpha, pulseAlpha));
    }

    private void SetAlpha(float alpha)
    {
        Color c = flashColor; 
        c.a = alpha; 
        image.color = c; 
    }

    [ContextMenu("Test Flash")]
    private void TestFlash()
    {
        flashAlpha = maxFlashAlpha;
    }
}
