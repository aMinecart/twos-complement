using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WeaponManager))]
[RequireComponent(typeof(EquipmentManager))]
[RequireComponent(typeof(StatsManager))]
public class PlayerInventory : MonoBehaviour
{
    public WeaponManager weaponManager { get; private set; }
    public EquipmentManager equipmentManager { get; private set; }
    public StatsManager statsManager { get; private set; }

    void Start()
    {
        // Get references to the player's inventory related components 
        weaponManager = GetComponent<WeaponManager>();
        equipmentManager = GetComponent<EquipmentManager>();
        statsManager = GetComponent<StatsManager>();
    }
}