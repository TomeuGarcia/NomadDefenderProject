using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using static Cinemachine.DocumentationSortingAttribute;

public class OverworldMapDecorator : MonoBehaviour
{
    [SerializeField] OWMapDecoratorSettings dSettings;

    private int firstDecoratedLevel;
    private int firstBattleLevel;


    public void DecorateMap(OWMap_Node[][] mapNodes)
    {
        firstDecoratedLevel = 0;
        if (dSettings.firstNodeIsEmpty)
        {
            firstDecoratedLevel = 1;
            DecorateFirstLevelEmpty(mapNodes[0]);
        }


        firstBattleLevel = firstDecoratedLevel + dSettings.battleAfterNLevels;


        int lastIteratedLevel = mapNodes.Length;
        if (dSettings.battleBeforeLastNode)
        {
            lastIteratedLevel -= 2;
            DecorateBattleLevel(mapNodes[mapNodes.Length - 2]); // Boss
            DecorateUpgradeLevel(mapNodes[mapNodes.Length - 1]); // Finale
        }


        for (int levelI = firstDecoratedLevel; levelI < lastIteratedLevel; ++levelI)
        {
            if (IsBattleLevel(levelI))
            {
                DecorateBattleLevel(mapNodes[levelI]);
            }
            else
            {
                DecorateUpgradeLevel(mapNodes[levelI]);
            }
        }

    }

    private void DecorateFirstLevelEmpty(OWMap_Node[] firstLevel)
    {
        // TODO firstLevel[0]
        firstLevel[0].SetHealthState(NodeEnums.HealthState.UNDAMAGED); // TEST
    }

    private void DecorateBattleLevel(OWMap_Node[] battleLevel)
    {
        for (int nodeI = 0; nodeI < battleLevel.Length; ++nodeI)
        {
            // TODO decorate properly
            battleLevel[nodeI].SetHealthState(NodeEnums.HealthState.DESTROYED); // TEST
            battleLevel[nodeI].nodeType = NodeEnums.NodeType.BATTLE;
        }
    }

    private void DecorateUpgradeLevel(OWMap_Node[] upgradeLevel)
    {
        for (int nodeI = 0; nodeI < upgradeLevel.Length; ++nodeI)
        {
            // TODO decorate properly
            upgradeLevel[nodeI].SetHealthState(NodeEnums.HealthState.SLIGHTLY_DAMAGED); // TEST
            upgradeLevel[nodeI].nodeType = NodeEnums.NodeType.UPGRADE;
        }
    }

    private bool IsBattleLevel(int levelI)
    {
        if (levelI > dSettings.lastLevelWithBattles) return false;

        return (levelI - firstBattleLevel) % dSettings.battleAfterNLevels == 0;
    }


}
