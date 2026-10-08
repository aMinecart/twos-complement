using UnityEngine;
using UnityEngine.InputSystem;

public class DamageTester : MonoBehaviour
{
    [SerializeField] private Health targetHealth;
    [SerializeField] private float testDamage = 25f;
    [SerializeField] private float testHealing = 25f;

    void Update()
    {
        if (targetHealth == null || Keyboard.current == null) return;

        // Simulate incoming damage when T is pressed 
        if(Keyboard.current.tKey.wasPressedThisFrame)
        {
            targetHealth.TakeDamage(testDamage);
        }

        // Simulate healing when H is pressed 
        if(Keyboard.current.hKey.wasPressedThisFrame)
        {
            targetHealth.Heal(testHealing);
        }
    }
}
