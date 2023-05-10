using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class OWCameraMovement : MonoBehaviour
{
    private Vector2 dragRange;

    private Vector3 originalPos;
    private Vector3 nextLevelPos;
    private Vector3 distanceToNextLevel;

    private float lastDragPos;

    private float speed = 0.008f;

    private bool canDrag = false;
    private bool moving = false;
    private bool nodeSelected = false;

    void Update()
    {
        if(canDrag)
        {
            if (Input.GetMouseButtonDown(1))
            {
                lastDragPos = Input.mousePosition.y;
            }
            else if (Input.GetMouseButton(1))
            {
                float difference = Input.mousePosition.y - lastDragPos;

                if (difference == 0) { return; }

                float newPos = Mathf.Clamp(transform.position.z + -difference * speed, dragRange.x, dragRange.y);

                transform.position = new Vector3(transform.position.x, transform.position.y, newPos);

                lastDragPos = Input.mousePosition.y;
            }
        }
    }

    public void Init(Vector3 newDistanceToNextLevel, float maxDistance)
    {
        dragRange = new Vector2(transform.position.z - 1.0f, maxDistance - 4.0f);
        distanceToNextLevel = newDistanceToNextLevel;
        CanDrag(true);

        SetPositions();
    }

    private void SetPositions()
    {
        originalPos = transform.position;
        nextLevelPos = originalPos + distanceToNextLevel;
    }

    public void LockCamera()
    {
        moving = true;
        CanDrag(false);
        transform.position = originalPos;
    }

    public void MoveToNextLevel()
    {
        StartCoroutine(LateMoveToNextLevel());
    }

    private IEnumerator LateMoveToNextLevel()
    {
        transform.DOMove(nextLevelPos, 2.0f);
        originalPos = nextLevelPos;
        nextLevelPos = originalPos + distanceToNextLevel;
        yield return new WaitForSeconds(2.0f);

        moving = false;
        if(!nodeSelected)
        {
            CanDrag(true);
        }
    }

    public void ResetPosition()
    {
        if (moving)
        {
            return;
        }
        CanDrag(false);
        float timeToReset = 0.5f;

        transform.DOMove(originalPos, timeToReset);
    }

    public void CanDrag(bool _canDrag)
    {
        canDrag = _canDrag;
    }
    public void NodeSelected(bool _nodeSelected)
    {
        nodeSelected = _nodeSelected;
    }


}
