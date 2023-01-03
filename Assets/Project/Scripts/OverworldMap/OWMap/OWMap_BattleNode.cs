using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OWMap_BattleNode : OWMap_NodeClass
{
    public NodeEnums.BattleType battleType;

    public OWMap_BattleNode(int _nextLevelNodes, ref NodeEnums.HealthState _healthState, NodeEnums.BattleType _battleType) : base(NodeEnums.NodeType.BATTLE, _nextLevelNodes, ref _healthState) 
    {
        battleType = _battleType;
    }
    public override void StartLevel(OverworldMapGameManager overwolrdMapGameManager)
    {
        overwolrdMapGameManager.StartBattleScene(battleType, nextLevelNodes);
    }
}
