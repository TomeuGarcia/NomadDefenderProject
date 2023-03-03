using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OWMapPawn : MonoBehaviour
{

    [SerializeField] private Transform moveTransform;
    [SerializeField] private GameObject followCamera;
    public Transform FollowCameraTransform => followCamera.transform;

    private OWMap_Node currentNode;

    private OverworldMapGameManager owMapGameManager;

    private Vector3 camDisplacementToNextLevel;

    private Quaternion defaultRotation;
    


    public void Init(OverworldMapGameManager owMapGameManager, OWMap_Node startNode, Vector3 camDisplacementToNextLevel)
    {
        this.owMapGameManager = owMapGameManager;
        currentNode = startNode;

        moveTransform.position = GetNodePos(startNode);
        moveTransform.rotation = Quaternion.LookRotation(startNode.Forward, startNode.Up);

        defaultRotation = moveTransform.rotation;

        this.camDisplacementToNextLevel = camDisplacementToNextLevel;
    }

    public void MoveToNode(OWMap_Node targetNode)
    {
        if (currentNode == null)
        {
            currentNode = targetNode;
            return;
        }


        Vector3 startFwd = moveTransform.forward;
        Vector3 endFwd = (targetNode.Position - currentNode.Position).normalized;

        currentNode = targetNode;

        Vector3 targetPos = GetNodePos(currentNode);
        float distance = Vector3.Distance(moveTransform.position, targetPos);
        float duration = distance * 0.5f;

        moveTransform.DORotateQuaternion(Quaternion.FromToRotation(startFwd, endFwd), 0.25f)
            .OnComplete(() => moveTransform.DOMove(targetPos, duration)
                .OnComplete(() => moveTransform.DORotateQuaternion(defaultRotation, 0.25f)
                    .OnComplete(NotifyNodeWasReached)));
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
