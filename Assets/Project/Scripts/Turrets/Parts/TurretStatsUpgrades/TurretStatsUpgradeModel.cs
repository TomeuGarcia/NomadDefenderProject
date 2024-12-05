using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[CreateAssetMenu(fileName = "TurretStatsUpgradeModel_NAME",
    menuName = SOAssetPaths.TURRET_PARTS_BONUSSTATS + "TurretStatsUpgradeModel")]
public class TurretStatsUpgradeModel : ScriptableObject
{
    public struct StatString
    {
        public bool IsNull { get; private set; }
        public string Value { get; private set; }

        public StatString(int value, bool isPercent, int nullValue = 0)
        {
            IsNull = value == nullValue;
            Value = (value < 0 ? "-" : "+") + Mathf.Abs(value);
            if (isPercent) Value += '%';
        }

        public void OverwriteValue(string value)
        {
            Value = value;
        }
    }



    [Header("BONUS PERCENTS")]
    [SerializeField] private int _damageMultiplier = 20;
    [SerializeField] private int _shotsPerSecondMultiplier = 20;
    [SerializeField] private int _radiusRangeMultiplier = 20;

    [Header("BONUS FLATS")] 
    [SerializeField, Range(1, -2)] private int _extraLevels = 1;
    [SerializeField] private int _extraPlayCost = 0;
    
    public int ExtraLevels => _extraLevels;
    public int ExtraPlayCost => _extraPlayCost;
    

    public TurretStatsMultiplicationSnapshot MakeStatMultiplicationSnapshot()
    {
        return new TurretStatsMultiplicationSnapshot(
            Per100StatMultiplierToPer1(_damageMultiplier),
            Per100StatMultiplierToPer1(_shotsPerSecondMultiplier),
            Per100StatMultiplierToPer1(_radiusRangeMultiplier)
        );
    }

    private float Per100StatMultiplierToPer1(int statMultiplier)
    {
        return statMultiplier / 100f;
    }

    public void MakeStatStrings(out StatString damageStat, out StatString shotsPerSecondStat, out StatString radiusRangeStat,
        out StatString extraLevels, out StatString extraPlayCost)
    {
        damageStat = new StatString(_damageMultiplier, true);
        shotsPerSecondStat = new StatString(_shotsPerSecondMultiplier, true);
        radiusRangeStat = new StatString(_radiusRangeMultiplier, true);

        extraLevels = new StatString(_extraLevels, false,1);
        if (_extraLevels == 0) extraLevels.OverwriteValue("No Lvl UPG");
        
        extraPlayCost = new StatString(_extraPlayCost, false);
    }




}
