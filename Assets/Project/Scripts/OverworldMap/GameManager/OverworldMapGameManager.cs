using UnityEngine;
using static MapData;
using static OWMap_Node;

public class OverworldMapGameManager : MonoBehaviour
{
    private OWMap_Node[][] mapNodes;
    public OWMap_Node[][] GetMapNodes() { return mapNodes; }

    [Header("CREATOR & DECORATOR")]
    [SerializeField] private OverworldMapCreator owMapCreator;
    [SerializeField] private OverworldMapDecorator owMapDecorator;
    [SerializeField] private UpgradeSceneSetupInfo upgradeSceneSetupInfo;

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

    [Header("Tutorial")]
    [SerializeField] private OWMapTutorialManager owMapTutorial;



    private OWMap_Node currentNode;

    private bool moveCameraAfterNodeScene;


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
        mapSceneLoader.OnSceneFromMapLoaded += DoOnSceneFromMapLoaded;
        mapSceneLoader.OnSceneFromMapUnloaded += DoOnSceneFromMapUnloaded;
    }

    private void OnDisable()
    {
        MapSceneNotifier.OnMapSceneFinished -= ResumeMapAfterNodeScene;
        mapSceneLoader.OnSceneFromMapLoaded -= DoOnSceneFromMapLoaded;
        mapSceneLoader.OnSceneFromMapUnloaded -= DoOnSceneFromMapUnloaded;
    }


    private void Start()
    {
        owMapCreator.RegenerateMap(out mapNodes);
        owMapDecorator.DecorateMap(mapNodes);

        mapSceneLoader.Init();
        if(!TutorialsSaverLoader.GetInstance().IsTutorialDone(Tutorials.BATTLE))
        {
            mapSceneLoader.LoadTutorialScene();
            moveCameraAfterNodeScene = false;
            //Do OWMapTutorial

            mapSceneLoader.OnSceneFromMapUnloaded += StartMapTutorial;
        }
        else
        {
            moveCameraAfterNodeScene = true;
            if (!TutorialsSaverLoader.GetInstance().IsTutorialDone(Tutorials.OW_MAP))
                StartMapTutorial();
        }

        StartAtFirstLevel();

        if (TutorialsSaverLoader.GetInstance().IsTutorialDone(Tutorials.OW_MAP))
            StartCommunicationWithNextNodes(currentNode);

    }

    private void StartMapTutorial()
    {
        mapSceneLoader.OnSceneFromMapUnloaded -= StartMapTutorial;
        owMapTutorial.StartTutorial();
    }


    private void StartAtFirstLevel()
    {
        currentNode = mapNodes[0][0];

        currentNode.SetOwMapGameManagerRef(this);
        currentNode.SetSelected(); // Simulate node is clicked

        owMapPawn.Init(this, currentNode, owMapCreator.DisplacementBetweenLevels);        

    }

    public void OnMapNodeSelected(OWMap_Node owMapNode)
    {
        owMapPawn.MoveToNode(owMapNode);
    }


    public void OnOwMapPawnReachedNode(OWMap_Node reachedNode)
    {
        bool cameFromNodeWasBattle = IsCurrentNodeBattle();

        this.currentNode = reachedNode;

        if (currentNode.GetNodeType() == NodeEnums.NodeType.NONE)
        {
            ResumeMap(); // Node is empty, so skip scene (example: first map node)
        }
        else
        {
            if (IsCurrentNodeUpgrade())
            {
                if (cameFromNodeWasBattle)
                {
                    upgradeSceneSetupInfo.SetData(currentNode.nodeClass.progressionState, currentNode.healthState, currentBattleStateResult.DidWinWithPerfectDefense());
                }
                else
                {
                    upgradeSceneSetupInfo.ResetDataAsPredefined(currentNode.nodeClass.progressionState);
                }
            }

            currentNode.nodeClass.StartLevel(this);
        }      
        
    }

    private void ResumeMapAfterNodeScene() // called from event
    {

        FinishCurrentMapLevelScene();
        ResumeMap();
        if (moveCameraAfterNodeScene)
        {
            owMapPawn.MoveCameraToNextLevel();
            
        }
        moveCameraAfterNodeScene = true;
    }
    private void ResumeMap()
    {
        // If current node was BATTLE, apply BattleStateResult
        if (IsCurrentNodeBattle())
            ApplyBattleStateResult();

        if (TutorialsSaverLoader.GetInstance().IsTutorialDone(Tutorials.OW_MAP))
            StartCommunicationWithNextNodes(currentNode);
    }



    public void StartCommunicationWithNextNodes(OWMap_Node owMapNode)
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
    private bool IsCurrentNodeUpgrade()
    {
        return currentNode.GetNodeType() == NodeEnums.NodeType.UPGRADE;
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


    private void DoOnSceneFromMapUnloaded()
    {
        owMapPawn.ActivateCamera();

        cardDisplayer.ResetAll();
        cardShower.SetActive(true);
    }
    private void DoOnSceneFromMapLoaded()
    {
        owMapPawn.DeactivateCamera();

        if (IsCurrentNodeUpgrade())
            Debug.Log("entered upgrade scene");

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


    public OWMap_Node GetCurrentNode()
    {
        return currentNode;
    }
}
