using System;
using System.Collections.Generic;
using UnityEngine;

public class SupportCardStatsController : ISupportStatsStateSource, IBuildingUpgradesController
{
    private readonly TurretStatState _radiusRangeStatState;

    public ITurretStatState RadiusRangeStatState => _radiusRangeStatState;

    public SupportStatsSnapshot CurrentStats { get; private set; }

    public int CurrentUpgradeLevel { get; private set; }

    public Action OnStatsUpdated;



    public SupportCardStatsController(CardStatConfig radiusRangeStat)
    {
        _radiusRangeStatState = new TurretStatState(radiusRangeStat, new TurretRadiusRangeStatStateValuePerser());

        CurrentUpgradeLevel = 0;
        CurrentStats = new SupportStatsSnapshot(
            _radiusRangeStatState.GetValueByLevel(CurrentUpgradeLevel)
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
            _radiusRangeStatState.GetValueByLevel(CurrentUpgradeLevel)
        );

        OnStatsUpdated?.Invoke();
    }
}
