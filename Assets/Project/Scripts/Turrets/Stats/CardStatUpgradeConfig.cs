using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardStatUpgradeConfig_NAME",
    menuName = SOAssetPaths.CARDS_STATS + "CardStatUpgradeConfig")]
public class CardStatUpgradeConfig : ScriptableObject
{
    [SerializeField, Min(0)] private float _baseMultiplier = 1.2f;
    [SerializeField, Min(0)] private float _multiplierIncrementPerLevel = 0.2f;

    public float ComputeValue(float baseValue, int level)
    {
        return level < 1 
            ? baseValue
            : baseValue * (_baseMultiplier + (_multiplierIncrementPerLevel * (level - 1)));
    }
}
