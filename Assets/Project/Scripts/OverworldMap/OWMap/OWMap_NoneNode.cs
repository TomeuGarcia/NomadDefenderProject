using NodeEnums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OWMap_NoneNode : OWMap_NodeClass
{
    public OWMap_NoneNode(int _nextLevelNodes, ref HealthState _healthState) : base(NodeEnums.NodeType.NONE, _nextLevelNodes, ref _healthState)
    {
    }

    public override void StartLevel(OverworldMapGameManager overwolrdMapGameManager) { }
}
