using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretDamageStatState : ITurretDamageState
{
    private readonly CardStatConfig _stat;
    public StatValueBonus BaseStatMultiplicationBonus { get; private set; }

    public TurretDamageStatState(CardStatConfig stat)
    {
        _stat = stat;
        BaseStatMultiplicationBonus = new StatValueBonus();
    }
    
    public TurretDamageStatState(CardStatConfig stat, StatValueBonus baseStatMultiplicationBonusToCopy)
    {
        _stat = ScriptableObject.Instantiate(stat);
        BaseStatMultiplicationBonus = new StatValueBonus(baseStatMultiplicationBonusToCopy);
    }


    public int BaseDamage => GetDamageByLevel(0);

    public string BaseDamageText => GetDamageByLevelText(0);

    public int GetDamageByLevel(int upgradeLevel)
    {
        return (int)_stat.ComputeValueByLevel(upgradeLevel, BaseStatMultiplicationBonus.AccumulatedBonusSum);
    }

    public string GetDamageByLevelText(int upgradeLevel)
    {
        return GetDamageByLevel(upgradeLevel).ToString();
    }
}
