using UnityEngine;

public class EquipmentPickup : Pickup
{
    [SerializeField] private Equipment equipment;

    public static EquipmentPickup CreateRandEquipmentPickup(System.Random randGen, BuffPickup basePrefab, Vector3 position)
    {
        return null;
    }

    protected override void RegisterPickup(PlayerInventory inventory)
    {
        inventory.equipmentManager.RegisterEquipment(equipment);
    }
}