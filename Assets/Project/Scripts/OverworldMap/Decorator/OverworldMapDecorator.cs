using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            DecorateUpgradeLevel(mapNodes[levelI], levelI);
        }

        for (int levelI = firstBattleLevel; levelI < lastIteratedLevel; ++levelI)
        {
            if (IsBattleLevel(levelI))
            {
                DecorateBattleLevel(mapNodes[levelI], levelI);
            }
            else
            {
                DecorateUpgradeLevel(mapNodes[levelI], levelI);
            }
        }

    }


    private void DecorateLastLevelEmpty(OWMap_Node[] lastLevel)
    {
        // TODO firstLevel[0]
        for (int nodeI = 0; nodeI < lastLevel.Length; ++nodeI)
        {
            lastLevel[nodeI].SetBorderColor(OWMap_Node.blueColor);

            OWMap_NoneNode lastNode = new OWMap_NoneNode(0, ref lastLevel[nodeI].healthState, NodeEnums.ProgressionState.LATE);
            lastLevel[nodeI].SetNodeClass(lastNode, dUtils.GetEmptyNodeTexture(NodeEnums.EmptyType.LAST_LEVEL)); //Get Empty Node Texture
        }
    } 
    private void DecorateFirstLevelEmpty(OWMap_Node[] firstLevel)
    {
        // TODO firstLevel[0]
        for (int nodeI = 0; nodeI < firstLevel.Length; ++nodeI)
        {
            firstLevel[nodeI].SetBorderColor(OWMap_Node.blueColor);

            int nextLevelNodes = firstLevel[nodeI].GetMapReferencesData().nextLevelNodes.Length;

            OWMap_NoneNode firstNode = new OWMap_NoneNode(nextLevelNodes, ref firstLevel[nodeI].healthState, NodeEnums.ProgressionState.EARLY);
            firstLevel[nodeI].SetNodeClass(firstNode, dUtils.GetEmptyNodeTexture(NodeEnums.EmptyType.FIRST_LEVEL)); //Get Empty Node Texture
        }
    }

    private void DecorateBattleLevel(OWMap_Node[] battleLevel, int levelI)
    {
        for (int nodeI = 0; nodeI < battleLevel.Length; ++nodeI)
        {
            // TODO decorate properly
            battleLevel[nodeI].SetBorderColor(OWMap_Node.redColor);

            int nextLevelNodes = battleLevel[nodeI].GetMapReferencesData().nextLevelNodes.Length;
            NodeEnums.BattleType battleType;
            NodeEnums.ProgressionState progressionState;

            if (levelI < dSettings.midBattleStartIndex)
            {
                battleType = NodeEnums.BattleType.EARLY;
                progressionState = NodeEnums.ProgressionState.EARLY;
            }
            else if (levelI < dSettings.lateBattleStartIndex)
            {
                battleType = NodeEnums.BattleType.MID;
                progressionState = NodeEnums.ProgressionState.MID;
            }
            else
            {
                battleType = NodeEnums.BattleType.LATE;
                progressionState = NodeEnums.ProgressionState.LATE;
            }

            OWMap_BattleNode battleNode = new OWMap_BattleNode(nextLevelNodes, ref battleLevel[nodeI].healthState, battleType, progressionState);
            battleLevel[nodeI].SetNodeClass(battleNode, dUtils.GetBattleNodeTexture(battleType));
        }
    }

    private void DecorateUpgradeLevel(OWMap_Node[] upgradeLevel, int levelI)
    {
        HashSet<NodeEnums.UpgradeType> randomUpgradeTypes = new HashSet<NodeEnums.UpgradeType>();
        while (randomUpgradeTypes.Count < upgradeLevel.Length) // UNSAFE this could explode in theory
        {
            randomUpgradeTypes.Add((NodeEnums.UpgradeType)Random.Range(0, (int)NodeEnums.UpgradeType.COUNT));
        }
        NodeEnums.UpgradeType[] randomUpgradeTypesArray = randomUpgradeTypes.ToArray();

        for (int nodeI = 0; nodeI < upgradeLevel.Length; ++nodeI)
        {
            // TODO decorate properly
            upgradeLevel[nodeI].SetBorderColor(OWMap_Node.blueColor);

            int nextLevelNodes = upgradeLevel[nodeI].GetMapReferencesData().nextLevelNodes.Length;
            NodeEnums.UpgradeType upgradeType = randomUpgradeTypesArray[nodeI];

            NodeEnums.ProgressionState progressionState;

            if (levelI < dSettings.midBattleStartIndex)
            {
                progressionState = NodeEnums.ProgressionState.EARLY;
            }
            else if (levelI < dSettings.lateBattleStartIndex)
            {
                progressionState = NodeEnums.ProgressionState.MID;
            }
            else
            {
                progressionState = NodeEnums.ProgressionState.LATE;
            }

            OWMap_UpgradeNode upgradeNodeClass = new OWMap_UpgradeNode(nextLevelNodes, ref upgradeLevel[nodeI].healthState, upgradeType, progressionState);
            upgradeLevel[nodeI].SetNodeClass(upgradeNodeClass, dUtils.GetUpgradeNodeTexture(upgradeType));
        }
    }

    private bool IsBattleLevel(int levelI)
    {
        if (levelI > dSettings.lastLevelWithBattles) return false;

        return (levelI - firstBattleLevel) % dSettings.battleAfterNLevels == 0;
    }


}
