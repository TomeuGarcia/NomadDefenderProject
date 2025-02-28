using UnityEngine;

public abstract class AOWMapLifetimeListener : MonoBehaviour
{
    public abstract void OnNodeSelected(OWMap_Node selectedNode);
    public abstract void OnBattleResultsSet(BattleStateResult.NodeBattleStateResult[] nodeResults);
    public abstract void OnComeBackFromNodeScene(OWMap_Node currentNode, OWMap_Node firstNextNode);
}
