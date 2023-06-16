using UnityEngine;
using DG.Tweening;


public class OverworldMapGameManager : MonoBehaviour
{
    private OWMap_Node[][] mapNodes;
    public OWMap_Node[][] GetMapNodes() { return mapNodes; }
    private int currentMapLevelI;
    private bool gameFinished = false;


    [Header("CREATOR & DECORATOR")]
    [SerializeField] private OverworldMapCreator owMapCreator;
    [SerializeField] private OverworldMapGenerator owMapGenerator;
    [SerializeField] private OverworldMapDecorator owMapDecorator;
    [SerializeField] private UpgradeSceneSetupInfo upgradeSceneSetupInfo;

    [Header("MAP DATA")]
    [SerializeField] private MapData testMapData;
    [SerializeField] private MapData[] mapDataPool;
    [SerializeField] private bool useTestOverPool = false;
    [SerializeField] private bool useProceduralMap = false;

    [Header("MAP SCENE LOADER")]
    [SerializeField] protected MapSceneLoader mapSceneLoader;

    [Header("PAWN")]     
    [SerializeField] private OWMapPawn owMapPawn;

    [Header("DECK DISPLAY")]
    [SerializeField] private OverworldCardShower cardDisplayer;

    [Header("AUDIO (Ambience)")]
    [SerializeField] private AmbienceAudio ambienceAudio;

    [Header("OTHER")]
    [SerializeField] private GameObject mapEventSystemGO;

    protected bool canDisplayDeck = true;

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
        DisableCurrentLevelNodesInfoDisplay();

        moveCameraAfterNodeScene = true;

