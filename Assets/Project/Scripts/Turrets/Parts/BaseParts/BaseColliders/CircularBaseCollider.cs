using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularBaseCollider : BaseCollider
{
    [SerializeField] private CapsuleCollider rangeCollider;
    private float _nextRange;

    public override void UpdateRange(float statsRange)
    {
        float planeRange = statsRange * 2 + 1; //only for square
        float range = statsRange;

        rangeCollider.radius = range + 0.5f;
        rangePlaneMesh.transform.localScale = Vector3.one * (planeRange / 10f);
        rangePlaneMaterial.SetFloat("_TileNum", planeRange);
    }

    public override void DisableCollisions()
    {
        rangeCollider.enabled = false;
    }

    public override void EnableCollisions()
    {
        rangeCollider.enabled = true;
    }

    public override bool IsPointWithinRange(Vector3 point)
    {
        Vector3 center = transform.position;
        center.y = 0f;

        point.y = 0f;

        float distance = Vector3.Distance(center, point);
        //Debug.Log(distance + " < " + rangeCollider.radius);        

        return distance <= rangeCollider.radius;
    }
    
    public override bool IsBoundsWithinRange(Bounds bounds)
    {
        if (Vector3.Distance(bounds.center, transform.position) < rangeCollider.radius)
        {
            return true;
        }

        Vector3 rangeColliderSurfacePoint = (bounds.center - transform.position).normalized * rangeCollider.radius + transform.position;
        bounds.extents = Vector3.one * 0.3f;

        return bounds.Contains(rangeColliderSurfacePoint);
    }

    protected override void UpdatePreviewRange(float currentRangeStat, float nextRangeStat)
    {
        float currentPlaneRange = currentRangeStat * 2 + 1; //only for square
        float nextPlaneRange = nextRangeStat * 2 + 1; //only for square
        float changeMultiplier = currentPlaneRange / nextPlaneRange;
        
        _nextRange = rangeCollider.radius * (1f / changeMultiplier);

        upgradePreviewRangePlaneMesh.transform.localScale = Vector3.one * (nextPlaneRange / 10f);
        upgradePreviewRangePlaneMaterial.SetFloat("_TileNum", nextPlaneRange);
        upgradePreviewRangePlaneMaterial.SetFloat("_InnerMask", changeMultiplier);
    }

    public override Collider GetCollider()
    {
        return rangeCollider;
    }

    public override bool ColliderIsWithinRange(SphereCollider otherCollider)
    {
        return ColliderWithinRange(otherCollider, rangeCollider.radius);
    }

    public override bool ColliderWillBeWithinRange(SphereCollider otherCollider)
    {
        return ColliderWithinRange(otherCollider, _nextRange);
    }
    
    private bool ColliderWithinRange(SphereCollider otherCollider, float thisColliderRadius)
    {
        Vector3 otherPosition = otherCollider.transform.position;
        otherPosition.y = 0;
        Vector3 thisPosition = rangeCollider.transform.position;
        thisPosition.y = 0;
        
        float distance = (otherPosition - thisPosition).magnitude;
        float radiusSum = thisColliderRadius + otherCollider.radius;
        
        return distance <= radiusSum;
    }
}
