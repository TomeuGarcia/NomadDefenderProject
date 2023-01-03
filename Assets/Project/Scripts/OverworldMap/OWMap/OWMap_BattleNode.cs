using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OWMap_BattleNode : OWMap_NodeClass
{
    public NodeEnums.BattleType battleType;

    public OWMap_BattleNode(int _nextLevelNodes, ref NodeEnums.HealthState _healthState) : base(_nextLevelNodes, ref _healthState) { }
    public override void StartLevel(OverworldMapGameManager overwolrdMapGameManager)
    {
        overwolrdMapGameManager.StartBattleScene(battleType, nextLevelNodes);
    }
}