        // MAP AMBIENCE
        ambienceAudio.Play();
    }

    private MapData SetupMapData()
    {
        MapData mapData;

        if (useProceduralMap)
        {
            ////// TODO use this map data once procedural generation works 
            mapData = owMapGenerator.GenerateMap();
            //return mapData;
        }

        if (useTestOverPool)
        {
            mapData = testMapData;
        }
        else
        {
            int randomIndex = Random.Range(0, mapDataPool.Length);
            mapData = mapDataPool[randomIndex];
            //Debug.Log(randomIndex);
        }        

        return mapData;
    }

    protected void InitMapGeneration()
    {
        MapData mapData = SetupMapData();
        owMapCreator.RegenerateMap(out mapNodes, mapData);
        owMapDecorator.DecorateMap(mapNodes);
    }
    protected void InitMapSceneLoader()
    {
        mapSceneLoader.Init();
    }


    protected void StartAtFirstLevel()
    {
        currentMapLevelI = 0;
        currentNode = mapNodes[0][0];

        currentNode.SetOwMapGameManagerRef(this);
        currentNode.SetSelected(false); // Simulate node is clicked

        owMapPawn.Init(this, currentNode, OverworldMapCreator.DisplacementBetweenLevels);
    }

    public void OnMapNodeSelected(OWMap_Node owMapNode, bool wasSelectedByPlayer)
    {

        owMapPawn.MoveToNode(owMapNode);

        if (wasSelectedByPlayer)
        {
            GameAudioManager.GetInstance().PlayNodeSelectedSound();

            owMapPawn.ResetPosition();
            DisableCardDisplayer();
        }

        // scuffed camera shake :)
        //owMapPawn.FollowCameraTransform.DOPunchRotation(new Vector3(Random.Range(0.5f, 1.0f), Random.Range(0f, 0.4f), Random.Range(0f, 0.4f)) * 2f, 0.5f);
    }


    public void OnOwMapPawnReachedNode(OWMap_Node reachedNode)
    {        
        ++this.currentMapLevelI;

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
        bool isNextLevelLastLevel = currentNode.GetMapReferencesData().isLastLevelNode;

        // If current node was BATTLE, apply BattleStateResult
        if (IsCurrentNodeBattle())
        {
            ApplyBattleStateResult();
        }        
        else if (!isNextLevelLastLevel && IsNextLevelUpgrade())
        {
            ApplyBattleStateResultForUpgradeBackToBack();
        }

        if (TutorialsSaverLoader.GetInstance().IsTutorialDone(Tutorials.OW_MAP))
            StartCommunicationWithNextNodes(currentNode);


        DisableCurrentLevelNodesInfoDisplay();
    }
    
    private void DisableCurrentLevelNodesInfoDisplay()
    {
        // Disable current info displays
        foreach (OWMap_Node node in mapNodes[currentMapLevelI])
        {
            node.InvokeOnNodeInfoInteractionDisabled();
        }
    }


    public virtual void StartCommunicationWithNextNodes(OWMap_Node owMapNode)
    {
        OWMap_Node.MapReferencesData nodeMapRefData = owMapNode.GetMapReferencesData();

        if (nodeMapRefData.isLastLevelNode)
        {
            gameFinished = true;
            InvokeOnVictory();
            DisableCardDisplayer();
            Debug.Log("END OF MAP REACHED ---> VICTORY");
            return;
        }
        

        OWMap_Node[] nextLevelEnabledNodes = currentNode.EnableNextLevelNodesInteraction();

        if (nextLevelEnabledNodes.Length > 0)
        {
            for (int i = 0; i < nextLevelEnabledNodes.Length; ++i)
            {
                nextLevelEnabledNodes[i].SetOwMapGameManagerRef(this);
                // TODO set node material for active interaction
            }
        }
        else
        {
            gameFinished = true;
            InvokeOnGameOver();
            DisableCardDisplayer();
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
            nodeResults[i].owMapNode.SetHealthState(nodeResults[i].healthState, wonWithPerfectDefense, true);
            if(nodeResults[i].healthState == NodeEnums.HealthState.DESTROYED)
            {
                currentNode.GetNextLevelConnections()[i].LightConnection(true);
            }
            //currentNode.GetNextLevelConnections()[i].LightConnection(nodeResults[i].healthState == NodeEnums.HealthState.DESTROYED);
        }
    }
    private void ApplyBattleStateResultForUpgradeBackToBack()
    {
        // Check if next nodes are upgardes that come after non-battle nodes 
        OWMap_Node[] nextLevelNodes = currentNode.GetMapReferencesData().nextLevelNodes;
        foreach (OWMap_Node node in nextLevelNodes)
        {
            node.SetHealthState(NodeEnums.HealthState.UNDAMAGED, false, true);
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
    private bool IsNextLevelUpgrade()
    {
        return currentNode.GetMapReferencesData().nextLevelNodes[0].GetNodeType() == NodeEnums.NodeType.UPGRADE;
    }

    // SCENES
    public void StartUpgradeScene(NodeEnums.UpgradeType upgradeType, NodeEnums.HealthState nodeHealthState)
    {        
        mapSceneLoader.LoadUpgradeScene(upgradeType, nodeHealthState);
        if (OnMapNodeSceneStartsLoading != null) OnMapNodeSceneStartsLoading();
    }

    public void StartBattleScene(NodeEnums.BattleType battleType, int numLocationsToDefend)
    {
        GameAudioManager.GetInstance().ChangeMusic(GameAudioManager.MusicType.BATTLE, 1f);
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
        cardDisplayer.gameObject.SetActive(canDisplayDeck);

        if (gameFinished)
        {
            cardDisplayer.gameObject.SetActive(false);
        }

        mapEventSystemGO.SetActive(true);

        EnemyFactory.GetInstance().ResetPools();
        ProjectileParticleFactory.GetInstance().ResetPools();

        // MAP AMBIENCE
        ambienceAudio.Play();
    }
    private void DoOnSceneFromMapLoaded()
    {
        owMapPawn.DeactivateCamera();

        cardDisplayer.DestroyAllCards();
        if (gameFinished)
        {
            cardDisplayer.gameObject.SetActive(false);
        }

        mapEventSystemGO.SetActive(false);

        // MAP AMBIENCE
        ambienceAudio.Stop();
    }

    public void DisableCardDisplayer()
    {
        cardDisplayer.gameObject.SetActive(false);
    }


    protected void InvokeOnGameOver()
    {
        if (OnGameOver != null) OnGameOver();
    }

    protected void InvokeOnVictory()
    {
        if (OnVictory != null) OnVictory();
    }

}
