using UnityEngine;
using UnityEngine.UI; 
using TMPro;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private Image healthBarFill; 
    [SerializeField] private TMP_Text healthText; 
    
    [Header("Player Reference")]
    [SerializeField] private HealthTracker playerHealth;

    /*//temporary testing to ensure that the health bar decreases correctly!
    [Header("Temporary UI Testing")]
    [SerializeField] private bool testHealthBar = true; 
    private float testHealth = 100f; */
    
    private void OnEnable()
    {
        if(playerHealth != null)
        {
            playerHealth.OnHealthChanged += UpdateHealthUI; 
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(playerHealth != null)
        {
            UpdateHealthUI(
                playerHealth.CurrentHealth, 
                playerHealth.MaxHealth
            );
        }
        else
        {
            Debug.LogWarning("PlayerHealthUI: Player HealthTracker isn't assigned!");
        }
    }

    private void OnDisable()
    {
        if(playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHealthUI; 
        }
    }

    private void UpdateHealthUI(float currentHealth, float maxHealth)
    {
        float percentage = maxHealth > 0f ? Mathf.Clamp01(currentHealth / maxHealth) : 0f;
        if(healthBarFill != null)
        {
            healthBarFill.fillAmount = percentage;
            if (percentage > 0.6f)
            {
                healthBarFill.color = new Color32(34,197,94,255); //healthy 
            } else if(percentage > 0.3f)
            {
                healthBarFill.color = new Color32(234,179,8,255); //caution
            } else if(percentage > 0.1f)
            {
                healthBarFill.color = new Color32(249, 115, 22, 225); //danger
            }
            else
            {
                healthBarFill.color = new Color32(239, 68, 68, 255); //critical 
            }
        }


        //changes the health bar color based on the remaining health (green (70-100%); yellow (31-59%); orange(11-30%); red(0-10%))
        if(healthText != null)
        {
            healthText.text = Mathf.CeilToInt(currentHealth) + " / " + Mathf.CeilToInt(maxHealth);
        }
    }

   /*private void Update()
   {
        if (!testHealthBar || Keyboard.current == null)
            return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            testHealth = 100f;
        else if (Keyboard.current.digit2Key.wasPressedThisFrame)
            testHealth = 75f;
        else if (Keyboard.current.digit3Key.wasPressedThisFrame)
            testHealth = 40f;
        else if (Keyboard.current.digit4Key.wasPressedThisFrame)
            testHealth = 10f;
        else if (Keyboard.current.digit5Key.wasPressedThisFrame)
            testHealth = 0f;
        else
            return;

        UpdateHealthUI(testHealth, 100f);

        //TESTING: triggers the death Screen @ zero health
        if (testHealth <= 0f)
        {
            if (GameManager.instance != null)
            {
                Debug.Log("Health test reached zero. Triggering Game Over.");
                GameManager.instance.GameOver();
            }
            else
            {
                Debug.LogError("Cannot trigger Game Over: GameManager.instance is null!");
            }
        }
    }*/
}
