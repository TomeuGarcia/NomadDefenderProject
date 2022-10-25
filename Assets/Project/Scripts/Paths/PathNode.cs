using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PathNode : MonoBehaviour
{
    static float arrivedThreshold = 0.05f;


    [SerializeField] private PathNode nextNode;

    public bool IsLastNode => nextNode == null;
    public Vector3 Position => transform.position;




    public PathNode GetNextNode()
    {
        return nextNode;
    }

    public Vector3 GetDirectionToNextNode()
    {
        return (nextNode.Position - Position).normalized;
    }

    public bool HasArrived(Vector3 position)
    {
        return Vector3.Distance(position, Position) <= arrivedThreshold;
    }


}
