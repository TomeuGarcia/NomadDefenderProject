using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollower : MonoBehaviour
{

    // Target Node
    private PathNode targetNode = null;
    private Vector3 moveDirection;
    [SerializeField, Min(0f)] private float moveSpeed = 10f;
    private Transform transformToMove;

    // Path Follow Control
    private bool finished = false;
    [HideInInspector] public bool paused = false;

    // Distance
    private float travelledDistance = 0f;
    private float totalDistanceToTravel = 0f;
    float DistanceLeftToEnd => totalDistanceToTravel - travelledDistance; 

    // Position
    public Vector3 Position => transformToMove.position;



    // Actions
    public delegate void PathFollowerAction();
    public PathFollowerAction OnPathFollowStart;
    public PathFollowerAction OnPathEndReached;



    public void Init(PathNode startTargetNode, Vector3 startDirection, float totalDistanceToTravel, Transform transformToMove = null)
    {
        targetNode = startTargetNode;
        moveDirection = startDirection;

        this.totalDistanceToTravel = totalDistanceToTravel;

        this.transformToMove = transformToMove ? transformToMove : transform;

        if (OnPathFollowStart != null) OnPathFollowStart();
    }



    private void Update()
    {
        if (!finished && !paused) FollowPath();
    }


    void FollowPath()
    {
        if (targetNode.HasArrived(Position))
        {
            transformToMove.position = targetNode.Position; // Snap at position


            if (targetNode.IsLastNode)
            {
                finished = true;
                if (OnPathEndReached != null) OnPathEndReached();
            }
            else
            {
                moveDirection = targetNode.GetDirectionToNextNode();
                targetNode = targetNode.GetNextNode();
            }

        }

        Vector3 travelStep = moveDirection * (moveSpeed * Time.deltaTime);
        transformToMove.position = Position + (travelStep);
        travelledDistance += travelStep.magnitude;

        if (Vector3.Dot(transformToMove.forward, moveDirection) < 1f)
        {
            transformToMove.rotation = Quaternion.RotateTowards(transformToMove.rotation, Quaternion.LookRotation(moveDirection, transform.up), 300f * Time.deltaTime);
        }
        
    }

    public float GetTravelDistance()
    {
        return travelledDistance;
    }
}
