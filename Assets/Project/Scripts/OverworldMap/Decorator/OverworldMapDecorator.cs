using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldMapDecorator : MonoBehaviour
{
    [SerializeField] OWMapDecoratorSettings dSettings;
    [SerializeField] OWMapDecoratorUtils dUtils;

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

        int lastIteratedLevel = mapNodes.Length;
        if (dSettings.battleBeforeLastNode)
        {
            lastIteratedLevel -= 2;
            DecorateBattleLevel(mapNodes[mapNodes.Length - 2], lastIteratedLevel); // Boss
            DecorateLastLevelEmpty(mapNodes[mapNodes.Length - 1]); // Finale
        }

        firstBattleLevel = firstDecoratedLevel + dSettings.numStartUpgradesLevels;
        for (int levelI = firstDecoratedLevel; levelI < firstBattleLevel; ++levelI)
        {
            DecorateUpgradeLevel(mapNodes[levelI]);
        }

        for (int levelI = firstBattleLevel; levelI < lastIteratedLevel; ++levelI)
        {
            if (IsBattleLevel(levelI))
            {
                DecorateBattleLevel(mapNodes[levelI], levelI);
            }
            else
            {
                DecorateUpgradeLevel(mapNodes[levelI]);
            }
        }

    }


    private void DecorateLastLevelEmpty(OWMap_Node[] lastLevel)
    {
        // TODO firstLevel[0]
        for (int nodeI = 0; nodeI < lastLevel.Length; ++nodeI)
        {
            lastLevel[nodeI].SetHealthState(NodeEnums.HealthState.UNDAMAGED); // TEST

            OWMap_NoneNode lastNode = new OWMap_NoneNode(0, ref lastLevel[nodeI].healthState);
            lastLevel[nodeI].SetNodeClass(lastNode, dUtils.GetEmptyNodeTexture(NodeEnums.EmptyType.LAST_LEVEL)); //Get Empty Node Texture
        }
    } 
    private void DecorateFirstLevelEmpty(OWMap_Node[] firstLevel)
    {
        // TODO firstLevel[0]
        for (int nodeI = 0; nodeI < firstLevel.Length; ++nodeI)
        {
            firstLevel[nodeI].SetHealthState(NodeEnums.HealthState.UNDAMAGED); // TEST

            int nextLevelNodes = firstLevel[nodeI].GetMapReferencesData().nextLevelNodes.Length;

            OWMap_NoneNode firstNode = new OWMap_NoneNode(nextLevelNodes, ref firstLevel[nodeI].healthState);
            firstLevel[nodeI].SetNodeClass(firstNode, dUtils.GetEmptyNodeTexture(NodeEnums.EmptyType.FIRST_LEVEL)); //Get Empty Node Texture
        }
    }

    private void DecorateBattleLevel(OWMap_Node[] battleLevel, int levelI)
    {
        for (int nodeI = 0; nodeI < battleLevel.Length; ++nodeI)
        {
            // TODO decorate properly
            battleLevel[nodeI].SetHealthState(NodeEnums.HealthState.DESTROYED); // TEST

            int nextLevelNodes = battleLevel[nodeI].GetMapReferencesData().nextLevelNodes.Length;
            NodeEnums.BattleType battleType;

            if (levelI < dSettings.lateBattleStartIndex)
            {
                battleType = NodeEnums.BattleType.EARLY;
            }
            else
            {
                battleType = NodeEnums.BattleType.LATE;
            }
            OWMap_BattleNode battleNode = new OWMap_BattleNode(nextLevelNodes, ref battleLevel[nodeI].healthState, battleType);
            battleLevel[nodeI].SetNodeClass(battleNode, dUtils.GetBattleNodeTexture(battleType));
        }
    }

    private void DecorateUpgradeLevel(OWMap_Node[] upgradeLevel)
    {
        for (int nodeI = 0; nodeI < upgradeLevel.Length; ++nodeI)
        {
            // TODO decorate properly
            upgradeLevel[nodeI].SetHealthState(NodeEnums.HealthState.SLIGHTLY_DAMAGED); //TEST


            int nextLevelNodes = upgradeLevel[nodeI].GetMapReferencesData().nextLevelNodes.Length;
            NodeEnums.UpgradeType upgradeType = (NodeEnums.UpgradeType)Random.Range(0, (int)NodeEnums.UpgradeType.COUNT);
            OWMap_UpgradeNode upgradeNodeClass = new OWMap_UpgradeNode(nextLevelNodes, ref upgradeLevel[nodeI].healthState, upgradeType);
            upgradeLevel[nodeI].SetNodeClass(upgradeNodeClass, dUtils.GetUpgradeNodeTexture(upgradeType));
        }
    }

    private bool IsBattleLevel(int levelI)
    {
        if (levelI > dSettings.lastLevelWithBattles) return false;

        return (levelI - firstBattleLevel) % dSettings.battleAfterNLevels == 0;
    }


}
