using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationBranchPathNode : PathNode
{
    [SerializeField] private PathNode locationEndNode;
    private PathNode holderNextNode;

    [SerializeField] private PathLocation pathLocation;
    private bool LocationIsDead => pathLocation.IsDead;


    void Start()
    {
        holderNextNode = nextNode;
        nextNode = locationEndNode;

        StartCoroutine(LocationDeadResetNextNode());
    }

    
    private IEnumerator LocationDeadResetNextNode()
    {
        yield return new WaitUntil(() => LocationIsDead);

        nextNode = holderNextNode;
    }
}
