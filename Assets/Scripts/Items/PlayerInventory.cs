using UnityEngine;

[RequireComponent(typeof(WeaponManager))]
[RequireComponent(typeof(EquipmentManager))]
[RequireComponent(typeof(StatsManager))]
public class PlayerInventory : MonoBehaviour
{
    public WeaponManager weaponManager { get; private set; }
    public EquipmentManager equipmentManager { get; private set; }
    public StatsManager statsManager { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        weaponManager = GetComponent<WeaponManager>();
        equipmentManager = GetComponent<EquipmentManager>();
        statsManager = GetComponent<StatsManager>();
    }
}
