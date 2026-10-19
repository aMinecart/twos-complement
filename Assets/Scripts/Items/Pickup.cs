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
    //private static Dictionary<BuffType, int> genRecord = Enum.GetValues(typeof(BuffType))
    //                                                         .Cast<BuffType>()
    //                                                         .ToDictionary(buff => buff, buff => 0);

    //private static readonly BuffType[] buffList = Enum.GetValues(typeof(BuffType)).Cast<BuffType>().ToArray();
    //private static readonly BuffType lastBuff = BuffType.NONE;

    [SerializeField] protected BuffType buffType;
    [SerializeField] protected float buffAmount;
    
    //public static BuffType getNextBuff(System.Random randGen)
    //{
    //    BuffType buff = buffList[randGen.Next(1, buffList.Length)];
    //    if (lastBuffs.Contains(buff))
    //}

    public static BuffPickup createRandBuffPickup(System.Random randGen, BuffPickup basePrefab, Vector3 position)
    {
        // create new buff pickup using weighted psuedo-random system
        BuffPickup pickup = Instantiate(basePrefab, position, Quaternion.identity);
        pickup.buffType = BuffHelpers.GetNextBuffManaged(randGen);
        pickup.buffAmount = BuffHelpers.GetBuffAmountManaged(pickup.buffType);

        return pickup;
    }

    protected override void RegisterPickup(PlayerInventory inventory)
    {
        inventory.statsManager.ApplyBuff(buffType, buffAmount);
    }
}