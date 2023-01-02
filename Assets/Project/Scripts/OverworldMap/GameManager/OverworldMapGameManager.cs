using UnityEngine;
using static MapData;
using static OWMap_Node;

public class OverworldMapGameManager : MonoBehaviour
{
    private OWMap_Node[][] mapNodes;

    [Header("CREATOR & DECORATOR")]
    [SerializeField] private OverworldMapCreator owMapCreator;
    [SerializeField] private OverworldMapDecorator owMapDecorator;

    [Header("MAP SCENE LOADER")]
    [SerializeField] private MapSceneLoader mapSceneLoader;

    [Header("PAWN")]     
    [SerializeField] private OWMapPawn owMapPawn;

    private OWMap_Node currentNode;


    // BattleStateResult
    private BattleStateResult currentBattleStateResult;


    private void Awake()
    {
        TDGameManager.OnQueryReferenceToBattleStateResult += CreateNewBattleStateResult;
    }

    private void OnEnable()
    {
        MapSceneNotifier.OnMapSceneFinished += ResumeMapAfterNodeScene;
    }

    private void OnDisable()
    {
        MapSceneNotifier.OnMapSceneFinished -= ResumeMapAfterNodeScene;
    }


    private void Start()
    {
        owMapCreator.RegenerateMap(out mapNodes);
        owMapDecorator.DecorateMap(mapNodes);

        mapSceneLoader.Init();

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

        if (currentNode.nodeType == NodeEnums.NodeType.NONE)
        {
            ResumeMap(); // Node is empty, so skip scene (example: first map node)
        }
        else
        {
            StartCurrentNodeScene();
            //ResumeMapAfterNodeScene(); // TEST (this should be called on scene finish)
        }
    }

    private void ResumeMapAfterNodeScene() // called from event
    {
        FinishCurrentMapLevelScene();
        ResumeMap();
    }
    private void ResumeMap()
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



    // SCENES
    private void StartCurrentNodeScene()
    {
        if (currentNode.nodeType == NodeEnums.NodeType.UPGRADE)
        {
            StartUpgradeScene(NodeEnums.UpgradeType.DRAW_A_CARD, NodeEnums.HealthState.UNDAMAGED); // TEST hardcoded
        }
        else if (currentNode.nodeType == NodeEnums.NodeType.BATTLE)
        {
            StartBattleScene(NodeEnums.BattleType.EARLY, 1); // TEST hardcoded

            CreateNewBattleStateResult(out currentBattleStateResult); // TEST (remove this once scenes are working) !!!
        }
    }


    private void StartUpgradeScene(NodeEnums.UpgradeType upgradeType, NodeEnums.HealthState nodeHealthState)
    {
        mapSceneLoader.LoadUpgradeScene(upgradeType, nodeHealthState);
    }

    private void StartBattleScene(NodeEnums.BattleType battleType, int numLocationsToDefend)
    {
        mapSceneLoader.LoadBattleScene(battleType, numLocationsToDefend);
    }

    private void FinishCurrentMapLevelScene()
    {
        mapSceneLoader.FinishCurrentScene();
    }


}
