using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatValueBonus
{
    private List<float> _valueBonuses;
    public float AccumulatedBonusSum { get; private set; }

    public StatValueBonus()
    {
        _valueBonuses = new List<float>();
        UpdateAccumulatedBonusSum();
    }

    private void UpdateAccumulatedBonusSum()
    {
        AccumulatedBonusSum = 0;
        foreach (float bonus in _valueBonuses)
        {
            AccumulatedBonusSum += bonus;
        }
    }

    public void AddBonus(float bonus)
    {
        _valueBonuses.Add(bonus);
        UpdateAccumulatedBonusSum();
    }
    
    public void RemoveBonus(float bonus)
    {
        _valueBonuses.Add(-bonus);
        UpdateAccumulatedBonusSum();
    }

}
