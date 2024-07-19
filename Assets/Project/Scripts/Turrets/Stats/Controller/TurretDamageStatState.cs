using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretDamageStatState : ITurretDamageState
{
    private readonly CardStatConfig _stat;
    public StatValueBonus StatValueBonus { get; private set; }

    public TurretDamageStatState(CardStatConfig stat)
    {
        _stat = stat;
        StatValueBonus = new StatValueBonus();
    }


    public int BaseDamage => GetDamageByLevel(0);

    public string BaseDamageText => GetDamageByLevelText(0);

    public int GetDamageByLevel(int upgradeLevel)
    {
        return (int)_stat.ComputeValueByLevel(upgradeLevel, StatValueBonus.AccumulatedBonusSum);
    }

    public string GetDamageByLevelText(int upgradeLevel)
    {
        return GetDamageByLevel(upgradeLevel).ToString();
    }
}
