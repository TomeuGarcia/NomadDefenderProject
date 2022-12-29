using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

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
        if (dSettings.lastNodeIsBattle)
        {
            --lastIteratedLevel;
            DecorateBattleLevel(mapNodes[mapNodes.Length - 1]);
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
        firstLevel[0].SetGameState(OWMap_Node.NodeGameState.UNDAMAGED); // TEST
    }

    private void DecorateBattleLevel(OWMap_Node[] battleLevel)
    {
        for (int nodeI = 0; nodeI < battleLevel.Length; ++nodeI)
        {
            // TODO mapNodes[levelI][nodeI]
            battleLevel[nodeI].SetGameState(OWMap_Node.NodeGameState.DESTROYED); // TEST
        }
    }

    private void DecorateUpgradeLevel(OWMap_Node[] upgradeLevel)
    {
        for (int nodeI = 0; nodeI < upgradeLevel.Length; ++nodeI)
        {
            // TODO mapNodes[levelI][nodeI]
            upgradeLevel[nodeI].SetGameState(OWMap_Node.NodeGameState.SLIGHTLY_DAMAGED); // TEST
        }
    }

    private bool IsBattleLevel(int levelI)
    {
        return (levelI - firstBattleLevel) % dSettings.battleAfterNLevels == 0;
    }


}
