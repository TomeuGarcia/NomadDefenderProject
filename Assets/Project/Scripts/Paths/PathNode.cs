using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode : MonoBehaviour
{
    static float ARRIVED_THRESHOLD = 0.08f;


    [SerializeField] protected PathNode nextNode;

    public bool IsLastNode => nextNode == null;
    public Vector3 Position => transform.position;




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


}
