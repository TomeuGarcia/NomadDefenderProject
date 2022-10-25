using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollower : MonoBehaviour
{

    
    private PathNode currentNode = null;
    private Vector3 moveDirection;
    [SerializeField, Min(0f)] private float moveSpeed = 20f;

    public Vector3 Position => transform.position;


    public void Init(PathNode startNode)
    {
        currentNode = startNode;


        moveDirection = currentNode.GetDirectionToNextNode();
    }



    private void Update()
    {
        if (currentNode.GetNextNode().HasArrived(Position))
        {
            if (!currentNode.IsLastNode)
            {
                currentNode = currentNode.GetNextNode();
                moveDirection = currentNode.GetDirectionToNextNode();
            }
            else
            {
                moveDirection = Vector3.zero;
            }

            transform.position = currentNode.Position;
        }

        transform.position = Position + (moveDirection * (moveSpeed * Time.deltaTime));
    }


}
