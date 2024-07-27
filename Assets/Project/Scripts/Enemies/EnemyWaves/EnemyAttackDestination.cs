using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackDestination
{
    private Dictionary<PathNode, PathLocation> _endNodeToLocation;


    public EnemyAttackDestination(EnemyWaveManager.PathEndData[] pathEndsData)
    {
        _endNodeToLocation = new Dictionary<PathNode, PathLocation>(pathEndsData.Length);

        foreach (EnemyWaveManager.PathEndData pathEndData in pathEndsData)
        {
            _endNodeToLocation.Add(pathEndData.EndNode, pathEndData.EndLocation);
        }
    }


    public PathLocation GetLocationToAttack(PathNode endNode)
    {
        return _endNodeToLocation[endNode];
    }
}
