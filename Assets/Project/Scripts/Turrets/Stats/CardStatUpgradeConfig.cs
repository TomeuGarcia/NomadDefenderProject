using UnityEngine;

[CreateAssetMenu(fileName = "CardStatUpgradeConfig_NAME",
    menuName = SOAssetPaths.CARDS_STATS + "CardStatUpgradeConfig")]
public class CardStatUpgradeConfig : ScriptableObject
{
    [SerializeField, Min(0)] private float _baseMultiplier = 1.2f;

    public float ComputeValue(float baseValue, int level)
    {
        return baseValue * Mathf.Pow(_baseMultiplier, Mathf.Max(level, 0));
    }
}
