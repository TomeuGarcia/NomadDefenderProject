using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretStatState : ITurretStatState
{

    private readonly CardStatConfig _stat;
    private ITurretStatStateValuePerser _valueParser;
    public StatValueBonus BaseStatMultiplicationBonus { get; private set; }


    public TurretStatState(CardStatConfig stat, ITurretStatStateValuePerser valueParser)
    {
        _stat = stat;
        _valueParser = valueParser;
        BaseStatMultiplicationBonus = new StatValueBonus();
    }

    public TurretStatState(CardStatConfig stat, StatValueBonus baseStatMultiplicationBonusToCopy, ITurretStatStateValuePerser valueParser)
    {
        _stat = ScriptableObject.Instantiate(stat);
        BaseStatMultiplicationBonus = new StatValueBonus(baseStatMultiplicationBonusToCopy);
        _valueParser = valueParser;
    }



    public float BaseValue => GetValueByLevel(0);

    public string BaseValueText => GetValueTextByLevel(0);

    public float GetValueByLevel(int upgradeLevel)
    {
        return _valueParser.ParseValue(GetStatValueByLevel(upgradeLevel));
    }

    public string GetValueTextByLevel(int upgradeLevel)
    {
        return _valueParser.ParseValueToString(GetStatValueByLevel(upgradeLevel));
    }

    private float GetStatValueByLevel(int upgradeLevel)
    {
        return _stat.ComputeValueByLevel(upgradeLevel, BaseStatMultiplicationBonus.AccumulatedBonusSum);
    }
}
