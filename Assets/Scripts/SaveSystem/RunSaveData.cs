using System;
using System.Collections.Generic;
using UnityEngine;

// Stores information needed to restore a saved run.
[Serializable]
public class RunSaveData
{
    // Current level
    public string currentLevelId;

    // Player position and rotation
    public Vector3 playerPosition;
    public Quaternion playerRotation;

    // Player health
    public float currentHealth;
    public float maxHealth;
    public float damageResist;

    // All weapons owned by the player, including ammunition
    public List<WeaponSaveData> weapons = new();
    // Weapons assigned to the hotbar, in order
    public List<string> hotbarWeaponIds = new();
    // Currently selected hotbar slot (-1 = empty hands)
    public int selectedWeaponIndex = -1;

    public List<ItemSaveData> inventory = new();

    // Subway map
    public List<LevelSaveData> levels = new();
}

// Information about a weapon owned by the player.
[Serializable]
public class WeaponSaveData
{
    public string weaponId;
    public int magazineAmmo;
    public int reserveAmmo;
}

// Information about an item in the player's inventory.
[Serializable]
public class ItemSaveData
{
    public string itemId;
    public int quantity;
}

// Information about each level in the transit network.
[Serializable]
public class LevelSaveData
{
    public string levelId;
    public Vector2 mapPosition;

    public bool discovered;
    public bool completed;

    public int layoutSeed;
    public List<string> connections = new();
}