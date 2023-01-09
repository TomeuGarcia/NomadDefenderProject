using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OWMap_UpgradeNode : OWMap_NodeClass
{
    public NodeEnums.UpgradeType upgradeType;

    public OWMap_UpgradeNode(int _nextLevelNodes, ref NodeEnums.HealthState _healthState, NodeEnums.UpgradeType _upgradeType) : base(NodeEnums.NodeType.UPGRADE, _nextLevelNodes, ref _healthState) 
    {
        upgradeType= _upgradeType;
    }
    public override void StartLevel(OverworldMapGameManager overwolrdMapGameManager)
    {
        overwolrdMapGameManager.StartUpgradeScene(upgradeType, healthState);
    }
}
