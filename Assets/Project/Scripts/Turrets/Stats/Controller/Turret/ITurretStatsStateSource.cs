using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITurretStatsStateSource
{
    public ITurretDamageState DamageStatState { get; }
    public ITurretShotsPerSecondState ShotsPerSecondStatState { get; }
    public ITurretRadiusRangeState RadiusRangeStatState { get; }
}
