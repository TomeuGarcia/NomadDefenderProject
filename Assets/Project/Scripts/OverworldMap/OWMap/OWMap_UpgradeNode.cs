using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OWMap_UpgradeNode : OWMap_NodeClass
{
    public NodeEnums.UpgradeType upgradeType;
    public NodeEnums.ProgressionState progressionState;

    public OWMap_UpgradeNode(int _nextLevelNodes, ref NodeEnums.HealthState _healthState, NodeEnums.UpgradeType _upgradeType, NodeEnums.ProgressionState _progressionState) 
        : base(NodeEnums.NodeType.UPGRADE, _nextLevelNodes, ref _healthState) 
    {
        upgradeType= _upgradeType;
        progressionState = _progressionState;
    }
    public override void StartLevel(OverworldMapGameManager overwolrdMapGameManager)
    {
        overwolrdMapGameManager.StartUpgradeScene(upgradeType, healthState);
    }
}
