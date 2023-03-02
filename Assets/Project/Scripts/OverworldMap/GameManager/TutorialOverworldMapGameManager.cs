using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialOverworldMapGameManager : OverworldMapGameManager
{
    [Header("TUTORIAL")]
    [SerializeField] protected OWMapTutorialManager owMapTutorial;
    [SerializeField] protected OWMapTutorialManager2 owMapTutorial2;
    bool firstBattleResultApplied = false;
    int battleCounter = 0;


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
        battleCounter++;
        if (battleCounter <= 1) return;

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
                nodeResults[i].owMapNode.SetHealthState(nodeResults[i].healthState, wonWithPerfectDefense);
            }
        }
        else
        {
            firstBattleResultApplied = true;
            for (int i = 0; i < nodeResults.Length; ++i)
            {
                nodeResults[i].owMapNode.SetHealthState(NodeEnums.HealthState.UNDAMAGED, false); // Hardcoded to always apply UNDAMAGED
            }
        }

    }

}
