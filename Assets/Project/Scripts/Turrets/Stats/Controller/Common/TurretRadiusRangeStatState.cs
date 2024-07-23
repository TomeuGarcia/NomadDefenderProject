using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretRadiusRangeStatState : ITurretRadiusRangeState
{
    private readonly CardStatConfig _stat;
    public StatValueBonus BaseStatMultiplicationBonus { get; private set; }

    public TurretRadiusRangeStatState(CardStatConfig stat)
    {
        _stat = stat;
        BaseStatMultiplicationBonus = new StatValueBonus();
    }
    public TurretRadiusRangeStatState(CardStatConfig stat, StatValueBonus baseStatMultiplicationBonus)
    {
        _stat = ScriptableObject.Instantiate(stat);
        BaseStatMultiplicationBonus = new StatValueBonus(baseStatMultiplicationBonus);
    }

    public float BaseRadiusRange => GetRadiusRangeByLevel(0);

    public string BaseRadiusRangeText => GetRadiusRangeByLevelText(0);

    public float GetRadiusRangeByLevel(int upgradeLevel)
    {
        return _stat.ComputeValueByLevel(upgradeLevel, BaseStatMultiplicationBonus.AccumulatedBonusSum);
    }

    public string GetRadiusRangeByLevelText(int upgradeLevel)
    {
        return GetRadiusRangeByLevel(upgradeLevel).ToString("0.0").Replace(',', '.');
    }
}
