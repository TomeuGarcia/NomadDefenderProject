using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OWMapPawn : MonoBehaviour
{

    [SerializeField] private Transform moveTransform;
    [SerializeField] private OWCameraMovement followCamera;
    [SerializeField] private FMODUnity.StudioListener cameraListener;
    public Transform FollowCameraTransform => followCamera.gameObject.transform;

    private OWMap_Node currentNode;

    private OverworldMapGameManager owMapGameManager;

    private Quaternion defaultRotation;
    


    public void Init(OverworldMapGameManager owMapGameManager, OWMap_Node startNode, Vector3 camDisplacementToNextLevel)
    {
        this.owMapGameManager = owMapGameManager;
        currentNode = startNode;

        moveTransform.position = GetNodePos(startNode);
        moveTransform.rotation = Quaternion.LookRotation(startNode.Forward, startNode.Up);

        defaultRotation = moveTransform.rotation;

        this.followCamera.Init(camDisplacementToNextLevel, owMapGameManager.GetMapNodes()[owMapGameManager.GetMapNodes().Length - 1][0].Position.z);
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

        followCamera.CanDrag(false);
        moveTransform.DORotateQuaternion(Quaternion.FromToRotation(startFwd, endFwd), 0.25f)
            .OnComplete(() => moveTransform.DOMove(targetPos, duration)
                .OnComplete(() => followCamera.CanDrag(true))
                    .OnComplete(() => moveTransform.DORotateQuaternion(defaultRotation, 0.25f)
                        .OnComplete(NotifyNodeWasReached)));
    }

    public void ResetPosition()
    {
        followCamera.ResetPosition();
    }
    public void NodeSelected()
    {
        ResetPosition();
        followCamera.NodeSelected(true);
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
        followCamera.gameObject.SetActive(true);
    }

    public void DeactivateCamera()
    {
        followCamera.gameObject.SetActive(false);
    }

    public void MoveCameraToNextLevel()
    {
        followCamera.NodeSelected(false);
        followCamera.LockCamera();
        StartCoroutine(LateMoveToNextLevel());
    }

    private IEnumerator LateMoveToNextLevel()
    {
        yield return new WaitForSeconds(1.5f);
        ActivateCamera();
        followCamera.MoveToNextLevel();
    }

    public void MuteListener()
    {
        cameraListener.enabled = false;
    }
    public void UnmuteListener()
    {
        cameraListener.enabled = true;
    }
}
