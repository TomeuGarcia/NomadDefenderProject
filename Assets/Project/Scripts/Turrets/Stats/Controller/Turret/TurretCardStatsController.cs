using System;
using System.Collections.Generic;
using UnityEngine;

public class TurretCardStatsController : ITurretStatsStateSource, ITurretStatsBonusController, IBuildingUpgradesController
{
    private TurretStatState _damageStatState;
    private TurretStatState _shotsPerSecondInvertedStatState;
    private TurretStatState _radiusRangeStatState;

    public ITurretStatState DamageStatState => _damageStatState;
    public ITurretStatState ShotsPerSecondInvertedStatState => _shotsPerSecondInvertedStatState;
    public ITurretStatState RadiusRangeStatState => _radiusRangeStatState;


    public TurretStatsSnapshot CurrentStats { get; private set; }
    public int CurrentUpgradeLevel { get; private set; }

    public Action OnStatsUpdated;



    public TurretCardStatsController(CardStatConfig damageStat, CardStatConfig shotsPerSecondStat, CardStatConfig radiusRangeStat)
    {
        _damageStatState = new TurretStatState(damageStat, new TurretDamageStatStateValuePerser());
        _shotsPerSecondInvertedStatState = new TurretStatState(shotsPerSecondStat, new TurretShotsPerSecondStatStateValuePerser());
        _radiusRangeStatState = new TurretStatState(radiusRangeStat, new TurretRadiusRangeStatStateValuePerser());

        CurrentUpgradeLevel = 0;
        InitCurrentStats();
    }
    

    public TurretCardStatsController(TurretCardStatsController statsControllerToCopy,
        CardStatConfig damageStat, CardStatConfig shotsPerSecondStat, CardStatConfig radiusRangeStat)
    {
        _damageStatState = new TurretStatState(damageStat, statsControllerToCopy._damageStatState.BaseStatMultiplicationBonus, 
            new TurretDamageStatStateValuePerser());
        _shotsPerSecondInvertedStatState = new TurretStatState(shotsPerSecondStat, statsControllerToCopy._shotsPerSecondInvertedStatState.BaseStatMultiplicationBonus, 
            new TurretShotsPerSecondStatStateValuePerser());
        _radiusRangeStatState = new TurretStatState(radiusRangeStat, statsControllerToCopy._radiusRangeStatState.BaseStatMultiplicationBonus,
            new TurretRadiusRangeStatStateValuePerser());

        CurrentUpgradeLevel = 0;
        InitCurrentStats();        
    }
    

    private void InitCurrentStats()
    {
        CurrentStats = new TurretStatsSnapshot(
            (int)_damageStatState.GetValueByLevel(CurrentUpgradeLevel),
            _shotsPerSecondInvertedStatState.GetValueByLevel(CurrentUpgradeLevel),
            _radiusRangeStatState.GetValueByLevel(CurrentUpgradeLevel)
        );
    }
    public void UpdateCurrentStats()
    {
        CurrentStats.Reset(
            (int)_damageStatState.GetValueByLevel(CurrentUpgradeLevel),
            _shotsPerSecondInvertedStatState.GetValueByLevel(CurrentUpgradeLevel),
            _radiusRangeStatState.GetValueByLevel(CurrentUpgradeLevel)
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
        UpdateCurrentStats();
    }

    public void AddBonusBaseStatsMultiplication(TurretStatsMultiplicationSnapshot bonusStatsMultiplication)
    {
        if (bonusStatsMultiplication.HasDamage())
        {
            _damageStatState.BaseStatMultiplicationBonus.AddBonus(bonusStatsMultiplication.DamagePer1);
        }
        if (bonusStatsMultiplication.HasShotsPerSecond())
        {
            _shotsPerSecondInvertedStatState.BaseStatMultiplicationBonus.AddBonus(bonusStatsMultiplication.ShotsPerSecondPer1);
        }
        if (bonusStatsMultiplication.HasRadiusRange())
        {
            _radiusRangeStatState.BaseStatMultiplicationBonus.AddBonus(bonusStatsMultiplication.RadiusRangePer1);
        }

        UpdateCurrentStats();
    }

    public void RemoveBonusBaseStatsMultiplication(TurretStatsMultiplicationSnapshot bonusStatsMultiplication)
    {
        if (bonusStatsMultiplication.HasDamage())
        {
            _damageStatState.BaseStatMultiplicationBonus.RemoveBonus(bonusStatsMultiplication.DamagePer1);
        }
        if (bonusStatsMultiplication.HasShotsPerSecond())
        {
            _shotsPerSecondInvertedStatState.BaseStatMultiplicationBonus.RemoveBonus(bonusStatsMultiplication.ShotsPerSecondPer1);
        }
        if (bonusStatsMultiplication.HasRadiusRange())
        {
            _radiusRangeStatState.BaseStatMultiplicationBonus.RemoveBonus(bonusStatsMultiplication.RadiusRangePer1);
        }

        UpdateCurrentStats();
    }

}
