using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "CardStatConfig_NAME",
    menuName = SOAssetPaths.CARDS_STATS + "CardStatConfig")]
public class CardStatConfig : ScriptableObject
{
    [Header("VALUE")]
    [SerializeField, Min(0)] private float _baseValue = 5.0f;

    [Header("UPGRADE")]
    [Expandable] [SerializeField] private CardStatUpgradeConfig _upgradeConfig;
    [SerializeField, Min(0)] private int _startingLevel = 0;



    public float ComputeValueByLevel(int upgradeLevel, float baseValueBonus = 0f)
    {
        return _upgradeConfig.ComputeValue(_baseValue + baseValueBonus, _startingLevel + upgradeLevel);
    }

}
