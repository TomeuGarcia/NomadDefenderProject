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
    [SerializeField] protected MapSceneLoader mapSceneLoader;

    [Header("PAWN")]     
    [SerializeField] private OWMapPawn owMapPawn;

    [Header("DECK DISPLAY")]
    [SerializeField] private OverworldCardShower cardDisplayer;

    protected OWMap_Node currentNode;
    protected bool moveCameraAfterNodeScene;

    // BattleStateResult
    protected BattleStateResult currentBattleStateResult;


    public delegate void OverworldMapGameManagerAction();
    public event OverworldMapGameManagerAction OnVictory; // Event to be invoked when player reaches the end of the map
    public event OverworldMapGameManagerAction OnGameOver; // Event to be invoked when all available paths are destroyed
    public event OverworldMapGameManagerAction OnMapNodeSceneStartsLoading; // For now only used to clear console in tutorial



    private void OnEnable()
    {
        MapSceneNotifier.OnMapSceneFinished += ResumeMapAfterNodeScene;
        mapSceneLoader.OnSceneFromMapLoaded += DoOnSceneFromMapLoaded;
        mapSceneLoader.OnSceneFromMapUnloaded += DoOnSceneFromMapUnloaded;

        TDGameManager.OnQueryReferenceToBattleStateResult += CreateNewBattleStateResult;
    }

    private void OnDisable()
    {
        MapSceneNotifier.OnMapSceneFinished -= ResumeMapAfterNodeScene;
        mapSceneLoader.OnSceneFromMapLoaded -= DoOnSceneFromMapLoaded;
        mapSceneLoader.OnSceneFromMapUnloaded -= DoOnSceneFromMapUnloaded;

        TDGameManager.OnQueryReferenceToBattleStateResult -= CreateNewBattleStateResult;
    }


    private void Start()
    {
        Init();
    }

    protected virtual void Init()
    {
        InitMapGeneration();
        InitMapSceneLoader();
        StartAtFirstLevel();
        StartCommunicationWithNextNodes(currentNode);

        moveCameraAfterNodeScene = true;
    }

    protected void InitMapGeneration()
    {
        owMapCreator.RegenerateMap(out mapNodes);
        owMapDecorator.DecorateMap(mapNodes);
    }
    protected void InitMapSceneLoader()
    {
        mapSceneLoader.Init();
    }


    protected void StartAtFirstLevel()
    {
        currentNode = mapNodes[0][0];

        currentNode.SetOwMapGameManagerRef(this);
        currentNode.SetSelected(false); // Simulate node is clicked

        owMapPawn.Init(this, currentNode, owMapCreator.DisplacementBetweenLevels);        

    }

    public void OnMapNodeSelected(OWMap_Node owMapNode, bool wasSelectedByPlayer)
    {
        owMapPawn.MoveToNode(owMapNode);
        
        if (wasSelectedByPlayer)
        {
            GameAudioManager.GetInstance().PlayNodeSelectedSound();
        }
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
        else if (!currentNode.GetMapReferencesData().isLastLevelNode)
        {
            // Check if next nodes are upgardes that come after non-battle nodes 
            OWMap_Node[] nextLevelNodes = currentNode.GetMapReferencesData().nextLevelNodes;
            if (nextLevelNodes[0].GetNodeType() == NodeEnums.NodeType.UPGRADE)
            {
                foreach (OWMap_Node node in nextLevelNodes)
                {
                    node.SetHealthState(NodeEnums.HealthState.UNDAMAGED, false);
                }
            }
        }

        if (TutorialsSaverLoader.GetInstance().IsTutorialDone(Tutorials.OW_MAP))
            StartCommunicationWithNextNodes(currentNode);
    }



    public void StartCommunicationWithNextNodes(OWMap_Node owMapNode)
    {
        OWMap_Node.MapReferencesData nodeMapRefData = owMapNode.GetMapReferencesData();

        if (nodeMapRefData.isLastLevelNode)
        {
            Debug.Log("END OF MAP REACHED ---> VICTORY");
            InvokeOnVictory();
            return;
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
            InvokeOnGameOver();
            Debug.Log("ALL PATHS DESTROYED ---> GAME OVER");
        }

    }


    protected void CreateNewBattleStateResult(out BattleStateResult battleStateResultRef)
    {
        OWMap_Node[] nextNodes = this.currentNode.GetMapReferencesData().nextLevelNodes;

        currentBattleStateResult = new BattleStateResult(nextNodes);

        battleStateResultRef = currentBattleStateResult;
    }

    protected virtual void ApplyBattleStateResult()
    {
        BattleStateResult.NodeBattleStateResult[] nodeResults = currentBattleStateResult.nodeResults;
        bool wonWithPerfectDefense = currentBattleStateResult.DidWinWithPerfectDefense();

        for (int i = 0; i < nodeResults.Length; ++i)
        {
            nodeResults[i].owMapNode.SetHealthState(nodeResults[i].healthState, wonWithPerfectDefense);
        }
    }

    protected bool IsCurrentNodeBattle()
    {        
        return currentNode.GetNodeType() == NodeEnums.NodeType.BATTLE;
    }
    private bool IsCurrentNodeUpgrade()
    {
        return currentNode.GetNodeType() == NodeEnums.NodeType.UPGRADE;
    }
    public OWMap_Node GetCurrentNode()
    {
        return currentNode;
    }


    // SCENES
    public void StartUpgradeScene(NodeEnums.UpgradeType upgradeType, NodeEnums.HealthState nodeHealthState)
    {        
        mapSceneLoader.LoadUpgradeScene(upgradeType, nodeHealthState);
        if (OnMapNodeSceneStartsLoading != null) OnMapNodeSceneStartsLoading();
    }

    public void StartBattleScene(NodeEnums.BattleType battleType, int numLocationsToDefend)
    {
        mapSceneLoader.LoadBattleScene(battleType, numLocationsToDefend);
        if (OnMapNodeSceneStartsLoading != null) OnMapNodeSceneStartsLoading();
    }

    private void FinishCurrentMapLevelScene()
    {
        mapSceneLoader.FinishCurrentScene();
    }


    private void DoOnSceneFromMapUnloaded()
    {
        owMapPawn.ActivateCamera();

        cardDisplayer.ResetAll();
        cardDisplayer.gameObject.SetActive(true);

        EnemyFactory.GetInstance().ResetPools();
        ProjectileParticleFactory.GetInstance().ResetPools();
    }
    private void DoOnSceneFromMapLoaded()
    {
        owMapPawn.DeactivateCamera();

        cardDisplayer.DestroyAllCards();
        cardDisplayer.gameObject.SetActive(false);
    }



    private void InvokeOnGameOver()
    {
        if (OnGameOver != null) OnGameOver();
    }

    private void InvokeOnVictory()
    {
        if (OnVictory != null) OnVictory();
    }

}
