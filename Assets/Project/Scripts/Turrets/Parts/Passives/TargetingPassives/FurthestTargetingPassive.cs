using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FurthestTargetingPassive", menuName = "TurretPassives/FurthestTargetingPassive")]
public class FurthestTargetingPassive : BaseTargetingPassive
{
    protected override int SortingFunction(Enemy e1, Enemy e2)
    {
        //return e2.pathFollower.DistanceLeftToEnd.CompareTo(e1.pathFollower.DistanceLeftToEnd);

        float distanceE1 = Vector3.Distance(e1.Position, turretOwner.transform.position);
        float distanceE2 = Vector3.Distance(e2.Position, turretOwner.transform.position);

        return distanceE1.CompareTo(distanceE2);
    }
}
