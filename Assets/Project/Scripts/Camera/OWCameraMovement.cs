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

    private float _goalMouseScroll;

    void Update()
    {
        if(canDrag)
        {
            if (Input.GetMouseButtonDown(1))
            {
                lastDragPos = Input.mousePosition.y;
                _goalMouseScroll = 0;
            }
            else if (Input.GetMouseButton(1))
            {
                float difference = Input.mousePosition.y - lastDragPos;

                if (difference == 0) { return; }

                MoveCameraDragging(-difference, out float excess);
                
                lastDragPos = Input.mousePosition.y;
            }
            else if (!Input.mouseScrollDelta.y.AlmostZero())
            {
                _goalMouseScroll += Input.mouseScrollDelta.y * _wheelMove;
            }

            UpdateMouseWheelScroll();
        }
    }

    private void MoveCameraDragging(float moveAmount, out float excess)
    {
        float desiredPosition = transform.position.z + moveAmount * speed;
        float newPos = Mathf.Clamp(desiredPosition, dragRange.x, dragRange.y);
        excess = desiredPosition - newPos;
        
        transform.position = new Vector3(transform.position.x, transform.position.y, newPos);
    }

    [SerializeField, Min(0)] private float _wheelMove = 60f;
    [SerializeField, Min(0)] private float _wheelSpeed1 = 1200f;
    [SerializeField, Min(0)] private float _wheelSpeed2 = 500f;
    [SerializeField, Min(0)] private float _wheelMoveSharpness = 10f;
    private void UpdateMouseWheelScroll()
    {
        if (_goalMouseScroll.AlmostZero()) return;

        float moveStep = Time.deltaTime * _wheelSpeed1; 
        moveStep *= Mathf.Pow(Mathf.Lerp(0f, 1f, Mathf.Abs(_goalMouseScroll * _wheelSpeed2)), _wheelMoveSharpness);
        
        bool needToIncrementing = _goalMouseScroll < 0;
        float moveAmount = needToIncrementing 
            ? Mathf.Min(moveStep, -_goalMouseScroll) 
            : Mathf.Min(-moveStep, _goalMouseScroll);
        
        _goalMouseScroll += moveAmount;
        
        MoveCameraDragging(-moveAmount, out float excess);

        _goalMouseScroll -= excess;
    }
    
    public void Init(Vector3 newDistanceToNextLevel, float maxDistance)
    {
        UpdateMaxDragDistance(maxDistance);
        distanceToNextLevel = newDistanceToNextLevel;
        CanDrag(true);

        SetPositions();
    }

    public void UpdateMaxDragDistance(float maxDistance)
    {
        dragRange = new Vector2(transform.position.z - 1.0f, maxDistance - 4.0f);
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
