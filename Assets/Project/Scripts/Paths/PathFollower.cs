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
    private Vector3 moveDirection;
    [SerializeField, Min(0f)] public float moveSpeed = 10f;
    private float baseMoveSpeed;
    private Transform transformToMove;

    // Path Follow Control
    private bool finished = false;
    [HideInInspector] public bool paused = false;

    // Distance
    private float travelledDistance = 0f;
    private float totalDistanceToTravel = 0f;
    public float DistanceLeftToEnd => totalDistanceToTravel - travelledDistance; 

    // Position
    public Vector3 Position => transformToMove.position;

    private Coroutine pauseCoroutine = null;


    // Actions
    public delegate void PathFollowerAction();
    public PathFollowerAction OnPathFollowStart;
    public PathFollowerAction OnPathEndReached;

    public delegate void PathFollowerAction2(PathFollower thisPathFollower);
    public PathFollowerAction2 OnPathEndReached2;



    public void Init(PathNode startTargetNode, Vector3 startDirection, Vector3 positionOffset, float totalDistanceToTravel, Transform transformToMove = null)
    {
        baseMoveSpeed = moveSpeed;

        targetNode = startTargetNode;
        moveDirection = startDirection;

        finished = false;
        paused = false;

        travelledDistance = 0.0f;
        this.totalDistanceToTravel = totalDistanceToTravel;

        this.transformToMove = transformToMove ? transformToMove : transform;


        this.positionOffset = positionOffset;
        currentStartPosition = transformToMove.position + positionOffset;
        currentEndPosition = targetNode.Position + positionOffset;

        this.transformToMove.rotation = Quaternion.LookRotation((currentEndPosition - currentStartPosition).normalized, targetNode.Up);

        startToEndT = 0f;

        distanceStartToEnd = (currentEndPosition - currentStartPosition).magnitude;
        step = moveSpeed / distanceStartToEnd;


        if (OnPathFollowStart != null) OnPathFollowStart();
    }



    private void Update()
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
        if (startToEndT > 0.999999f)
        {
            transformToMove.position = currentEndPosition; // Snap at position

            if (targetNode.IsLastNode)
            {
                finished = true;
                if (OnPathEndReached != null) OnPathEndReached();
                if (OnPathEndReached2 != null) OnPathEndReached2(this);
            }
            else
            {
                moveDirection = targetNode.GetDirectionToNextNode();
                targetNode = targetNode.GetNextNode();

                currentStartPosition = currentEndPosition;
                currentEndPosition = targetNode.Position + positionOffset;

                startToEndT = 0f;

                distanceStartToEnd = (currentEndPosition - currentStartPosition).magnitude;
                step = moveSpeed / distanceStartToEnd;
            }
        }

        float iterationStep = Time.deltaTime * step * GameTime.TimeScale;
        travelledDistance += iterationStep * distanceStartToEnd;

        startToEndT = Mathf.Clamp01(startToEndT + iterationStep);
        transformToMove.position = Vector3.LerpUnclamped(currentStartPosition, currentEndPosition, startToEndT);


        if (Vector3.Dot(transformToMove.forward, moveDirection) < 1f)
        {
            transformToMove.rotation = Quaternion.RotateTowards(transformToMove.rotation, Quaternion.LookRotation(moveDirection, transform.up), 300f * Time.deltaTime * GameTime.TimeScale);
        }
    }


    
    public void PauseForDuration(float duration)
    {
        if (!gameObject.activeInHierarchy) return;
        if (pauseCoroutine != null) StopCoroutine(pauseCoroutine);
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
