using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialOverworldMapGameManager : OverworldMapGameManager
{
    [Header("TUTORIAL")]
    [SerializeField] protected OWMapTutorialManager owMapTutorial;
    [SerializeField] protected OWMapTutorialManager2 owMapTutorial2;
    bool firstBattleResultApplied = false;
    int battleCounter = 1;


    protected override void Init()
    {
        canDisplayDeck = false;

        InitMapGeneration();
        InitMapSceneLoader();

        StartAtFirstLevel();

        if (TutorialsSaverLoader.GetInstance().IsTutorialDone(Tutorials.BATTLE))
        {
            firstBattleResultApplied = true;
            moveCameraAfterNodeScene = true;
            if (!TutorialsSaverLoader.GetInstance().IsTutorialDone(Tutorials.OW_MAP))
            {
                CreateNewBattleStateResult(out currentBattleStateResult);
                StartMapTutorial();
            }
        }
        else
        {
            mapSceneLoader.LoadTutorialScene();
            moveCameraAfterNodeScene = false;
            //Do OWMapTutorial

            mapSceneLoader.OnSceneFromMapUnloaded += StartMapTutorial;
        }

        //StartAtFirstLevel();

        if (TutorialsSaverLoader.GetInstance().IsTutorialDone(Tutorials.OW_MAP))
        {
            CreateNewBattleStateResult(out currentBattleStateResult);
            StartCommunicationWithNextNodes(currentNode);
        }
    }


    private void StartMapTutorial()
    {
        mapSceneLoader.OnSceneFromMapUnloaded -= StartMapTutorial;
        owMapTutorial.StartTutorial();
        mapSceneLoader.OnSceneFromMapUnloaded += MapTutorialAfterSecondBattle;
    }


    private void MapTutorialAfterSecondBattle()
    {
        if (!IsCurrentNodeBattle()) return;
        battleCounter++;
        if (battleCounter < 3) return;

        owMapTutorial2.StartTutorial();
        mapSceneLoader.OnSceneFromMapUnloaded -= MapTutorialAfterSecondBattle;

    }
  

    protected override void ApplyBattleStateResult()
    {
        BattleStateResult.NodeBattleStateResult[] nodeResults = currentBattleStateResult.nodeResults;
        bool wonWithPerfectDefense = currentBattleStateResult.DidWinWithPerfectDefense();
        if (firstBattleResultApplied)
        {
            for (int i = 0; i < nodeResults.Length; ++i)
            {
                nodeResults[i].owMapNode.SetHealthState(nodeResults[i].healthState, wonWithPerfectDefense, true);
            }                        
        }
        else
        {
            firstBattleResultApplied = true;
            for (int i = 0; i < nodeResults.Length; ++i)
            {
                nodeResults[i].owMapNode.SetHealthState(NodeEnums.HealthState.UNDAMAGED, false, true); // Hardcoded to always apply UNDAMAGED
            }
        }

    }



    public override void StartCommunicationWithNextNodes(OWMap_Node owMapNode)
    {
        OWMap_Node.MapReferencesData nodeMapRefData = owMapNode.GetMapReferencesData();

        if (nodeMapRefData.isLastLevelNode)
        {
            Debug.Log("END OF MAP REACHED ---> VICTORY");
            InvokeOnVictory();
            return;
        }


        int aliveNodesCount;
        OWMap_Node[] nextLevelEnabledNodes;

        Debug.Log("battleCounter: " + battleCounter);
        if (battleCounter == 1)
        {
            nextLevelEnabledNodes = currentNode.EnableAllNextLevelNodesInteraction(out aliveNodesCount);
        }
        else
        {
            nextLevelEnabledNodes = currentNode.EnableNextLevelNodesInteraction();
            aliveNodesCount = nextLevelEnabledNodes.Length;
        }


        for (int i = 0; i < nextLevelEnabledNodes.Length; ++i)
        {
            nextLevelEnabledNodes[i].SetOwMapGameManagerRef(this);
            // TODO set node material for active interaction
        }


        if (aliveNodesCount == 0)
        {
            //InvokeOnGameOver();
            Debug.Log("ALL PATHS DESTROYED ---> GAME OVER");
            StartCoroutine(ResurrectDestroyedCurrentNode(nextLevelEnabledNodes));
        }

    }


    private IEnumerator ResurrectDestroyedCurrentNode(OWMap_Node[] nextLevelEnabledNodes)
    {
        for (int i = 0; i < nextLevelEnabledNodes.Length; ++i)
        {
            nextLevelEnabledNodes[i].isInteractable = false;
        }

        yield return new WaitForSeconds(3f);
        Debug.Log("WATCHER: what the fuck are you doing!? get good for fuck sake");
        yield return new WaitForSeconds(3f);
        Debug.Log("WATCHER: I'll let you through... only this time you fuckhead");


        for (int i = 0; i < nextLevelEnabledNodes.Length; ++i)
        {
            nextLevelEnabledNodes[i].SetHealthState(NodeEnums.HealthState.GREATLY_DAMAGED, false, true);
            nextLevelEnabledNodes[i].SetResurrectedVisuals();
            nextLevelEnabledNodes[i].ClearCameFromConnection();
            nextLevelEnabledNodes[i].isInteractable = true;
        }


        /*
        OWMap_Node[] nextLevelEnabledNodes = currentNode.EnableAllNextLevelNodesInteraction();
        for (int i = 0; i < nextLevelEnabledNodes.Length; ++i)
        {
            nextLevelEnabledNodes[i].SetOwMapGameManagerRef(this);
            nextLevelEnabledNodes[i].SetResurrectedVisuals();
            // TODO set node material for active interaction
        }
        */
    }


}
