using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemManager : MonoBehaviour
{
    public List<Weapon> weapons = new();
    public int currWeaponIndex = 0;

    // public List<Equipment> equipment;
    // public BuffsRecord statBuffs;

    public void AddWeapon(Weapon weapon)
    {
        // enable weapon if the player currently has none
        weapon.enabled = (weapons.Count == 0);
        weapons.Add(weapon);
    }

    public void SwitchWeapons(bool reverse = false)
    {
        if (weapons.Count <= 1)
        {
            return;
        }

        weapons[currWeaponIndex].enabled = false;

        int offset = reverse ? -1 : 1;
        currWeaponIndex = (currWeaponIndex + offset) % weapons.Count;

        weapons[currWeaponIndex].enabled = true;
    }

    public void RemoveWeapon(Weapon weapon)
    {
        int removeIndex = weapons.IndexOf(weapon);
        if (removeIndex != -1)
        {
            if (removeIndex == currWeaponIndex)
            {
                SwitchWeapons();
            }

            weapon.enabled = false;
            weapons.RemoveAt(removeIndex);
        }
    }

    /*
    void OnCollisionEnter(Collision collision)
    {
        // to be implemented with item pickup system
        if (collision.gameObject.layer == LayerMask.NameToLayer("Pickup"))
        {
            print("");
        }
    }
    */

    public void OnWeaponSwitch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SwitchWeapon();
        }
    }

    void Update()
    {
        
    }
}