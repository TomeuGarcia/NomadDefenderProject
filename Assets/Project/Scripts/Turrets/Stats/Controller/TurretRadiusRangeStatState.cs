using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretRadiusRangeStatState : ITurretRadiusRangeState
{
    private readonly CardStatConfig _stat;
    public StatValueBonus StatValueBonus { get; private set; }

    public TurretRadiusRangeStatState(CardStatConfig stat)
    {
        _stat = stat;
        StatValueBonus = new StatValueBonus();
    }

    public float BaseRadiusRange => GetRadiusRangeByLevel(0);

    public string BaseRadiusRangeText => GetRadiusRangeByLevelText(0);

    public float GetRadiusRangeByLevel(int upgradeLevel)
    {
        return _stat.ComputeValueByLevel(upgradeLevel, StatValueBonus.AccumulatedBonusSum);
    }

    public string GetRadiusRangeByLevelText(int upgradeLevel)
    {
        return GetRadiusRangeByLevel(upgradeLevel).ToString().Replace(',', '.');
    }
}
