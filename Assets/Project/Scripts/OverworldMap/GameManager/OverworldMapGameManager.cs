using UnityEngine;
using static MapData;
using static OWMap_Node;

public class OverworldMapGameManager : MonoBehaviour
{
    private OWMap_Node[][] mapNodes;

    [Header("CREATOR & DECORATOR")]
    [SerializeField] private OverworldMapCreator owMapCreator;
    [SerializeField] private OverworldMapDecorator owMapDecorator;

    [Header("PAWN")]     
    [SerializeField] private OWMapPawn owMapPawn;

    private OWMap_Node currentNode;


    // BattleStateResult
    private BattleStateResult currentBattleStateResult;


    private void Awake()
    {
        TDGameManager.OnQueryReferenceToBattleStateResult += CreateNewBattleStateResult;
    }

    private void Start()
    {
        owMapCreator.RegenerateMap(out mapNodes);
        owMapDecorator.DecorateMap(mapNodes);

        StartAtFirstLevel();
    }


    private void StartAtFirstLevel()
    {
        currentNode = mapNodes[0][0];

        currentNode.SetOwMapGameManagerRef(this);
        currentNode.SetSelected();

        owMapPawn.Init(this, currentNode);        

        StartCommunicationWithNextNodes(currentNode);
    }

    public void OnMapNodeSelected(OWMap_Node owMapNode)
    {
        owMapPawn.MoveToNode(owMapNode);
    }


    public void OnOwMapPawnReachedNode(OWMap_Node reachedNode)
    {
        this.currentNode = reachedNode;

        // TODO: start node scene here
        // TEST: for now proceed
        if (IsCurrentNodeBattle() && !currentNode.GetMapReferencesData().isLastLevelNode) // TEST
            CreateNewBattleStateResult(out currentBattleStateResult); // TEST

        ResumeMapAfterNodeScene();
    }

    private void ResumeMapAfterNodeScene()
    {
        StartCommunicationWithNextNodes(currentNode);
        UpdateNodesInteraction();

        // If current node was BATTLE, apply BattleStateResult
        if (IsCurrentNodeBattle())
            ApplyBattleStateResult();
    }

    private void StartCommunicationWithNextNodes(OWMap_Node owMapNode)
    {
        OWMap_Node.MapReferencesData nodeMapRefData = owMapNode.GetMapReferencesData();
        if (!nodeMapRefData.isLastLevelNode)
        {
            OWMap_Node[] nextLevel = mapNodes[nodeMapRefData.ownerlevelIndex + 1];

            for (int nodeI = 0; nodeI < nextLevel.Length; ++nodeI)
            {
                nextLevel[nodeI].SetOwMapGameManagerRef(this);
            }
        }
    }

    private void UpdateNodesInteraction()
    {
        OWMap_Node.MapReferencesData nodeMapRefData = currentNode.GetMapReferencesData();

        if (nodeMapRefData.isLastLevelNode)
        {
            Debug.Log("END OF MAP REACHED");
            return; // TODO do something here (END GAME)
        }


        currentNode.EnableNextLevelNodesInteraction();

        for (int i = 0; i < nodeMapRefData.nextLevelNodes.Length; ++i)
        {
            OWMap_Node nextLevelNode = nodeMapRefData.nextLevelNodes[i];
            nextLevelNode.SetHealthState(NodeEnums.HealthState.UNDAMAGED); // TODO State shouldn't be set here
        }

    }



    private void CreateNewBattleStateResult(out BattleStateResult battleStateResultRef)
    {
        OWMap_Node[] nextNodes = this.currentNode.GetMapReferencesData().nextLevelNodes;

        currentBattleStateResult = new BattleStateResult(nextNodes);

        battleStateResultRef = currentBattleStateResult;
    }

    private void ApplyBattleStateResult()
    {
        // TODO
        BattleStateResult.NodeBattleStateResult[] nodeResults = currentBattleStateResult.nodeResults;

        for (int i = 0; i < nodeResults.Length; ++i)
        {
            nodeResults[i].owMapNode.SetHealthState(nodeResults[i].healthState);
        }
    }

    private bool IsCurrentNodeBattle()
    {        
        return currentNode.nodeType == NodeEnums.NodeType.BATTLE;
    }

}
