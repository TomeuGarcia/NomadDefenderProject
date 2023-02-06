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

    [Header("CANVAS")]
    [SerializeField] private GameObject gameOverHolder;
    [SerializeField] private GameObject victoryHolder;

    [Header("DeckDisplay")]
    [SerializeField] private OverworldCardShower cardDisplayer;
    [SerializeField] private GameObject cardShower;



    private OWMap_Node currentNode;


    // BattleStateResult
    private BattleStateResult currentBattleStateResult;


    private void Awake()
    {
        TDGameManager.OnQueryReferenceToBattleStateResult += CreateNewBattleStateResult;

        gameOverHolder.SetActive(false);
        victoryHolder.SetActive(false);
    }

    private void OnEnable()
    {
        MapSceneNotifier.OnMapSceneFinished += ResumeMapAfterNodeScene;
        mapSceneLoader.OnMapSceneLoaded += DoMapSceneLoaded;
        mapSceneLoader.OnMapSceneUnloaded += DoMapSceneUnloaded;
    }

    private void OnDisable()
    {
        MapSceneNotifier.OnMapSceneFinished -= ResumeMapAfterNodeScene;
        mapSceneLoader.OnMapSceneLoaded -= DoMapSceneLoaded;
        mapSceneLoader.OnMapSceneUnloaded -= DoMapSceneUnloaded;
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
        currentNode.SetSelected(); // Simulate node is clicked

        owMapPawn.Init(this, currentNode, owMapCreator.DisplacementBetweenLevels);        

        StartCommunicationWithNextNodes(currentNode);
    }

    public void OnMapNodeSelected(OWMap_Node owMapNode)
    {
        owMapPawn.MoveToNode(owMapNode);
    }


    public void OnOwMapPawnReachedNode(OWMap_Node reachedNode)
    {
        this.currentNode = reachedNode;

        if (currentNode.GetNodeType() == NodeEnums.NodeType.NONE)
        {
            ResumeMap(); // Node is empty, so skip scene (example: first map node)
        }
        else
        {
            currentNode.nodeClass.StartLevel(this);
        }        
    }

    private void ResumeMapAfterNodeScene() // called from event
    {
        FinishCurrentMapLevelScene();
        ResumeMap();
        owMapPawn.MoveCameraToNextLevel();
    }
    private void ResumeMap()
    {
        // If current node was BATTLE, apply BattleStateResult
        if (IsCurrentNodeBattle())
            ApplyBattleStateResult();        

        StartCommunicationWithNextNodes(currentNode);
    }


    private void StartCommunicationWithNextNodes(OWMap_Node owMapNode)
    {
        OWMap_Node.MapReferencesData nodeMapRefData = owMapNode.GetMapReferencesData();

        if (nodeMapRefData.isLastLevelNode)
        {
            Debug.Log("END OF MAP REACHED, ---> VICTORY");
            StartVictory();
            return; // TODO do something here (END GAME)
        }


        OWMap_Node[] nextLevelEnabledNodes = currentNode.EnableNextLevelNodesInteraction();

        if (nextLevelEnabledNodes.Length > 0)
        {
            for (int i = 0; i < nextLevelEnabledNodes.Length; ++i)
            {
                nextLevelEnabledNodes[i].SetOwMapGameManagerRef(this);
            }
        }
        else
        {
            StartGameOver();
            Debug.Log("ALL PATHS DESTROYED, ---> GAME OVER");
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
        BattleStateResult.NodeBattleStateResult[] nodeResults = currentBattleStateResult.nodeResults;

        for (int i = 0; i < nodeResults.Length; ++i)
        {
            nodeResults[i].owMapNode.SetHealthState(nodeResults[i].healthState);
        }
    }

    private bool IsCurrentNodeBattle()
    {        
        return currentNode.GetNodeType() == NodeEnums.NodeType.BATTLE;
    }



    // SCENES
    public void StartUpgradeScene(NodeEnums.UpgradeType upgradeType, NodeEnums.HealthState nodeHealthState)
    {
        mapSceneLoader.LoadUpgradeScene(upgradeType, nodeHealthState);
    }

    public void StartBattleScene(NodeEnums.BattleType battleType, int numLocationsToDefend)
    {
        mapSceneLoader.LoadBattleScene(battleType, numLocationsToDefend);
    }

    private void FinishCurrentMapLevelScene()
    {
        mapSceneLoader.FinishCurrentScene();
    }


    private void DoMapSceneUnloaded()
    {
        owMapPawn.ActivateCamera();
        cardDisplayer.ResetAll();
        cardShower.SetActive(true);
    }
    private void DoMapSceneLoaded()
    {
        owMapPawn.DeactivateCamera();
        cardDisplayer.DestroyAllCards();
        cardShower.SetActive(false);
    }


    private void StartGameOver()
    {
        gameOverHolder.SetActive(true);
        mapSceneLoader.LoadMainMenuScene(5f);
        
    }

    private void StartVictory()
    {
        victoryHolder.SetActive(true);
        mapSceneLoader.LoadMainMenuScene(5f);
    }



}
