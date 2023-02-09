using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStateResult
{
    public struct NodeBattleStateResult
    {
        public NodeBattleStateResult(OWMap_Node owMapNode)
        {
            this.owMapNode = owMapNode;
            this.healthState = NodeEnums.HealthState.UNDAMAGED;
        }

        public readonly OWMap_Node owMapNode;
        public NodeEnums.HealthState healthState;
    }


    public BattleStateResult(OWMap_Node[] owMapNodes)
    {
        nodeResults = new NodeBattleStateResult[owMapNodes.Length];

        for (int i = 0; i < owMapNodes.Length; i++)
        {
            nodeResults[i] = new NodeBattleStateResult(owMapNodes[i]);
        }
    }

    public NodeBattleStateResult[] nodeResults;
    public bool DidWinWithPerfectDefense()
    {
        if (nodeResults.Length < 2) return false;

        for (int i = 0; i < nodeResults.Length; i++)
        {
            if (nodeResults[i].healthState != NodeEnums.HealthState.UNDAMAGED)
            {
                return false;
            }
        }

        return true;
    }

}
