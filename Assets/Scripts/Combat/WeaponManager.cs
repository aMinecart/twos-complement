using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    // Weapons available 
    [SerializeField] private List<Weapon> weapons = new();

    // Index of currently selected weapon
    private int currWeaponIndex = 0;

    // Returns currently equipped weapon 
    public Weapon ActiveWeapon
    {
        get
        {
            if (currWeaponIndex < 0 || currWeaponIndex >= weapons.Count)
                return null;

            return weapons[currWeaponIndex];
        }
    }

    // Enable only the currently selected weapon
    private void UpdateActiveWeapon()
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            if (weapons[i] != null)
                weapons[i].enabled = (i == currWeaponIndex);
        }  
    }

    void Start()
    {
        // Strt with first equipped weapon selected 
        currWeaponIndex = 0;
        UpdateActiveWeapon();
    }

    // Equip weapon - hotbar
    public void EquipWeapon(Weapon weapon)
    {
        if (weapon == null || weapons.Contains(weapon))
            return;

        weapons.Add(weapon);
        UpdateActiveWeapon();
    }

    // Unequip weapon without removing from player's inventory
    public void UnequipWeapon(Weapon weapon)
    {
        int removeIndex = weapons.IndexOf(weapon);
        
        if (removeIndex == -1)
            return;

        weapon.enabled = false;
        weapons.RemoveAt(removeIndex);

        if (weapons.Count == 0)
        {
            currWeaponIndex = 0;
            return;
        }

        // Adjust selected index after removing 
        if (removeIndex < currWeaponIndex)
            currWeaponIndex--;

        currWeaponIndex = Mathf.Clamp(currWeaponIndex, 0, weapons.Count - 1);

       UpdateActiveWeapon();
    }

    // Switch to next or previous weapon
    public void SwitchWeapons(bool reverse = false)
    {
        if (weapons.Count <= 1)
            return;

        int offset = reverse ? -1 : 1;

        // Wrap around when reaching beginning or end 
        currWeaponIndex = (currWeaponIndex + offset + weapons.Count) % weapons.Count;

        UpdateActiveWeapon();

        if (ActiveWeapon != null)
            Debug.Log("Switched to: " + ActiveWeapon.name);
    }

    // Select specific weapon from hotbar slot 
    public void SelectWeapon(int index)
    {
        if (index < 0 || index >= weapons.Count || index == currWeaponIndex)
            return;

        currWeaponIndex = index;

        UpdateActiveWeapon();

        if (ActiveWeapon != null)
            Debug.Log("Selected weapon: " + ActiveWeapon.name);
    }


    // Called by player input or enemy AI
    public void Attack()
    {
        if (ActiveWeapon != null && ActiveWeapon.isActiveAndEnabled)
            ActiveWeapon.Attack();
    }

    // Reload currently equiped weapon 
    public void Reload()
    {
        if (ActiveWeapon != null && ActiveWeapon.isActiveAndEnabled)
            ActiveWeapon.Reload();
    }

    // Receive attack input from the player 
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
            Attack();
    }

    // Receive reload input from player 
    public void OnReload(InputAction.CallbackContext context)
    {
        if (context.performed)
            Reload();
    }

    // Switch to next equipped weapon
    public void OnNext(InputAction.CallbackContext context)
    {
        if (context.performed)
            SwitchWeapons();
    }

    // Switch to previous equipped weapon
    public void OnPrevious(InputAction.CallbackContext context)
    {
        if (context.performed)
            SwitchWeapons(true);
    }
}