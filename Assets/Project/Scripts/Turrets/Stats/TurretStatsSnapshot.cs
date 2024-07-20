using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretStatsSnapshot
{
    public int Damage { get; private set; }
    public float ShotsPerSecond { get; private set; }
    public float ShotsPerSecondInverted { get; private set; }
    public float RadiusRange { get; private set; }

    public TurretStatsSnapshot(int damage, float shotsPerSecond, float radiusRange)
    {
        Reset(damage, shotsPerSecond, radiusRange);
    }

    public void Reset(int damage, float shotsPerSecond, float radiusRange)
    {
        Damage = damage;
        ShotsPerSecond = shotsPerSecond;
        RadiusRange = radiusRange;
        ShotsPerSecondInverted = 1f / shotsPerSecond;
    }

    public bool HasDamage()
    {
        return Damage != 0;
    }
    public bool HasShotsPerSecond()
    {
        return ShotsPerSecond != 0;
    }
    public bool HasRadiusRange()
    {
        return RadiusRange != 0;
    }
}
