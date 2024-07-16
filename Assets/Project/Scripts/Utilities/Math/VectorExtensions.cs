using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtensions
{
    public static Vector3 RandomVector(Vector3 bounds)
    {
        return new Vector3(
            Random.Range(-bounds.x, bounds.x),
            Random.Range(-bounds.y, bounds.y),
            Random.Range(-bounds.z, bounds.z)
            );
    }
}
