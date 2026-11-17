using UnityEngine;
using UnityEngine.InputSystem; 

//TEMPORARY TESTING HELPER: switch weapon and ammo w/keys 
//1-4 selects hotbar 1-4 
//0 empy hands/no weapon 
//K -> set to active ranged weapons ammo to 0 (tests the ammo empty color and dimmed crosshair)
//L -> refills the active ranged weapon's magazine 
//J-> sets ammo to 2 (tests the lower ammo color)
public class Weapontestkeys : MonoBehaviour
{
    [SerializeField] private WeaponManager weaponManager; 
    [SerializeField] private bool keysEnabled = true; 

    private void Start()
    {
        if(!keysEnabled || Keyboard.current == null){
            return; 
        }

        if (weaponManager == null)
        {
            if (PlayerReference.Instance != null)
                weaponManager = PlayerReference.Instance.GetComponentInChildren<WeaponManager>();
 
            if (weaponManager == null)
                return;
        }

        Keyboard kb = Keyboard.current; 

        //hotbar slot 
        if (kb.digit1Key.wasPressedThisFrame) weaponManager.SelectHotbarSlot(0);
        if (kb.digit2Key.wasPressedThisFrame) weaponManager.SelectHotbarSlot(1);
        if (kb.digit3Key.wasPressedThisFrame) weaponManager.SelectHotbarSlot(2);
        if (kb.digit4Key.wasPressedThisFrame) weaponManager.SelectHotbarSlot(3);
        if (kb.digit0Key.wasPressedThisFrame) weaponManager.SelectHotbarSlot(-1); // Empty hands
 
        // Ammo changes only apply to ranged weapons
        if (weaponManager.ActiveWeapon is RangedWeapon ranged)
        {
            if (kb.kKey.wasPressedThisFrame) ranged.SetAmmo(0);
            if (kb.jKey.wasPressedThisFrame) ranged.SetAmmo(2);
            if (kb.lKey.wasPressedThisFrame) ranged.SetAmmo(ranged.MagazineSize);
        }
    }
}
