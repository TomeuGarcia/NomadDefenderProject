using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode : MonoBehaviour
{
    static float ARRIVED_THRESHOLD = 0.08f;


    [SerializeField] protected PathNode nextNode;
    [SerializeField] private Vector3 _positionOffset = Vector3.zero;

    public bool IsLastNode => nextNode == null;
    public Vector3 Position => transform.position + transform.TransformVector(_positionOffset);
    public Vector3 Up => transform.up;

    private Vector3 PositionForGizmo => Position + Up * 0.25f;



    public PathNode GetNextNode()
    {
        return nextNode;
    }

    public float GetDistanceToNextNode()
    {
        return (nextNode.Position - Position).magnitude;
    }

    public Vector3 GetDirectionToNextNode()
    {
        return (nextNode.Position - Position).normalized;
    }

    public bool HasArrived(Vector3 position)
    {
        return Vector3.Distance(position, Position) <= ARRIVED_THRESHOLD;
    }


    public float ComputeTotalDistanceUntilEnd()
    {
        float totalDistance = 0f;
        PathNode itNode = this;
        while (!itNode.IsLastNode)
        {
            totalDistance += itNode.GetDistanceToNextNode();
            itNode = itNode.GetNextNode();
        }

        return totalDistance;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(PositionForGizmo, 0.05f);
        
        PathNode tempNode = this;
        int pathLength = 1;
        while (!tempNode.IsLastNode)
        {
            if (pathLength > 20)
            {
                Debug.LogError("Path is too long or there are references in a LOOP, please ensure everything is correct");
                return;
            }
            
            tempNode = tempNode.nextNode;
            ++pathLength;
        }
        
        tempNode = this;
        int pathItR = pathLength;
        while (!tempNode.IsLastNode)
        {
            Gizmos.color = Color.magenta * (((float)pathItR / pathLength) * 0.5f + 0.5f);
            GizmosUtility.DrawArrow(tempNode.PositionForGizmo, tempNode.nextNode.PositionForGizmo);
            
            tempNode = tempNode.nextNode;
            --pathItR;
        }
    }
}
