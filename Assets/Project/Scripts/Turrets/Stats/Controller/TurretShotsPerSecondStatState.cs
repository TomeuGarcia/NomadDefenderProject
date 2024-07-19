using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretShotsPerSecondStatState : ITurretShotsPerSecondState
{
    private readonly CardStatConfig _stat;
    public StatValueBonus StatValueBonus { get; private set; }


    public TurretShotsPerSecondStatState(CardStatConfig stat)
    {
        _stat = stat;
        StatValueBonus = new StatValueBonus();
    }


    public float BaseShotsPerSecond => GetShotsPerSecondByLevel(0);

    public float BaseShotsPerSecondInverted => GetShotsPerSecondInvertedByLevel(0);

    public string BaseShotsPerSecondText => GetShotsPerSecondByLevelText(0);


    public float GetShotsPerSecondByLevel(int upgradeLevel)
    {
        return _stat.ComputeValueByLevel(upgradeLevel, StatValueBonus.AccumulatedBonusSum);
    }
    public float GetShotsPerSecondInvertedByLevel(int upgradeLevel)
    {
        return 1f / GetShotsPerSecondByLevel(upgradeLevel);
    }
    public string GetShotsPerSecondByLevelText(int upgradeLevel)
    {
        return GetShotsPerSecondByLevel(upgradeLevel).ToString().Replace(',', '.');
    }

}
