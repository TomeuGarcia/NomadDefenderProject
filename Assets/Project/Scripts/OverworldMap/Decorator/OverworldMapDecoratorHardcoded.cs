using NodeEnums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldMapDecoratorHardcoded : OverworldMapDecorator
{
    [Header("HARDCODED DATA")]
    [SerializeField] private EmptyLevelData[] emptyLevels;
    [SerializeField] private BattleLevelData[] battleLevels;
    [SerializeField] private UpgradeLevelData[] upgradeLevels;


    private class LevelData
    {
        [SerializeField] public int levelI;
        [SerializeField] public int nodeI;
        [SerializeField] public NodeEnums.ProgressionState progressionState;
    }

    [System.Serializable]
    private class EmptyLevelData : LevelData
    {
        [SerializeField] public NodeEnums.EmptyType emptyType;
    }

    [System.Serializable]
    private class BattleLevelData : LevelData
    {
        [SerializeField] public NodeEnums.BattleType battleType;
        [SerializeField] public int numNextLevelNodes;
    }

    [System.Serializable]
    private class UpgradeLevelData : LevelData
    {
        [SerializeField] public NodeEnums.UpgradeType upgradeType;
        [SerializeField] public int numNextLevelNodes;
    }


    public override void DecorateMap(OWMap_Node[][] mapNodes)
    {
        DecorateHardcodedEmptyLevels(mapNodes);
        DecorateHardcodedBattleLevels(mapNodes);
        DecorateHardcodedUpgradeLevels(mapNodes);
    }


    private void DecorateHardcodedEmptyLevels(OWMap_Node[][] mapNodes)
    {
        foreach (EmptyLevelData emptyLevel in emptyLevels)
        {
            if (emptyLevel.emptyType == EmptyType.FIRST_LEVEL)
            {
                DecorateFirstLevelEmpty(mapNodes[emptyLevel.levelI]);
            }
            else if (emptyLevel.emptyType == EmptyType.LAST_LEVEL)
            {
                DecorateLastLevelEmpty(mapNodes[emptyLevel.levelI]);
            }
        }
    }

    private void DecorateHardcodedBattleLevels(OWMap_Node[][] mapNodes)
    {
        foreach (BattleLevelData battleLevel in battleLevels)
        {
            DecorateBattleNode(mapNodes[battleLevel.levelI][battleLevel.nodeI], battleLevel.numNextLevelNodes, battleLevel.battleType, battleLevel.progressionState);
        }
    }

    private void DecorateHardcodedUpgradeLevels(OWMap_Node[][] mapNodes)
    {
        foreach (UpgradeLevelData upgradeLevel in upgradeLevels)
        {
            DecorateUpgradeNode(mapNodes[upgradeLevel.levelI][upgradeLevel.nodeI], upgradeLevel.numNextLevelNodes, upgradeLevel.nodeI, mapNodes[upgradeLevel.levelI].Length, 
                upgradeLevel.upgradeType, upgradeLevel.progressionState);
        }
    }

}
