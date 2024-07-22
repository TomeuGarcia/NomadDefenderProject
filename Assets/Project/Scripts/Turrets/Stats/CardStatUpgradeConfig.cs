using UnityEngine;

[CreateAssetMenu(fileName = "CardStatUpgradeConfig_NAME",
    menuName = SOAssetPaths.CARDS_STATS + "CardStatUpgradeConfig")]
public class CardStatUpgradeConfig : ScriptableObject
{
    private const float MINIMUM_VALUE_INCREMENT = 0.1f;

    [SerializeField, Min(0)] private float _baseMultiplier = 1.2f;

    public float ComputeValue(float baseValue, int level)
    {
        float value = baseValue * Mathf.Pow(_baseMultiplier, Mathf.Max(level, 0));
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
