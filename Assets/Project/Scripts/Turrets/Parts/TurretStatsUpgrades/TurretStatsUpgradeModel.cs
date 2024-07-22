using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TurretStatsUpgradeModel_NAME",
    menuName = SOAssetPaths.TURRET_PARTS_BONUSSTATS + "TurretStatsUpgradeModel")]
public class TurretStatsUpgradeModel : ScriptableObject
{
    public struct StatString
    {
        public bool IsNull { get; private set; }
        public string Value { get; private set; }

        public StatString(int value)
        {
            IsNull = value == 0;
            Value = (value < 0 ? "-" : "+") + ' ' + Mathf.Abs(value).ToString() + '%';
        }
    }



    [Header("BONUS PERCENTS")]
    [SerializeField] private int _damageMultiplier = 20;
    [SerializeField] private int _shotsPerSecondMultiplier = 20;
    [SerializeField] private int _radiusRangeMultiplier = 20;


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

    public void MakeStatStrings(out StatString damageStat, out StatString shotsPerSecondStat, out StatString radiusRangeStat)
    {
        damageStat = new StatString(_damageMultiplier);
        shotsPerSecondStat = new StatString(_shotsPerSecondMultiplier);
        radiusRangeStat = new StatString(_radiusRangeMultiplier);
    }




}
