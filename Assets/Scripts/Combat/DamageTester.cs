using UnityEngine;
using UnityEngine.InputSystem;

public class DamageTester : MonoBehaviour
{
    [SerializeField] private Health targetHealth;
    [SerializeField] private float testDamage = 25f;

    void Update()
    {
        // Simulate incoming damage when T is pressed 
        if(Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame)
        {
            if(targetHealth != null)
            {
                targetHealth.TakeDamage(testDamage);
            }
        }
    }
}
