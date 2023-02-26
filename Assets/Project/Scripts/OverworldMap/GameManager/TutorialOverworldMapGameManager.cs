using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialOverworldMapGameManager : OverworldMapGameManager
{
    [Header("TUTORIAL")]
    [SerializeField] protected OWMapTutorialManager owMapTutorial;
    [SerializeField] protected OWMapTutorialManager2 owMapTutorial2;
    bool firstBattleResultApplied = false;


    protected override void Init()
    {
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

        owMapTutorial2.StartTutorial();
        mapSceneLoader.OnSceneFromMapUnloaded -= MapTutorialAfterSecondBattle;

    }
  

    protected override void ApplyBattleStateResult()
    {
        BattleStateResult.NodeBattleStateResult[] nodeResults = currentBattleStateResult.nodeResults;

        if (firstBattleResultApplied)
        {
            for (int i = 0; i < nodeResults.Length; ++i)
            {
                nodeResults[i].owMapNode.SetHealthState(nodeResults[i].healthState);
            }
        }
        else
        {
            firstBattleResultApplied = true;
            for (int i = 0; i < nodeResults.Length; ++i)
            {
                nodeResults[i].owMapNode.SetHealthState(NodeEnums.HealthState.UNDAMAGED); // Hardcoded to always apply UNDAMAGED
            }
        }

    }

}
