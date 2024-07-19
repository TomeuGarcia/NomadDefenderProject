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

    [Space(40)]
    [ShowNonSerializedField] private float _valueLevel1;
    [ShowNonSerializedField] private float _valueLevel2;
    [ShowNonSerializedField] private float _valueLevel3;


    private void OnValidate()
    {
        _valueLevel1 = ComputeValueByLevel(1);
        _valueLevel2 = ComputeValueByLevel(2);
        _valueLevel3 = ComputeValueByLevel(3);
    }

    public float ComputeValueByLevel(int upgradeLevel, float valueBonus = 0f)
    {
        return _upgradeConfig.ComputeValue(_baseValue, upgradeLevel) + valueBonus;
    }

}
