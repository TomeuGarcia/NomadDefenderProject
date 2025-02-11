using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardStatUpgradeConfig_NAME",
    menuName = SOAssetPaths.CARDS_STATS + "CardStatUpgradeConfig")]
public class CardStatUpgradeConfig : ScriptableObject
{
    private const float MINIMUM_VALUE_INCREMENT = 0.1f;

    [SerializeField] private List<float> _baseMultipliers;


    private void OnValidate()
    {
        for (int i = 0; i < _baseMultipliers.Count; ++i)
        {
            _baseMultipliers[i] = Mathf.Max(MINIMUM_VALUE_INCREMENT, _baseMultipliers[i]);
        }
        
        if (_baseMultipliers.Count < 1)
        {
            _baseMultipliers.Add(MINIMUM_VALUE_INCREMENT);
        }
    }


    public float ComputeValue(float baseValue, int level)
    {
        float accumulated = 1f;
        for (int i = 0; i < level; ++i)
        {
            int multiplierIndex = Mathf.Min(i, _baseMultipliers.Count - 1);
            accumulated *= _baseMultipliers[multiplierIndex];
        }
        
        
        float value = baseValue * accumulated;
        if (level < 1)
        {
            return value;
        }

        return Mathf.Max(value, ComputeValueMinimumIncrement(baseValue));
    }

    private float ComputeValueMinimumIncrement(float baseValue)
    {
        return baseValue + MINIMUM_VALUE_INCREMENT;
    }
}
