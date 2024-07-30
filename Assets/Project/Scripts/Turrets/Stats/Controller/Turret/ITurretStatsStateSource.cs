using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITurretStatsStateSource
{
    public ITurretStatState DamageStatState { get; }
    public ITurretStatState ShotsPerSecondInvertedStatState { get; }
    public ITurretStatState RadiusRangeStatState { get; }
}
