using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

// TODO:
// - Integrate equippedWeapons with the inventory and hotbar UI
// - Test weapon equipping, removal, and switching with collected weapons
// - Verify hotbar restoration after loading a saved run
public class WeaponManager : MonoBehaviour
{
    // All weapons player owns
    [SerializeField] private List<Weapon> weapons = new();

    // Weapons assigned to hotbar
    [SerializeField] private List<Weapon> equippedWeapons = new();
    [SerializeField, Min(1)] private int maxEquippedWeapons = 4;

    // Automatically assign starting weapons only for a fresh setup
    [SerializeField] private bool autoEquipStartingWeapons = true;

    // Index of currently selected hotbar weapon
    private int currWeaponIndex = -1;

    // Returns currently equipped weapon
    public Weapon ActiveWeapon
    {
        get
        {
            if (currWeaponIndex < 0 || currWeaponIndex >= equippedWeapons.Count)
            {
                return null;
            }

            return equippedWeapons[currWeaponIndex];
        }
    }

    // Read-only access for inventory UI and save system
    public IReadOnlyList<Weapon> OwnedWeapons => weapons;
    public IReadOnlyList<Weapon> EquippedWeapons => equippedWeapons;

    // Currently selected hotbar index (-1 means empty hands)
    public int SelectedWeaponIndex => currWeaponIndex;

    // disable current weapon (if any) and switch to the weapon at the specified index
    private void SwitchToWeapon(int index)
    {
        // log faulty index input
        if (index < -1 || index >= equippedWeapons.Count)
        {
            Debug.LogError($"Attempted to switch to invalid hotbar index {index}");
            return;
        }

        // check if weapon is already being used
        if (currWeaponIndex == index)
        {
            return;
        }

        // disable current active weapon
        if (currWeaponIndex >= 0 && currWeaponIndex < equippedWeapons.Count)
        {
            Weapon previous = equippedWeapons[currWeaponIndex];
            if (previous != null)
                previous.enabled = false;
                if(previous.WeaponModel != null) previous.WeaponModel.SetActive(false); //Disables weapon model
        }

        // switch to new weapon and enable it
        currWeaponIndex = index;

        // enable new weapon, unless caller requested to unequip all weapons (index = -1)
        if (currWeaponIndex != -1)
        {
            Weapon selected = equippedWeapons[currWeaponIndex];
            if(selected != null)
                selected.enabled = true;
                if(selected.WeaponModel != null) selected.WeaponModel.SetActive(true); //Makes weapon visible
        }

        Debug.Log("Switched to: " + (ActiveWeapon != null ? ActiveWeapon.GetType().Name : "empty hands"));
    }

    void Start()
    {
        // Register weapons attached to the player
        foreach (Weapon weapon in GetComponentsInChildren<Weapon>(includeInactive: true))
        {
            if (weapon != null && !weapons.Contains(weapon))
            {
                weapons.Add(weapon);
            }
        }

        // Disable all weapons initially
        foreach (Weapon weapon in weapons)
        {
            if (weapon != null)
                weapon.enabled = false;
        }

        // Remove invalid or unowned hotbar weapons
        equippedWeapons.RemoveAll(weapon =>
            weapon == null || !weapons.Contains(weapon));

        // Respect the hotbar capacity
        if (equippedWeapons.Count > maxEquippedWeapons)
        {
            equippedWeapons.RemoveRange(
                maxEquippedWeapons,
                equippedWeapons.Count - maxEquippedWeapons
            );
        }

        // Only fill the hotbar automatically for initial setup
        if (autoEquipStartingWeapons)
        {
            foreach (Weapon weapon in weapons)
            {
                if (equippedWeapons.Count >= maxEquippedWeapons)
                    break;

                if (weapon != null && !equippedWeapons.Contains(weapon))
                {
                    equippedWeapons.Add(weapon);
                }
            }
        }

        currWeaponIndex = -1;

        // Select first assigned weapon
        if (equippedWeapons.Count > 0)
            SwitchToWeapon(0);
    }


    // Add a weapon to player's inventory without equipping it
    public void AddWeapon(Weapon weapon)
    {
        if (weapon == null || weapons.Contains(weapon))
        {
            return;
        }

        weapon.enabled = false; // New weapons start unequipped
        weapons.Add(weapon); // Add to weapons list

        Debug.Log("Added to inventory: " + weapon.name);
    }

