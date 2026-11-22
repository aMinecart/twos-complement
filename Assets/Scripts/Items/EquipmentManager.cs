using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    private PlayerInventory inventory;
    private HashSet<Equipment> equipmentList = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventory = GetComponent<PlayerInventory>();
    }

    public void RegisterEquipment(Equipment equipment)
    {
        equipmentList.Add(equipment);
    }
}