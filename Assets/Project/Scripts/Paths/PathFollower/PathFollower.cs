using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    // Interpolation
    private Vector3 positionOffset;
    private Vector3 currentStartPosition;
    private Vector3 currentEndPosition;
    private float _startToEndT;
    private float step;
    private float distanceStartToEnd;

    // Target Node
    public PathNode CurrentNode { get; private set; }
    private PathNode targetNode = null;
    public PathNode CurrentTargetNode => targetNode;
    private Quaternion _targetRotation;
    public Vector3 MoveDirection { get; private set; }
    [SerializeField, Min(0f)] private float moveSpeed = 10f;
    private float _speedMultiplier;
    [SerializeField, Min(0f)] public float _rotationSpeed = 300f;
    private float _baseMoveSpeed;
    [SerializeField] private Rigidbody _rigidbodyToMove;

    // Path Follow Control
    private bool finished = false;
    [HideInInspector] public bool paused = false;

    // Distance
    public float TravelledDistance { get; private set; } = 0f;
    private float totalDistanceToTravel = 0f;
    public float DistanceLeftToEnd => totalDistanceToTravel - TravelledDistance; 

    // Position
    public Vector3 Position => _rigidbodyToMove.position;

    private Coroutine pauseCoroutine = null;

    public Vector3 PositionBetweenNodes =>
        Vector3.LerpUnclamped(CurrentNode.Position, targetNode.Position, _startToEndT);

    // Actions
    public delegate void PathFollowerAction();
    public PathFollowerAction OnPathFollowStart;
    public PathFollowerAction OnPathEndReached;

    public delegate void PathFollowerAction2(PathFollower thisPathFollower);
    public PathFollowerAction2 OnPathEndReached2;

    public float ToNextNodeT => _startToEndT;

    private void OnDisable()
    {
        TravelledDistance = 0.0f;
    }

    public void Init(PathNode startNode, Vector3 positionOffset, float totalDistanceToTravel, float startToEndT = 0)
    {
        UpdateBaseMoveSpeed(moveSpeed);
        _speedMultiplier = 1;

        CurrentNode = targetNode = startNode;
        UpdateTarget(CurrentNode.GetNextNode(), CurrentNode.GetDirectionToNextNode());

        finished = false;
        paused = false;

        TravelledDistance = 0.0f;
        this.totalDistanceToTravel = totalDistanceToTravel;

        this.positionOffset = positionOffset;
        currentStartPosition = Position + positionOffset;
        currentEndPosition = targetNode.Position + positionOffset;

        _rigidbodyToMove.rotation = _targetRotation;

        _startToEndT = startToEndT;

        distanceStartToEnd = (currentEndPosition - currentStartPosition).magnitude;
        step = moveSpeed / distanceStartToEnd;


        if (OnPathFollowStart != null) OnPathFollowStart();
    }


    /*
    private void Update()
    {
        if (!finished && !paused) FollowPathInterpolated();
    }
    */

    private void FixedUpdate()
    {
        if (!finished && !paused) FollowPathInterpolated();
    }

    public void SetMoveSpeedMultiplier(float speedMultiplier)
    {
        _speedMultiplier = speedMultiplier;
        UpdateMoveSpeed();
    }

    public void UpdateBaseMoveSpeed(float baseMoveSpeed)
    {
        _baseMoveSpeed = baseMoveSpeed;
        UpdateMoveSpeed();
    }
    private void UpdateMoveSpeed()
    {
        moveSpeed = (_baseMoveSpeed * _speedMultiplier);
        step = moveSpeed / distanceStartToEnd;
    }
    
    private void FollowPathInterpolated()
    {
        if (_startToEndT > 0.99f)
        {
            _rigidbodyToMove.position = currentEndPosition; // Snap at position

            if (targetNode.IsLastNode)
            {
                finished = true;
                if (OnPathEndReached != null) OnPathEndReached();
                if (OnPathEndReached2 != null) OnPathEndReached2(this);
            }
            else
            {
                UpdateTarget(targetNode.GetNextNode(), targetNode.GetDirectionToNextNode());

                currentStartPosition = currentEndPosition;
                currentEndPosition = targetNode.Position + positionOffset;

                _startToEndT = 0f;

                distanceStartToEnd = (currentEndPosition - currentStartPosition).magnitude;
                step = moveSpeed / distanceStartToEnd;
            }
        }

        float iterationStep = step * GameTime.TimeScale * Time.fixedDeltaTime;
        TravelledDistance += iterationStep * distanceStartToEnd;

        _startToEndT = Mathf.Clamp01(_startToEndT + iterationStep);
        Vector3 currentPosition = Vector3.LerpUnclamped(currentStartPosition, currentEndPosition, _startToEndT);

        bool isLookingTowardsMoveDirection = Vector3.Dot(_rigidbodyToMove.transform.forward, MoveDirection) > 0.98f;
        Quaternion currentRotation = isLookingTowardsMoveDirection
            ? _targetRotation
            : Quaternion.RotateTowards(_rigidbodyToMove.rotation, _targetRotation, _rotationSpeed * GameTime.DeltaTime);


        _rigidbodyToMove.Move(currentPosition, currentRotation);
    }


    private void UpdateTarget(PathNode newTargetNode, Vector3 targetDirection)
    {
        CurrentNode = targetNode;
        targetNode = newTargetNode;
        MoveDirection = targetDirection;
        _targetRotation = Quaternion.LookRotation(MoveDirection, targetNode.Up);
    }

    
    public void PauseForDuration(float duration)
    {
        if (!gameObject.activeInHierarchy) return;
        if (pauseCoroutine != null)
        {
            StopCoroutine(pauseCoroutine);
        }
        pauseCoroutine = StartCoroutine(DoPauseForDuration(duration));
    }

    private IEnumerator DoPauseForDuration(float duration)
    {
        paused = true;
        yield return new WaitForSeconds(duration);
        paused = false;
        pauseCoroutine = null;
    }

    public void CheckDeactivateCoroutines()
    {
        if (pauseCoroutine != null) StopCoroutine(pauseCoroutine);
    }

}
