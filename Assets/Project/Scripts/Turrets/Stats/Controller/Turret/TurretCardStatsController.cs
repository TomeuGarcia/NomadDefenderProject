using System;
using System.Collections.Generic;
using UnityEngine;

public class TurretCardStatsController : ITurretStatsStateSource, ITurretStatsBonusController, IBuildingUpgradesController
{
    private readonly TurretDamageStatState _damageStatState;
    private readonly TurretShotsPerSecondStatState _shotsPerSecondStatState;
    private readonly TurretRadiusRangeStatState _radiusRangeStatState;
    
    public ITurretDamageState DamageStatState => _damageStatState;
    public ITurretShotsPerSecondState ShotsPerSecondStatState => _shotsPerSecondStatState;
    public ITurretRadiusRangeState RadiusRangeStatState => _radiusRangeStatState;


    public TurretStatsSnapshot CurrentStats { get; private set; }
    public int CurrentUpgradeLevel { get; private set; }

    public Action OnStatsUpdated;



    public TurretCardStatsController(CardStatConfig damageStat, CardStatConfig shotsPerSecondStat, CardStatConfig radiusRangeStat)
    {
        _damageStatState = new TurretDamageStatState(damageStat);
        _shotsPerSecondStatState = new TurretShotsPerSecondStatState(shotsPerSecondStat);
        _radiusRangeStatState = new TurretRadiusRangeStatState(radiusRangeStat);

        CurrentUpgradeLevel = 0;
        CurrentStats = new TurretStatsSnapshot(
            _damageStatState.GetDamageByLevel(CurrentUpgradeLevel),
            _shotsPerSecondStatState.GetShotsPerSecondByLevel(CurrentUpgradeLevel),
            _radiusRangeStatState.GetRadiusRangeByLevel(CurrentUpgradeLevel)
        );
    }

    private void UpdateCurrentStats()
    {
        CurrentStats.Reset(
            _damageStatState.GetDamageByLevel(CurrentUpgradeLevel),
            _shotsPerSecondStatState.GetShotsPerSecondByLevel(CurrentUpgradeLevel),
            _radiusRangeStatState.GetRadiusRangeByLevel(CurrentUpgradeLevel)
        );
        
        OnStatsUpdated?.Invoke();
    }

    public void IncrementUpgradeLevel()
    {
        ++CurrentUpgradeLevel;
        UpdateCurrentStats();
    }

    public void ResetUpgradeLevel()
    {
        CurrentUpgradeLevel = 0;
    }

    public void AddBonusStatsMultiplication(TurretStatsMultiplicationSnapshot bonusStatsMultiplication)
    {
        if (bonusStatsMultiplication.HasDamage())
        {
            _damageStatState.BaseStatMultiplicationBonus.AddBonus(bonusStatsMultiplication.DamagePer1);
        }
        if (bonusStatsMultiplication.HasShotsPerSecond())
        {
            _shotsPerSecondStatState.BaseStatMultiplicationBonus.AddBonus(bonusStatsMultiplication.ShotsPerSecondPer1);
        }
        if (bonusStatsMultiplication.HasRadiusRange())
        {
            _radiusRangeStatState.BaseStatMultiplicationBonus.AddBonus(bonusStatsMultiplication.RadiusRangePer1);
        }

        UpdateCurrentStats();
    }

    public void RemoveBonusStatsMultiplication(TurretStatsMultiplicationSnapshot bonusStatsMultiplication)
    {
        if (bonusStatsMultiplication.HasDamage())
        {
            _damageStatState.BaseStatMultiplicationBonus.RemoveBonus(bonusStatsMultiplication.DamagePer1);
        }
        if (bonusStatsMultiplication.HasShotsPerSecond())
        {
            _shotsPerSecondStatState.BaseStatMultiplicationBonus.RemoveBonus(bonusStatsMultiplication.ShotsPerSecondPer1);
        }
        if (bonusStatsMultiplication.HasRadiusRange())
        {
            _radiusRangeStatState.BaseStatMultiplicationBonus.RemoveBonus(bonusStatsMultiplication.RadiusRangePer1);
        }

        UpdateCurrentStats();
    }

}
