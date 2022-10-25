using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathStart : MonoBehaviour
{
    [SerializeField] private PathFollower pathFollowerPrefab;
    [SerializeField] private PathNode startNode;

    private void Start()
    {
        PathFollower pathFollower = Instantiate(pathFollowerPrefab, startNode.Position, Quaternion.identity);
        pathFollower.Init(startNode);
    }
}
