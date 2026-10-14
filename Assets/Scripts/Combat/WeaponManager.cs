using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    // Weapons available 
    [SerializeField] private List<Weapon> weapons = new();

    // Index of currently selected weapon
    private int currWeaponIndex = -1;

    // Returns currently equipped weapon 
    public Weapon ActiveWeapon
    {
        get
        {
            if (currWeaponIndex < 0 || currWeaponIndex >= weapons.Count)
            {
                return null;
            }

            return weapons[currWeaponIndex];
        }
    }

    // disable current weapon (if any) and switch to the weapon at the specified index
    private void SwitchToWeapon(int index)
    {
        // log faulty index input
        if (index < -1 || index >= weapons.Count)
        {
            Debug.LogError($"Attempted to switch to invalid weapon index {index}");
        }

        // check if weapon is already being used
        if (currWeaponIndex == index)
        {
            return;
        }

        // disable current active weapon
        if (currWeaponIndex >= 0 && currWeaponIndex < weapons.Count)
        {
            weapons[currWeaponIndex].enabled = false;
        }

        // switch to new weapon and enable it
        currWeaponIndex = index;

        // enable new weapon, unless caller requested to unequip all weapons (index = -1)
        if (currWeaponIndex != -1)
        {
            weapons[currWeaponIndex].enabled = true;
        }

        Debug.Log("Switched to: " + (ActiveWeapon != null ? ActiveWeapon.name : "empty hands"));
    }

    void Start()
    {
        // Register all weapons that the player starts with 
        foreach (Weapon weapon in GetComponentsInChildren<Weapon>(includeInactive: true))
        {
            if (weapon != null && !weapons.Contains(weapon))
            {
                weapons.Add(weapon);
            }
        }

        // Start with first equipped weapon selected, if any
        if (weapons.Count > 0)
        {
            SwitchToWeapon(0);
        }
    }

    // Add a weapon to player's inventory without equipping it 
    public void AddWeapon(Weapon weapon)
    {
        if (weapon == null || !weapons.Contains(weapon))
        {
            return;
        }

        weapon.enabled = false; // Keep unequipped weapons from attacking 
        weapons.Add(weapon); // Add to weapons list
        Debug.Log("Added to inventory: " + weapon.name);
    }

    // Remove a weapon from the player's inventory
    public void RemoveWeapon(int index)
    {
        // log faulty index input
        if (index < 0 || index >= weapons.Count)
        {
            Debug.LogError($"Attempted to remove invalid weapon index {index}");
            return;
        }

        // check if removing currently equipped weapon
        if (index == currWeaponIndex)
        {
            // switch away from current weapon
            if (weapons.Count == 1)
            {
                // last weapon, switch to empty hands
                SwitchToWeapon(-1);
            }
            else
            {
                // switch to next weapon in list
                SwitchToWeapon((currWeaponIndex + 1) % weapons.Count);
            }
        }

        Debug.Log($"Removing weapon: {weapons[index].name}");

        // remove weapon and adjust current weapon index if necessary
        weapons.RemoveAt(index);
        if (index > currWeaponIndex)
        {
            currWeaponIndex--;
        }
    }

    // Check whether player owns a weapon
    public bool HasWeapon(Weapon weapon)
    {
        return weapons.Contains(weapon);
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
            SwitchToWeapon((currWeaponIndex + 1) % weapons.Count);
    }

    // Switch to previous equipped weapon
    public void OnPrevious(InputAction.CallbackContext context)
    {
        if (context.performed)
            SwitchToWeapon((currWeaponIndex - 1 + weapons.Count) % weapons.Count);
    }
}