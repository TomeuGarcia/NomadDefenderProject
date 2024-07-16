using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewOWMapDecoratorSettings", 
    menuName = SOAssetPaths.MAP_OVERWORLD + "OWMapDecoratorSettings")]
public class OWMapDecoratorSettings : ScriptableObject
{
    [SerializeField] public bool firstNodeIsEmpty = true;
    [SerializeField, Min(0)] public int numStartUpgradesLevels = 2;
    [SerializeField, Min(1)] public int[] alternateBattleAfterNLevels;
    [SerializeField, Min(0)] public int lastLevelWithBattles = 10;
    [SerializeField] public bool battleBeforeLastNode = true;
    [SerializeField, Min(0)] public int midBattleStartIndex = 5;
    [SerializeField, Min(0)] public int lateBattleStartIndex = 8;


    private void OnValidate()
    {
        if (midBattleStartIndex > lateBattleStartIndex)
        {
            ++lateBattleStartIndex;
        }
    }

}
