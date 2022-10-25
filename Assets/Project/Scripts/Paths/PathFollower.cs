using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    
    private PathNode currentNode = null;
    private Vector3 moveDirection;

    public Vector3 Position => transform.position;


    public void Init(PathNode startNode)
    {
        currentNode = startNode;


        moveDirection = currentNode.GetDirectionToNextNode();
    }



    private void Update()
    {
        
        if (currentNode.HasArrived(Position))
        {
            // TODO
        }


    }


}
