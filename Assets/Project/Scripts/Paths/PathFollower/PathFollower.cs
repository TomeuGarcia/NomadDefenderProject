using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    // Interpolation
    private Vector3 positionOffset;
    private Vector3 currentStartPosition;
    private Vector3 currentEndPosition;
    private float startToEndT;
    private float step;
    private float distanceStartToEnd;

    // Target Node
    private PathNode targetNode = null;
    private Quaternion _targetRotation;
    public Vector3 MoveDirection { get; private set; }
    [SerializeField, Min(0f)] public float moveSpeed = 10f;
    private float baseMoveSpeed;
    [SerializeField] private Rigidbody _rigidbodyToMove;

    // Path Follow Control
    private bool finished = false;
    [HideInInspector] public bool paused = false;

    // Distance
    private float travelledDistance = 0f;
    private float totalDistanceToTravel = 0f;
    public float DistanceLeftToEnd => totalDistanceToTravel - travelledDistance; 

    // Position
    public Vector3 Position => _rigidbodyToMove.position;

    private Coroutine pauseCoroutine = null;


    // Actions
    public delegate void PathFollowerAction();
    public PathFollowerAction OnPathFollowStart;
    public PathFollowerAction OnPathEndReached;

    public delegate void PathFollowerAction2(PathFollower thisPathFollower);
    public PathFollowerAction2 OnPathEndReached2;



    public void Init(PathNode startTargetNode, Vector3 startDirection, Vector3 positionOffset, float totalDistanceToTravel)
    {
        baseMoveSpeed = moveSpeed;

        UpdateTarget(startTargetNode, startDirection);

        finished = false;
        paused = false;

        travelledDistance = 0.0f;
        this.totalDistanceToTravel = totalDistanceToTravel;

        this.positionOffset = positionOffset;
        currentStartPosition = Position + positionOffset;
        currentEndPosition = targetNode.Position + positionOffset;

        _rigidbodyToMove.rotation = _targetRotation;

        startToEndT = 0f;

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

    public void SetMoveSpeed(float speedCoef)
    {
        moveSpeed = (baseMoveSpeed * speedCoef);
        step = moveSpeed / distanceStartToEnd;
    }
    
    private void FollowPathInterpolated()
    {
        if (startToEndT > 0.99f)
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

                startToEndT = 0f;

                distanceStartToEnd = (currentEndPosition - currentStartPosition).magnitude;
                step = moveSpeed / distanceStartToEnd;
            }
        }

        float iterationStep = step * GameTime.TimeScale * Time.fixedDeltaTime;
        travelledDistance += iterationStep * distanceStartToEnd;

        startToEndT = Mathf.Clamp01(startToEndT + iterationStep);

        Vector3 currentPosition = Vector3.LerpUnclamped(currentStartPosition, currentEndPosition, startToEndT);
        Quaternion currentRotation = _targetRotation;// Quaternion.identity;

        _rigidbodyToMove.Move(currentPosition, currentRotation);

        return;
        if (Vector3.Dot(_rigidbodyToMove.transform.forward, MoveDirection) > 0.98f)
        {
            currentRotation = _targetRotation;
        }
        else
        {
            currentRotation = Quaternion.RotateTowards(_rigidbodyToMove.rotation, _targetRotation, 300f * GameTime.DeltaTime);
        }

        
    }


    private void UpdateTarget(PathNode newTargetNode, Vector3 targetDirection)
    {
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
