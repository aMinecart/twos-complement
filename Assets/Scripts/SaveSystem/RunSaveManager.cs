using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

// TODO: Connect
// - SaveRun and PrepareContinueRun to game's Save & Quit and Continue run menu functionality
// - Restore player data after destination scene and terrain are loaded
// - Add inventory item restoration and support for weapons that are not already
// attached to the Player prefab


// Handles saving and loading run data
public class RunSaveManager : MonoBehaviour
{
    public static RunSaveManager instance { get; private set; }
    private string SavePath => Path.Combine(Application.persistentDataPath, "astray_run.json");

    public bool HasSavedRun => File.Exists(SavePath);

    // Data waiting to be restored after the saved scene loads
    public RunSaveData PendingLoadData { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Write run data to a Json file
    public bool SaveRun(RunSaveData data)
    {
        if (data == null)
        {
            Debug.LogWarning("Cannot save: run data is null");
            return false;
        }

        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SavePath, json);

            Debug.Log("Run saved successfully!");
            return true;
        }
        catch (Exception error)
        {
            Debug.LogError("Failed to save run: " + error.Message);
            return false;
        }
    }

    // Read run data from saved JSON file
    public RunSaveData LoadRun()
    {
        if (!HasSavedRun)
        {
            Debug.LogWarning("No saved run found");
            return null;
        }

        try
        {
            string json = File.ReadAllText(SavePath);
            RunSaveData data = JsonUtility.FromJson<RunSaveData>(json);

            if (data == null)
            {
                Debug.LogError("Save file is invalid");
                return null;
            }

            Debug.Log("Run data loaded successfully!");
            return data;
        }
        catch (Exception error)
        {
            Debug.LogError("Failed to load run: " + error.Message);
            return null;
        }
    }

    // Capture player's current position and health
    public RunSaveData CapturePlayerData(GameObject player)
    {
        if (player == null)
        {
            Debug.LogWarning("Cannot capture player data: player not found");
            return null;
        }

        HealthTracker health = player.GetComponent<HealthTracker>();

        if (health == null)
        {
            Debug.LogWarning("Player does not have a HealthTracker");
            return null;
        }

        RunSaveData data = new RunSaveData
        {
            playerPosition = player.transform.position,
            playerRotation = player.transform.rotation,
            currentHealth = health.CurrentHealth,
            maxHealth = health.MaxHealth,
            damageResist = health.DamageResist
        };

        return data;
    }

    // Capture player information and current transit map
    public RunSaveData CaptureRunData(GameObject player)
    {
        if (LevelManager.instance == null)
        {
            Debug.LogWarning("Cannot capture run: LevelManager not found");
            return null;
        }

        RunSaveData data = CapturePlayerData(player);

        if (data == null)
            return null;

        if (string.IsNullOrEmpty(LevelManager.instance.CurrentLevelId))
        {
            Debug.LogWarning("Cannot capture run: no active level");
            return null;
        }

        data.currentLevelId = LevelManager.instance.CurrentLevelId;
        data.levels = LevelManager.instance.ExportLevelData();

        // Include owned weapons, ammo, and hotbar assignments
        CaptureWeaponData(player, data);
        return data;
    }

    // Restore the player's health, position, and rotation
    public bool RestorePlayerData(GameObject player, RunSaveData data)
    {
        if (player == null || data == null)
        {
            Debug.LogWarning("Cannot restore player: missing player or save data");
            return false;
        }

        HealthTracker health = player.GetComponent<HealthTracker>();

        if (health == null)
        {
            Debug.LogWarning("Cannot restore player: HealthTracker not found");
            return false;
        }

        // Restore health through HealthTracker
        health.InitializeHealth(data.currentHealth, data.maxHealth, data.damageResist);

        // Restore the player's exact saved position and rotation
        Rigidbody rb = player.GetComponent<Rigidbody>();

        if (rb != null && !rb.isKinematic)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        player.transform.SetPositionAndRotation(
            data.playerPosition,
            data.playerRotation
        );

        Physics.SyncTransforms();

        Debug.Log("Player health and position restored!");
        return true;
    }


    // Read the saved run and prepare it for scene loading
    public bool PrepareContinueRun()
    {
        RunSaveData data = LoadRun();

        if (data == null)
            return false;

        if (LevelManager.instance == null)
        {
            Debug.LogWarning("Cannot continue: LevelManager not found");
            return false;
        }

        // Restore the transit network before loading the level
        LevelManager.instance.ImportLevelData(
            data.levels,
            data.currentLevelId
        );

        PendingLoadData = data;

        Debug.Log("Saved run prepared for loading");
        return true;
    }

    // Capture owned weapons, ammunition, and hotbar assignments
    public void CaptureWeaponData(GameObject player, RunSaveData data)
    {
        if (player == null || data == null)
        {
            Debug.LogWarning("Cannot capture weapons: missing player or run data.");
            return;
        }

        WeaponManager manager = player.GetComponent<WeaponManager>();

        if (manager == null)
        {
            Debug.LogWarning("Cannot capture weapons: WeaponManager not found.");
            return;
        }

        data.weapons.Clear();
        data.hotbarWeaponIds.Clear();

        foreach (Weapon weapon in manager.OwnedWeapons)
        {
            if (weapon == null || string.IsNullOrWhiteSpace(weapon.WeaponId))
            {
                Debug.LogWarning("Skipping weapon with missing ID.");
                continue;
            }

            WeaponSaveData savedWeapon = new WeaponSaveData
            {
                weaponId = weapon.WeaponId,
                magazineAmmo = weapon is RangedWeapon ranged
                    ? ranged.CurrentAmmo
                    : 0
            };

            data.weapons.Add(savedWeapon);
        }

        foreach (Weapon weapon in manager.EquippedWeapons)
        {
            if (weapon != null && !string.IsNullOrWhiteSpace(weapon.WeaponId))
            {
                data.hotbarWeaponIds.Add(weapon.WeaponId);
            }
        }

        data.selectedWeaponIndex = manager.SelectedWeaponIndex;

        Debug.Log("Weapon data captured.");
    }

    // Restore owned weapons, ammo, and hotbar assignments
    public bool RestoreWeaponData(GameObject player, RunSaveData data)
    {
        if (player == null || data == null)
            return false;

        WeaponManager manager = player.GetComponent<WeaponManager>();

        if (manager == null)
        {
            Debug.LogWarning("Cannot restore weapons: WeaponManager not found.");
            return false;
        }

        if (data.weapons == null || data.hotbarWeaponIds == null)
        {
            Debug.LogWarning("Saved weapon data is missing.");
            return false;
        }

        // Find weapon components attached to the Player
        Weapon[] availableWeapons =
            player.GetComponentsInChildren<Weapon>(true);

        Dictionary<string, Weapon> weaponLookup = new();

        foreach (Weapon weapon in availableWeapons)
        {
            if (weapon == null || string.IsNullOrWhiteSpace(weapon.WeaponId))
                continue;

            if (!weaponLookup.TryAdd(weapon.WeaponId, weapon))
            {
                Debug.LogWarning("Duplicate weapon ID: " + weapon.WeaponId);
            }
        }

        // Rebuild the inventory from saved weapon IDs
        List<Weapon> restoredInventory = new();

        foreach (WeaponSaveData savedWeapon in data.weapons)
        {
            if (weaponLookup.TryGetValue(savedWeapon.weaponId, out Weapon weapon))
            {
                if (!restoredInventory.Contains(weapon))
                    restoredInventory.Add(weapon);
            }
        }

        manager.RestoreOwnedWeapons(restoredInventory);

        // Restore ammunition for saved weapons
        foreach (WeaponSaveData savedWeapon in data.weapons)
        {
            if (weaponLookup.TryGetValue(savedWeapon.weaponId, out Weapon weapon))
            {
                if (weapon is RangedWeapon ranged)
                    ranged.SetAmmo(savedWeapon.magazineAmmo);
            }
            else
            {
                Debug.LogWarning(
                    "Saved weapon not found on Player: " + savedWeapon.weaponId
                );
            }
        }

        // Restore hotbar order
        List<Weapon> restoredHotbar = new();

        foreach (string weaponId in data.hotbarWeaponIds)
        {
            if (weaponLookup.TryGetValue(weaponId, out Weapon weapon))
            {
                restoredHotbar.Add(weapon);
            }
        }

        manager.SetHotbar(restoredHotbar, data.selectedWeaponIndex);

        Debug.Log("Weapon data restored!");
        return true;
    }


    // Clear pending data after the player is restored
    public void ClearPendingLoad()
    {
        PendingLoadData = null;
    }

    // Delete saved run
    public void DeleteSavedRun()
    {
        try
        {
            if (HasSavedRun)
            {
                File.Delete(SavePath);
                Debug.Log("Saved run deleted");
            }
        }
        catch (Exception error)
        {
            Debug.LogError("Failed to delete saved run: " + error.Message);
        }
    }
}
