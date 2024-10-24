using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "UpgradeSceneSetupInfo", menuName = "Map/UpgradeSceneSetupInfo")]
public class UpgradeSceneSetupInfo : ScriptableObject
{
    public NodeEnums.ProgressionState CurrentNodeProgressionState { get; private set; }
    public NodeEnums.HealthState CurrentNodeHealthState { get; private set; }
    public bool LastBattleWasDefendedPerfectly { get; private set; }

    public void SetData(NodeEnums.ProgressionState currentNodeProgressionState, NodeEnums.HealthState currentNodeHealthState, bool lastBattleWasDefendedPerfectly)
    {
        CurrentNodeProgressionState = currentNodeProgressionState;
        CurrentNodeHealthState = currentNodeHealthState;
        LastBattleWasDefendedPerfectly = lastBattleWasDefendedPerfectly;
    }

    public void ResetDataAsPredefined(NodeEnums.ProgressionState currentNodeProgressionState)
    {
        CurrentNodeProgressionState = currentNodeProgressionState;
        CurrentNodeHealthState = NodeEnums.HealthState.UNDAMAGED;
        LastBattleWasDefendedPerfectly = false;
    }

}
