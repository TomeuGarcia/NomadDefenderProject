using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathStart : MonoBehaviour
{
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private PathNode startNode;

    private void Start()
    {
        float totalDistance = 0f;
        PathNode itNode = startNode;
        while (!itNode.IsLastNode)
        {
            totalDistance += itNode.GetDistanceToNextNode();
            itNode = itNode.GetNextNode();
        }


        Enemy enemy = Instantiate(enemyPrefab, startNode.Position, Quaternion.identity);
        enemy.pathFollower.Init(startNode.GetNextNode(), startNode.GetDirectionToNextNode(), totalDistance, enemy.transformToMove);
    }
}
