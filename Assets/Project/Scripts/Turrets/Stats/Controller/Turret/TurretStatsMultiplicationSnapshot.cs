using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 [System.Serializable]
public class TurretStatsMultiplicationSnapshot
{
    public float DamagePer1 { get; private set; }
    public float ShotsPerSecondPer1 { get; private set; }
    public float RadiusRangePer1 { get; private set; }

    public TurretStatsMultiplicationSnapshot(float damagePer1, float shotsPerSecondPer1, float radiusRangePer1)
    {
        Reset(damagePer1, shotsPerSecondPer1, radiusRangePer1);
    }

    public void Reset(float damagePer1, float shotsPerSecondPer1, float radiusRangePer1)
    {
        DamagePer1 = damagePer1;
        ShotsPerSecondPer1 = shotsPerSecondPer1;
        RadiusRangePer1 = radiusRangePer1;
    }

    public bool HasDamage()
    {
        return DamagePer1 != 0f;
    }
    public bool HasShotsPerSecond()
    {
        return ShotsPerSecondPer1 != 0f;
    }
    public bool HasRadiusRange()
    {
        return RadiusRangePer1 != 0f;
    }
}
