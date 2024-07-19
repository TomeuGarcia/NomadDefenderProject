using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardStatUpgradeCostsConfig_NAME",
    menuName = SOAssetPaths.CARDS_STATS + "CardStatUpgradeCostsConfig")]
public class CardStatUpgradeCostsConfig : ScriptableObject
{
    [SerializeField, Min(0)] private int _costLevel1 = 150;
    [SerializeField, Min(0)] private int _costLevel2 = 200;
    [SerializeField, Min(0)] private int _costLevel3 = 300;

    private Dictionary<int, int> _costsMap;

    private void OnEnable()
    {
        _costsMap = new()
        {
            { 1, _costLevel1 },
            { 2, _costLevel2 },
            { 3, _costLevel3 },
        };
    }

    public int GetCostByLevel(int level)
    {
        return _costsMap[level];
    }

}
