using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class OWMapPawn : MonoBehaviour
{

    [SerializeField] private Transform moveTransform;

    private OWMap_Node currentNode;

    private OverworldMapGameManager owMapGameManager;
    


    public void Init(OverworldMapGameManager owMapGameManager, OWMap_Node startNode)
    {
        this.owMapGameManager = owMapGameManager;
        currentNode = startNode;

        moveTransform.position = GetNodePos(startNode);
        moveTransform.rotation = Quaternion.LookRotation(startNode.Forward, startNode.Up);
    }

    public void MoveToNode(OWMap_Node targetNode)
    {
        currentNode = targetNode;
        
        Vector3 targetPos = GetNodePos(targetNode);
        float distance = Vector3.Distance(moveTransform.position, targetPos);
        float duration = distance * 0.5f;

        moveTransform.DOMove(targetPos, duration)
            .OnComplete(() => owMapGameManager.OnOwMapPawnReachedNode(currentNode) );
    }


    private Vector3 GetNodePos(OWMap_Node node)
    {
        return node.Position + (node.Up * 0.5f);
    }

}
