using UnityEngine;

public class BuffPickup : Pickup
{
    //private static Dictionary<BuffType, int> genRecord = Enum.GetValues(typeof(BuffType))
    //                                                         .Cast<BuffType>()
    //                                                         .ToDictionary(buff => buff, buff => 0);

    [SerializeField] private BuffType buffType;
    [SerializeField] private float buffAmount;

    public static BuffPickup CreateRandBuffPickup(System.Random randGen, BuffPickup basePrefab, Vector3 position)
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