using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupportStatsSnapshot
{
    public float RadiusRange { get; private set; }

    public SupportStatsSnapshot(float radiusRange)
    {
        Reset(radiusRange);
    }

    public void Reset(float radiusRange)
    {
        RadiusRange = radiusRange;
    }

    public bool HasRadiusRange()
    {
        return RadiusRange != 0;
    }
}
