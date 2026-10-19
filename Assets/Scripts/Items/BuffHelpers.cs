using System;
using System.Collections.Generic;
using System.Linq;

public enum BuffType
{
    NONE,
    FIRE_RATE,
    MAX_AMMO,
    COOLDOWN,
    DAMAGE,
    MAX_HEALTH,
    DAMAGE_RESIST,
    MOVE_SPEED
}

public class BuffHelpers
{
    private static readonly Dictionary<BuffType, float> buffAmountMap = new() {
        { BuffType.FIRE_RATE, 1.5f },
        { BuffType.MAX_AMMO, 3.0f },
        { BuffType.COOLDOWN, 0.5f },
        { BuffType.DAMAGE, 3.0f },
        { BuffType.MAX_HEALTH, 15.0f },
        { BuffType.DAMAGE_RESIST, 0.5f },
        { BuffType.MOVE_SPEED, 0.1f }
    };
    
    private static readonly Dictionary<BuffType, uint> buffLikelihoodMap = new() {
        { BuffType.FIRE_RATE, 2 },
        { BuffType.MAX_AMMO, 2 },
        { BuffType.COOLDOWN, 2 },
        { BuffType.DAMAGE, 2 },
        { BuffType.MAX_HEALTH, 4 },
        { BuffType.DAMAGE_RESIST, 2 },
        { BuffType.MOVE_SPEED, 1 },
    };

    private static readonly BuffType[] buffList = buffLikelihoodMap.Keys.ToArray();
    private static readonly uint[] buffWeights = buffLikelihoodMap.Values.ToArray();
    private static BuffType lastBuff = BuffType.NONE;

    private static readonly int secondsPerPowerLevel = 300; // increase buffs' strengh every five minutes

    public static int GetRandWeighted(Random randGen, uint[] weights, int biasedAgainst)
    {
        if (biasedAgainst >= weights.Length)
        {
            throw new ArgumentOutOfRangeException($"Invalid bias index {biasedAgainst}");
        }

        // sum weights
        uint weightSum = 0;
        for (int i = 0; i < weights.Length; ++i)
        {
            weightSum += weights[i];
        }

        // double the weight of each index except for biased index, if one was given
        if (biasedAgainst >= 0)
        {
            weightSum = 2 * weightSum - weights[biasedAgainst];
        }

        // make a selection from 0 to (weight sum - 1)
        uint selection = (uint)randGen.Next(0, (int)weightSum);

        // parse through weights array until index corresponding to selection is found
        int index = 0;
        while (selection > weights[index])
        {
            selection -= weights[index];
            index++;
        }

        return index;
    }

    private static BuffType GetNextBuff(Random randGen, BuffType[] buffList, uint[] buffWeights, ref BuffType lastBuff)
    {
        int nextBuffIndex = GetRandWeighted(randGen, buffWeights, Array.IndexOf(buffList, lastBuff));
        lastBuff = buffList[nextBuffIndex];
        return lastBuff;
    }

    public static BuffType GetNextBuffManaged(Random randGen)
    {
        return GetNextBuff(randGen, buffList, buffWeights, ref lastBuff);
    }

    private static float GetBuffAmount(BuffType buff, int powerLevel)
    {
        if (buffAmountMap.TryGetValue(buff, out float amount))
        {
            return amount * powerLevel;
        }

        return 0.0f;
    }

    public static float GetBuffAmountManaged(BuffType buff)
    { 
        return GetBuffAmount(buff, (int)(UnityEngine.Time.timeSinceLevelLoad / secondsPerPowerLevel));
    }
}