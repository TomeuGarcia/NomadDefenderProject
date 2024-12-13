using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatValueBonus
{
    public float AccumulatedBonusMultiplier { get; private set; }

    public StatValueBonus()
    {
        AccumulatedBonusMultiplier = 1f;
    }
    public StatValueBonus(StatValueBonus other)
    {
        AccumulatedBonusMultiplier = other.AccumulatedBonusMultiplier;
    }
    
    public void AddBonus(float bonus)
    {
        AccumulatedBonusMultiplier *= (1 + bonus);
        ClampAccumulatedBonusMultiplier();
    }
    
    public void RemoveBonus(float bonus)
    {
        AccumulatedBonusMultiplier /= (1 + bonus);
        ClampAccumulatedBonusMultiplier();
    }

    private void ClampAccumulatedBonusMultiplier()
    {
        AccumulatedBonusMultiplier = Mathf.Max(AccumulatedBonusMultiplier, 0.1f);
    }
}