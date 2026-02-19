using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class GameplayConfig
{
    public float[] starThresholds;
    public int coinGiftAmount;
    public int coinGiftCountdown;

    public int CalculateStarAmount(float refCompleteTime, float realCompleteTime)
    {
        var quotient = (refCompleteTime / realCompleteTime) * 100f;

        for (int i = starThresholds.Length - 1; i >= 0; i--)
        {
            if (quotient >= starThresholds[i]) return i + 1;
        }

        return 0;
    }
}