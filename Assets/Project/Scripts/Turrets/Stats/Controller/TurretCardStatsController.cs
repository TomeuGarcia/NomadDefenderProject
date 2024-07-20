using System;
using System.Collections.Generic;
using UnityEngine;

public class TurretCardStatsController : ITurretStatsStateSource, ITurretStatsBonusController
{
    private readonly TurretDamageStatState _damageStatState;
    private readonly TurretShotsPerSecondStatState _shotsPerSecondStatState;
    private readonly TurretRadiusRangeStatState _radiusRangeStatState;
    
    public ITurretDamageState DamageStatState => _damageStatState;
    public ITurretShotsPerSecondState ShotsPerSecondStatState => _shotsPerSecondStatState;
    public ITurretRadiusRangeState RadiusRangeStatState => _radiusRangeStatState;

    public Action OnStatsUpdatedWithBonusEvent;

    public TurretStatsSnapshot CurrentStats { get; private set; }


    public TurretCardStatsController(CardStatConfig damageStat, CardStatConfig shotsPerSecondStat, CardStatConfig radiusRangeStat)
    {
        _damageStatState = new TurretDamageStatState(damageStat);
        _shotsPerSecondStatState = new TurretShotsPerSecondStatState(shotsPerSecondStat);
        _radiusRangeStatState = new TurretRadiusRangeStatState(radiusRangeStat);
    }

    public TurretStatsSnapshot MakeStatsSnapshotFromLevel(int upgradeLevel)
    {
        return new TurretStatsSnapshot(
            _damageStatState.GetDamageByLevel(upgradeLevel),
            _shotsPerSecondStatState.GetShotsPerSecondByLevel(upgradeLevel),
            _radiusRangeStatState.GetRadiusRangeByLevel(upgradeLevel)
        );
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

        OnStatsUpdatedWithBonus();
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

        OnStatsUpdatedWithBonus();
    }

    private void OnStatsUpdatedWithBonus()
    {
        OnStatsUpdatedWithBonusEvent?.Invoke();
    }
}
