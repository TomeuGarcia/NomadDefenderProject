using System.Collections;
using System.Collections.Generic;
using NodeEnums;
using UnityEngine;

public class TutorialOverworldMapGameManager : OverworldMapGameManager
{
    [Header("TUTORIAL")]
    [SerializeField] protected OWMapTutorialManager owMapTutorial;
    [SerializeField] protected OWMapTutorialManager2 owMapTutorial2;
    [SerializeField] protected OWMapTutorialManagerOptional owMapTutorialOptional;
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

        _overworldMapVolume.DeactivateVolume();
    }


    private void StartMapTutorial()
    {
        mapSceneLoader.OnSceneFromMapUnloaded -= StartMapTutorial;

        if (currentBattleStateResult.nodeResults[0].healthState == HealthState.DESTROYED)
        {
            firstBattleResultApplied = true;
            ApplyBattleStateResult();
            RequestGameOver();
            return;
        }
        else
        {
            owMapTutorial.StartTutorial();
            //mapSceneLoader.OnSceneFromMapUnloaded += MapTutorialAfterSecondBattle; // 2nd TUTORIAL
        }
        
    }


    private void MapTutorialAfterSecondBattle()
    {
        if (!IsCurrentNodeBattle()) return;
        battleCounter++;
        if (battleCounter < 2) return;

        owMapTutorial2.StartTutorial();
        mapSceneLoader.OnSceneFromMapUnloaded -= MapTutorialAfterSecondBattle;

    }
  

    protected override void ApplyBattleStateResult()
    {
        if (firstBattleResultApplied)
        {
            base.ApplyBattleStateResult();
        }
        else
        {
            firstBattleResultApplied = true;
            BattleStateResult.NodeBattleStateResult[] nodeResults = currentBattleStateResult.nodeResults;
            for (int i = 0; i < nodeResults.Length; ++i)
            {
                nodeResults[i].owMapNode.SetHealthState(NodeEnums.HealthState.SURVIVED, true); // Hardcoded to always apply UNDAMAGED
            }
        }

    }


    /*
    public override void StartCommunicationWithNextNodes(OWMap_Node owMapNode)
    {
        OWMap_Node.MapReferencesData nodeMapRefData = owMapNode.GetMapReferencesData();

        if (nodeMapRefData.isLastLevelNode)
        {
            InvokeOnVictory();
            return;
        }


        int aliveNodesCount;
        OWMap_Node[] nextLevelEnabledNodes;

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
            StartCoroutine(ResurrectDestroyedCurrentNode(nextLevelEnabledNodes));
        }

    }
    */


    private IEnumerator ResurrectDestroyedCurrentNode(OWMap_Node[] nextLevelEnabledNodes)
    {
        for (int i = 0; i < nextLevelEnabledNodes.Length; ++i)
        {
            nextLevelEnabledNodes[i].isInteractable = false;
        }

        yield return (StartCoroutine(owMapTutorialOptional.Tutorial()));
        

        for (int i = 0; i < nextLevelEnabledNodes.Length; ++i)
        {
            nextLevelEnabledNodes[i].SetHealthState(NodeEnums.HealthState.SURVIVED, true);
            nextLevelEnabledNodes[i].SetResurrectedVisuals();
            nextLevelEnabledNodes[i].ClearCameFromConnection();
            nextLevelEnabledNodes[i].isInteractable = true;
        }
    }


}
