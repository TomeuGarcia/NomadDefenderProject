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
        base.DecorateMap(mapNodes);
        
        //DecorateHardcodedEmptyLevels(mapNodes);
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
            OWMap_Node node = mapNodes[battleLevel.levelI][battleLevel.nodeI];
            DestroyNodeAdditions(node);
            DecorateBattleNode(node, battleLevel.numNextLevelNodes, battleLevel.nodeI, mapNodes[battleLevel.levelI].Length,
                battleLevel.battleType, battleLevel.progressionState);
        }
    }

    private void DecorateHardcodedUpgradeLevels(OWMap_Node[][] mapNodes)
    {
        foreach (UpgradeLevelData upgradeLevel in upgradeLevels)
        {
            OWMap_Node node = mapNodes[upgradeLevel.levelI][upgradeLevel.nodeI];
            DestroyNodeAdditions(node);
            DecorateUpgradeNode(node, upgradeLevel.numNextLevelNodes, upgradeLevel.nodeI, mapNodes[upgradeLevel.levelI].Length, 
                upgradeLevel.upgradeType, upgradeLevel.progressionState);
        }
    }

    private void DestroyNodeAdditions(OWMap_Node node)
    {
        for (int i = node.NodeAdditionsTransform.childCount - 1; i >= 0; --i)
        {
            Destroy(node.NodeAdditionsTransform.GetChild(i).gameObject);
        }
    }
}
