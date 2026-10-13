using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WeaponManager))]
[RequireComponent(typeof(EquipmentManager))]
[RequireComponent(typeof(StatsManager))]
public class PlayerInventory : MonoBehaviour
{
    // All weapons the player owns, including unequipped weapons 
    [SerializeField] private List<Weapon> ownedWeapons = new();

    public WeaponManager weaponManager { get; private set; }
    public EquipmentManager equipmentManager { get; private set; }
    public StatsManager statsManager { get; private set; }

    void Awake()
    {
        // Get references to the player's inventory related components 
        weaponManager = GetComponent<WeaponManager>();
        equipmentManager = GetComponent<EquipmentManager>();
        statsManager = GetComponent<StatsManager>();
    }

    void Start()
    {
        // Register weapons the player starts with 
        foreach (Weapon weapon in GetComponentsInChildren<Weapon>(true))
        {
            if (weapon != null && !ownedWeapons.Contains(weapon))
                ownedWeapons.Add(weapon);
        }
    }

    // Add a weapon to player's inventory without equipping it 
    public void AddWeapon(Weapon weapon)
    {
        if (weapon == null || ownedWeapons.Contains(weapon))
            return;

        ownedWeapons.Add(weapon);

        // Keep unequipped weapons from attacking 
        weapon.enabled = false;

        Debug.Log("Added to inventory: " + weapon.name);
    }

    // Remove a weapon from the player's inventory 
    public void RemoveWeapon(Weapon weapon)
    {
        if (weapon == null || !ownedWeapons.Contains(weapon))
            return;

        // Unequip weapon before removing ir from inventory
        weaponManager.UnequipWeapon(weapon);
        ownedWeapons.Remove(weapon);
    }

    // Check whether player owns a weapon
    public bool HasWeapon(Weapon weapon)
    {
        return ownedWeapons.Contains(weapon);
    }

    // Equip an owned weapon for combat
    public void EquipWeapon(Weapon weapon)
    {
        if (!HasWeapon(weapon))
            return;

        weaponManager.EquipWeapon(weapon);
    }
}
