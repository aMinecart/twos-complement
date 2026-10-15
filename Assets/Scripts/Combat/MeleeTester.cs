using UnityEngine;
using UnityEngine.InputSystem;

public class MeleeTester : MonoBehaviour
{
    [SerializeField] private MeleeWeapon meleeWeapon;

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            if (meleeWeapon != null)
                meleeWeapon.Attack();
        }
    }
}
