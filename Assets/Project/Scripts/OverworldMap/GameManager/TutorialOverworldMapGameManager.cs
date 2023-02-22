using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialOverworldMapGameManager : OverworldMapGameManager
{
    [Header("TUTORIAL")]
    [SerializeField] protected OWMapTutorialManager owMapTutorial;

    protected override void Init()
    {
        InitMapGeneration();
        InitMapSceneLoader();

        StartAtFirstLevel();

        if (!TutorialsSaverLoader.GetInstance().IsTutorialDone(Tutorials.BATTLE))
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
            {
                CreateNewBattleStateResult(out currentBattleStateResult);
                StartMapTutorial();
            }
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
    }

    protected override void ApplyBattleStateResult()
    {
        BattleStateResult.NodeBattleStateResult[] nodeResults = currentBattleStateResult.nodeResults;

        for (int i = 0; i < nodeResults.Length; ++i)
        {
            //nodeResults[i].owMapNode.SetHealthState(nodeResults[i].healthState);
            nodeResults[i].owMapNode.SetHealthState(NodeEnums.HealthState.UNDAMAGED); // Hardcoded to always apply UNDAMAGED
        }
    }

}
