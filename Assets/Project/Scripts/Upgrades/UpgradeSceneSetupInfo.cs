using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "UpgradeSceneSetupInfo", 
    menuName = SOAssetPaths.MAP_SCENES + "UpgradeSceneSetupInfo")]
public class UpgradeSceneSetupInfo : ScriptableObject
{
    public NodeEnums.ProgressionState CurrentNodeProgressionState { get; private set; }
    public NodeEnums.HealthState CurrentNodeHealthState { get; private set; }

    public void SetData(NodeEnums.ProgressionState currentNodeProgressionState, NodeEnums.HealthState currentNodeHealthState)
    {
        CurrentNodeProgressionState = currentNodeProgressionState;
        CurrentNodeHealthState = currentNodeHealthState;
    }

    public void ResetDataAsPredefined(NodeEnums.ProgressionState currentNodeProgressionState)
    {
        CurrentNodeProgressionState = currentNodeProgressionState;
        CurrentNodeHealthState = NodeEnums.HealthState.NOT_FOUGHT_YET;
    }

}
