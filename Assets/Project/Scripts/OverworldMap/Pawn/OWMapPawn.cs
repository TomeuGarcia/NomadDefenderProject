using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class OWMapPawn : MonoBehaviour
{

    [SerializeField] private Transform moveTransform;
    [SerializeField] private GameObject followCamera;

    private OWMap_Node currentNode;

    private OverworldMapGameManager owMapGameManager;

    private Vector3 camDisplacementToNextLevel;
    


    public void Init(OverworldMapGameManager owMapGameManager, OWMap_Node startNode, Vector3 camDisplacementToNextLevel)
    {
        this.owMapGameManager = owMapGameManager;
        currentNode = startNode;

        moveTransform.position = GetNodePos(startNode);
        moveTransform.rotation = Quaternion.LookRotation(startNode.Forward, startNode.Up);

        this.camDisplacementToNextLevel = camDisplacementToNextLevel;
    }

    public void MoveToNode(OWMap_Node targetNode)
    {
        currentNode = targetNode;
        
        Vector3 targetPos = GetNodePos(targetNode);
        float distance = Vector3.Distance(moveTransform.position, targetPos);
        float duration = distance * 0.5f;

        moveTransform.DOMove(targetPos, duration)
            .OnComplete(NotifyNodeWasReached);
    }


    private Vector3 GetNodePos(OWMap_Node node)
    {
        return node.Position + (node.Up * 0.5f);
    }

    private void NotifyNodeWasReached()
    {
        owMapGameManager.OnOwMapPawnReachedNode(currentNode);
    }


    public void ActivateCamera()
    {
        followCamera.SetActive(true);
    }

    public void DeactivateCamera()
    {
        followCamera.SetActive(false);
    }

    public void MoveCameraToNextLevel()
    {
        StartCoroutine(LateMoveCameraToNextLevel());
    }
    private IEnumerator LateMoveCameraToNextLevel()
    {
        yield return new WaitForSeconds(1.5f);
        followCamera.transform.DOMove(followCamera.transform.position + camDisplacementToNextLevel, 2.0f);
    }
}
