using NodeEnums;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class OverworldMapDecorator : MonoBehaviour
{
    [Header("DECORATOR SETTINGS")]
    [SerializeField] OWMapDecoratorSettings dSettings;
    [Header("DECORATOR UTILS")]
    [SerializeField] OWMapDecoratorUtils dUtils;

    [Header("DECORATOR UTILS")]
    [SerializeField] private GameObject EmptyNodeInfoDisplayPrefab;
    [SerializeField] private GameObject BattleNodeInfoDisplayPrefab;
    [SerializeField] private GameObject UpgradeNodeInfoDisplayPrefab;

    private int firstDecoratedLevel;
    private int firstBattleLevel;

    private List<NodeEnums.UpgradeType> availableUpgradeTypes = new List<UpgradeType>();


    public virtual void DecorateMap(OWMap_Node[][] mapNodes)
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


    protected void DecorateLastLevelEmpty(OWMap_Node[] lastLevel)
    {
        // TODO firstLevel[0]
        for (int nodeI = 0; nodeI < lastLevel.Length; ++nodeI)
        {
            OWMap_Node node = lastLevel[nodeI];

            lastLevel[nodeI].SetBorderColor(OWMapDecoratorUtils.s_blueColor);

            OWMap_NoneNode lastNode = new OWMap_NoneNode(0, ref lastLevel[nodeI].healthState, NodeEnums.ProgressionState.LATE);
            lastLevel[nodeI].SetNodeClass(lastNode, dUtils.GetEmptyNodeTexture(NodeEnums.EmptyType.LAST_LEVEL)); //Get Empty Node Texture

            // Spawn EmptyNodeInfoDisplay
            OWMap_EmptyNodeInfoDisplay emptyNodeInfoDisplay = Instantiate(EmptyNodeInfoDisplayPrefab, node.NodeAdditionsTransform).GetComponent<OWMap_EmptyNodeInfoDisplay>();
            emptyNodeInfoDisplay.Init(node, node.MouseOverNotifier, PositionAtRight(nodeI, lastLevel.Length), NodeEnums.EmptyType.LAST_LEVEL);
            emptyNodeInfoDisplay.SetIsInteractableFalse();
        }
    }
    protected void DecorateFirstLevelEmpty(OWMap_Node[] firstLevel)
    {
        // TODO firstLevel[0]
        for (int nodeI = 0; nodeI < firstLevel.Length; ++nodeI)
        {
            OWMap_Node node = firstLevel[nodeI];

            node.SetBorderColor(OWMapDecoratorUtils.s_blueColor);

            int nextLevelNodes = node.GetMapReferencesData().nextLevelNodes.Length;

            OWMap_NoneNode firstNode = new OWMap_NoneNode(nextLevelNodes, ref firstLevel[nodeI].healthState, NodeEnums.ProgressionState.EARLY);
            node.SetNodeClass(firstNode, dUtils.GetEmptyNodeTexture(NodeEnums.EmptyType.FIRST_LEVEL)); //Get Empty Node Texture

            // Spawn EmptyNodeInfoDisplay
            OWMap_EmptyNodeInfoDisplay emptyNodeInfoDisplay = Instantiate(EmptyNodeInfoDisplayPrefab, node.NodeAdditionsTransform).GetComponent<OWMap_EmptyNodeInfoDisplay>();
            emptyNodeInfoDisplay.Init(node, node.MouseOverNotifier, PositionAtRight(nodeI, firstLevel.Length), NodeEnums.EmptyType.FIRST_LEVEL);
            emptyNodeInfoDisplay.SetIsInteractableFalse();
        }
    }

    private void DecorateBattleLevel(OWMap_Node[] battleLevel, int levelI)
    {
        for (int nodeI = 0; nodeI < battleLevel.Length; ++nodeI)
        {
            int nextLevelNodes = battleLevel[nodeI].GetMapReferencesData().nextLevelNodes.Length;
            NodeEnums.BattleType battleType = GetLevelBattleType(levelI);
            NodeEnums.ProgressionState progressionState = GetLevelProgressionState(levelI);

            DecorateBattleNode(battleLevel[nodeI], nextLevelNodes, nodeI, battleLevel.Length, battleType, progressionState);
        }
    }
    protected void DecorateBattleNode(OWMap_Node node, int nextLevelNodes, int nodeIndexInLevel, int totalNodesInLevel, 
        NodeEnums.BattleType battleType, NodeEnums.ProgressionState progressionState)
    {
        // TODO decorate properly
        node.SetBorderColor(OWMapDecoratorUtils.s_orangeColor);

        OWMap_BattleNode battleNode = new OWMap_BattleNode(nextLevelNodes, ref node.healthState, battleType, progressionState);
        node.SetNodeClass(battleNode, dUtils.GetBattleNodeTexture(battleType));

        // Spawn BattleNodeInfoDisplay
        OWMap_BattleNodeInfoDisplay battleNodeInfoDisplay = Instantiate(BattleNodeInfoDisplayPrefab, node.NodeAdditionsTransform).GetComponent<OWMap_BattleNodeInfoDisplay>();
        battleNodeInfoDisplay.Init(node, node.MouseOverNotifier, PositionAtRight(nodeIndexInLevel, totalNodesInLevel), battleType);
        battleNodeInfoDisplay.SetIsInteractableFalse();
    }

    private void DecorateUpgradeLevel(OWMap_Node[] upgradeLevel, int levelI)
    {
        List<NodeEnums.UpgradeType> upgradesAlreadyInLevel = new List<UpgradeType>();

        for (int nodeI = 0; nodeI < upgradeLevel.Length; ++nodeI)
        {
            int nextLevelNodes = upgradeLevel[nodeI].GetMapReferencesData().nextLevelNodes.Length;

            if (NoAvailableUpgradeTypesLeft()) ResetAvailableUpgradeTypes();
            NodeEnums.UpgradeType upgradeType = GetRandomAvailableUpgradeType(upgradesAlreadyInLevel.ToArray());
            upgradesAlreadyInLevel.Add(upgradeType);

            NodeEnums.ProgressionState progressionState = GetLevelProgressionState(levelI);

            DecorateUpgradeNode(upgradeLevel[nodeI], nextLevelNodes, nodeI, upgradeLevel.Length, upgradeType, progressionState);
        }
    }
    protected void DecorateUpgradeNode(OWMap_Node node, int nextLevelNodes, int nodeIndexInLevel, int totalNodesInLevel, 
        NodeEnums.UpgradeType upgradeType, NodeEnums.ProgressionState progressionState)
    {
        // TODO decorate properly
        node.SetBorderColor(OWMapDecoratorUtils.s_blueColor);

        OWMap_UpgradeNode upgradeNodeClass = new OWMap_UpgradeNode(nextLevelNodes, ref node.healthState, upgradeType, progressionState);
        node.SetNodeClass(upgradeNodeClass, dUtils.GetUpgradeNodeTexture(upgradeType));

        // Spawn UpgradeNodeInfoDisplay
        OWMap_UpgradeNodeInfoDisplay upgradeNodeInfoDisplay = Instantiate(UpgradeNodeInfoDisplayPrefab, node.NodeAdditionsTransform).GetComponent<OWMap_UpgradeNodeInfoDisplay>();
        upgradeNodeInfoDisplay.Init(node, node.MouseOverNotifier, PositionAtRight(nodeIndexInLevel, totalNodesInLevel));
        upgradeNodeInfoDisplay.SetIsInteractableFalse();
    }

    private bool IsBattleLevel(int levelI)
    {
        if (levelI > dSettings.lastLevelWithBattles) return false;

        return (levelI - firstBattleLevel) % dSettings.battleAfterNLevels == 0;
    }

    private NodeEnums.BattleType GetLevelBattleType(int levelI)
    {
        if (levelI < dSettings.midBattleStartIndex)
        {
            return NodeEnums.BattleType.EARLY;
        }
        else if (levelI < dSettings.lateBattleStartIndex)
        {
            return NodeEnums.BattleType.MID;
        }
        else
        {
            return NodeEnums.BattleType.LATE;
        }
    }
    private NodeEnums.ProgressionState GetLevelProgressionState(int levelI)
    {
        if (levelI < dSettings.midBattleStartIndex)
        {
            return NodeEnums.ProgressionState.EARLY;
        }
        else if (levelI < dSettings.lateBattleStartIndex)
        {
            return NodeEnums.ProgressionState.MID;
        }
        else
        {
            return NodeEnums.ProgressionState.LATE;
        }
    }


    private void ResetAvailableUpgradeTypes()
    {
        for (int i = 0; i < (int)NodeEnums.UpgradeType.COUNT; ++i)
        {
            availableUpgradeTypes.Add((NodeEnums.UpgradeType)i);
        }
    }
    private bool NoAvailableUpgradeTypesLeft()
    {
        return availableUpgradeTypes.Count == 0;
    }

    private NodeEnums.UpgradeType GetRandomAvailableUpgradeType(NodeEnums.UpgradeType[] upgradesAlreadyInLevel)
    {
        int randomI = Random.Range(0, availableUpgradeTypes.Count);
        NodeEnums.UpgradeType randomUpgradeType = availableUpgradeTypes[randomI];

        int tries = 0;
        int MAX_TRIES = availableUpgradeTypes.Count;


        while (upgradesAlreadyInLevel.Contains(randomUpgradeType) && tries < MAX_TRIES)
        {
            randomI = (randomI + 1) % availableUpgradeTypes.Count;
            randomUpgradeType = availableUpgradeTypes[randomI];

            ++tries;
        }

        availableUpgradeTypes.RemoveAt(randomI);

        return randomUpgradeType;
    }

    private bool PositionAtRight(int nodeIndexInLevel, int totalNodesInLevel)
    {
        return (nodeIndexInLevel + 1f) / (float)totalNodesInLevel > 0.5f;
    }
}
