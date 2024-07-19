using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretCardStatsController : ITurretStatsBonusController
{
    private readonly TurretDamageStatState _damageStatState;
    private readonly TurretShotsPerSecondStatState _shotsPerSecondStatState;
    private readonly TurretRadiusRangeStatState _radiusRangeStatState;
    
    public ITurretDamageState DamageStatState => _damageStatState;
    public ITurretShotsPerSecondState ShotsPerSecondStatState => _shotsPerSecondStatState;
    public ITurretRadiusRangeState RadiusRangeStatState => _radiusRangeStatState;


    public TurretCardStatsController(CardStatConfig damageStat, CardStatConfig shotsPerSecondStat, CardStatConfig radiusRangeStat)
    {
        _damageStatState = new TurretDamageStatState(damageStat);
        _shotsPerSecondStatState = new TurretShotsPerSecondStatState(shotsPerSecondStat);
        _radiusRangeStatState = new TurretRadiusRangeStatState(radiusRangeStat);
    }


    public void AddBonusStats(TurretStatsSnapshot bonusStats)
    {
        if (bonusStats.HasDamage())
        {
            _damageStatState.StatValueBonus.AddBonus(bonusStats.Damage);
        }
        if (bonusStats.HasShotsPerSecond())
        {
            _shotsPerSecondStatState.StatValueBonus.AddBonus(bonusStats.ShotsPerSecond);
        }
        if (bonusStats.HasRadiusRange())
        {
            _radiusRangeStatState.StatValueBonus.AddBonus(bonusStats.RadiusRange);
        }
    }
    public void RemoveBonusStats(TurretStatsSnapshot bonusStats)
    {
        if (bonusStats.HasDamage())
        {
            _damageStatState.StatValueBonus.RemoveBonus(bonusStats.Damage);
        }
        if (bonusStats.HasShotsPerSecond())
        {
            _shotsPerSecondStatState.StatValueBonus.RemoveBonus(bonusStats.ShotsPerSecond);
        }
        if (bonusStats.HasRadiusRange())
        {
            _radiusRangeStatState.StatValueBonus.RemoveBonus(bonusStats.RadiusRange);
        }
    }


}
