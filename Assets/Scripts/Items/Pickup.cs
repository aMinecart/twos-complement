using UnityEngine;

public abstract class Pickup : MonoBehaviour
{
    protected virtual void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player") && collider.TryGetComponent<PlayerInventory>(out var playerInventory))
        {
            RegisterPickup(playerInventory);
            Destroy(gameObject);
        }
    }

    protected abstract void RegisterPickup(PlayerInventory inventory);
}

public class WeaponPickup : Pickup
{
    [SerializeField] protected Weapon weaponPrefab;

    protected override void RegisterPickup(PlayerInventory inventory)
    {
        // Create weapon from its prefab
        Weapon weaponFromPickup = Instantiate(weaponPrefab);
        // Add weapon to inventory without automatically equipping it 
        inventory.weaponManager.AddWeapon(weaponFromPickup);
    }
}

public class BuffPickup : Pickup
{
    [SerializeField] protected BuffType buffType;
    [SerializeField] protected float buffAmount;

    protected override void RegisterPickup(PlayerInventory inventory)
    {
        inventory.statsManager.ApplyBuff(buffType, buffAmount);
    }
}