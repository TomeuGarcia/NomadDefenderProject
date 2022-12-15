using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurthestTargetingPassive : BaseTargetingPassives
{
    protected override int SortingFunction(Enemy e1, Enemy e2)
    {
        return e2.pathFollower.DistanceLeftToEnd.CompareTo(e1.pathFollower.DistanceLeftToEnd); //TEST
    }
}
