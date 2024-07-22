using System;
using System.Collections.Generic;
using UnityEngine;

public class SupportCardStatsController : ISupportStatsStateSource, IBuildingUpgradesController
{
    private readonly TurretRadiusRangeStatState _radiusRangeStatState;

    public ITurretRadiusRangeState RadiusRangeStatState => _radiusRangeStatState;

    public SupportStatsSnapshot CurrentStats { get; private set; }

    public int CurrentUpgradeLevel { get; private set; }

    public Action OnStatsUpdated;



    public SupportCardStatsController(CardStatConfig radiusRangeStat)
    {
        _radiusRangeStatState = new TurretRadiusRangeStatState(radiusRangeStat);

        CurrentUpgradeLevel = 0;
        CurrentStats = new SupportStatsSnapshot(
            _radiusRangeStatState.GetRadiusRangeByLevel(CurrentUpgradeLevel)
        );
    }

    public void IncrementUpgradeLevel()
    {
        ++CurrentUpgradeLevel;
        UpdateCurrentStats();
    }

    private void UpdateCurrentStats()
    {
        CurrentStats.Reset(
            _radiusRangeStatState.GetRadiusRangeByLevel(CurrentUpgradeLevel)
        );

        OnStatsUpdated?.Invoke();
    }
}
