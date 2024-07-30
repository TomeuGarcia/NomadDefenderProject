using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretStatsSnapshot
{
    public int Damage { get; private set; }
    public float ShotsPerSecondInverted { 
        get => _shotsPerSecondInverted; 
        private set {
            _shotsPerSecondInverted = value;
            _hasShotsPerSecond = value.AlmostZero();
        } 
    }
    public float RadiusRange { get; private set; }

    private float _shotsPerSecondInverted;
    private bool _hasShotsPerSecond;


    public TurretStatsSnapshot(int damage, float shotsPerSecondInverted, float radiusRange)
    {
        Reset(damage, shotsPerSecondInverted, radiusRange);
    }

    public void Reset(int damage, float shotsPerSecondInverted, float radiusRange)
    {
        Damage = damage;
        RadiusRange = radiusRange;
        ShotsPerSecondInverted = shotsPerSecondInverted;
    }

    public bool HasDamage()
    {
        return Damage != 0;
    }
    public bool HasShotsPerSecond()
    {
        return _hasShotsPerSecond;
    }
    public bool HasRadiusRange()
    {
        return RadiusRange.AlmostZero();
    }
}