    // Remove a weapon from the player's inventory
    public void RemoveWeapon(int index)
    {
        if (index < 0 || index >= weapons.Count)
        {
            Debug.LogError("Invalid inventory weapon index: " + index);
            return;
        }

        Weapon weapon = weapons[index];

        // Remove from hotbar first, if equipped
        int hotbarIndex = equippedWeapons.IndexOf(weapon);

        if (hotbarIndex >= 0)
        {
            RemoveFromHotbar(hotbarIndex);
        }

        if (weapon != null)
            weapon.enabled = false;

        weapons.RemoveAt(index);

        Debug.Log("Removed weapon from inventory: " + (weapon != null ? weapon.name : "Unknown"));
    }

    // Assign an owned weapon to the hotbar
    public bool EquipWeapon(Weapon weapon)
    {
        if (weapon == null || !weapons.Contains(weapon))
            return false;

        if (equippedWeapons.Contains(weapon))
            return false;

        if (equippedWeapons.Count >= maxEquippedWeapons)
        {
            Debug.Log("Hotbar is full!");
            return false;
        }

        equippedWeapons.Add(weapon);
        weapon.enabled = false;

        // Select it if the hotbar was previously empty
        if (currWeaponIndex == -1)
            SwitchToWeapon(0);

        Debug.Log("Equipped to hotbar: " + weapon.name);
        return true;
    }

    // Remove a weapon from the hotbar without removing ownership
    public void RemoveFromHotbar(int index)
    {
        if (index < 0 || index >= equippedWeapons.Count)
            return;

        Weapon weapon = equippedWeapons[index];
        bool wasActive = index == currWeaponIndex;

        if (weapon != null)
            weapon.enabled = false;

        equippedWeapons.RemoveAt(index);

        if (equippedWeapons.Count == 0)
        {
            currWeaponIndex = -1;
        }
        else if (wasActive)
        {
            currWeaponIndex = -1;
            SwitchToWeapon(Mathf.Min(index, equippedWeapons.Count - 1));
        }
        else if (index < currWeaponIndex)
        {
            // Selected weapon shifted one slot left
            currWeaponIndex--;
        }

        Debug.Log("Removed from hotbar: " +
            (weapon != null ? weapon.name : "Unknown"));
    }


    // Restore hotbar assignments in the supplied order
    public void SetHotbar(List<Weapon> assignedWeapons, int selectedIndex)
    {
        // Disable the currently active weapon
        if (ActiveWeapon != null)
            ActiveWeapon.enabled = false;

        // Disable all owned weapons before restoring assignments
        foreach (Weapon weapon in weapons)
        {
            if (weapon != null)
                weapon.enabled = false;
        }

        equippedWeapons.Clear();
        currWeaponIndex = -1;

        if (assignedWeapons != null)
        {
            foreach (Weapon weapon in assignedWeapons)
            {
                if (equippedWeapons.Count >= maxEquippedWeapons)
                    break;

                if (weapon != null &&
                    weapons.Contains(weapon) &&
                    !equippedWeapons.Contains(weapon))
                {
                    equippedWeapons.Add(weapon);
                }
            }
        }

        // Restore selected weapon, including empty hands
        if (selectedIndex >= 0 && selectedIndex < equippedWeapons.Count)
        {
            SwitchToWeapon(selectedIndex);
        }

        Debug.Log("Hotbar assignments restored.");
    }

    // Restore the exact list of weapons the player owns
    public void RestoreOwnedWeapons(List<Weapon> restoredWeapons)
    {
        // Disable all currently registered weapons
        foreach (Weapon weapon in weapons)
        {
            if (weapon != null)
                weapon.enabled = false;
        }

        weapons.Clear();
        equippedWeapons.Clear();
        currWeaponIndex = -1;

        if (restoredWeapons == null)
            return;

        foreach (Weapon weapon in restoredWeapons)
        {
            if (weapon == null || weapons.Contains(weapon))
                continue;

            weapon.enabled = false;
            weapons.Add(weapon);
        }

        Debug.Log("Weapon inventory restored.");
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
        {
            ActiveWeapon.Attack();
            if(ActiveWeapon.WeaponAnimation != null)
                ActiveWeapon.WeaponAnimation.Play();
            if(ActiveWeapon.WeaponSound != null)
                ActiveWeapon.WeaponSound.Play();
        }
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
        if (context.performed && equippedWeapons.Count > 0)
            SwitchToWeapon((currWeaponIndex + 1) % equippedWeapons.Count);
    }

    // Switch to previous equipped weapon
    public void OnPrevious(InputAction.CallbackContext context)
    {
        if (context.performed && equippedWeapons.Count > 0)
        {
            int prevIndex = currWeaponIndex == -1 ? equippedWeapons.Count - 1 : (currWeaponIndex - 1 + equippedWeapons.Count) % equippedWeapons.Count;
            SwitchToWeapon(prevIndex);
        }
    }
}