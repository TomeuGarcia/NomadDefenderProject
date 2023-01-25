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
        rangePlaneMeshObject.transform.localScale = Vector3.one * (planeRange / 10f);
        rangePlaneMaterial = rangePlaneMeshObject.GetComponent<MeshRenderer>().materials[0];
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

}
