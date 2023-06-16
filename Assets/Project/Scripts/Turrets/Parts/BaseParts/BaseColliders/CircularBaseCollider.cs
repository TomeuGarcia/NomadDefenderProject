using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularBaseCollider : BaseCollider
{
    [SerializeField] private CapsuleCollider rangeCollider;

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
        Debug.Log(distance + " < " + rangeCollider.radius);

        return distance <= rangeCollider.radius;
    }

}
