using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "BonusStatsPartsLibraryContents",
    menuName = SOAssetPaths.TURRET_PARTS_LIBRARIES + "BonusStatsPartsLibraryContent")]
public class BonusStatsPartsLibraryContent : ScriptableObject
{
    [System.Serializable]
    private class ByStat
    {
        [SerializeField] private PartsLibrary.PartsByProgressionState<TurretStatsUpgradeModel> _earlyBonusStats;
        [SerializeField] private PartsLibrary.PartsByProgressionState<TurretStatsUpgradeModel> _midBonusStats;
        [SerializeField] private PartsLibrary.PartsByProgressionState<TurretStatsUpgradeModel> _lateBonusStats;
        
        public PartsLibrary.PartsByProgressionState<TurretStatsUpgradeModel> EarlyBonusStats => _earlyBonusStats;
        public PartsLibrary.PartsByProgressionState<TurretStatsUpgradeModel> MidBonusStats => _midBonusStats;
        public PartsLibrary.PartsByProgressionState<TurretStatsUpgradeModel> LateBonusStats => _lateBonusStats;

        public void OnValidate()
        {
            _earlyBonusStats.progressionState = NodeEnums.ProgressionState.EARLY;
            _midBonusStats.progressionState = NodeEnums.ProgressionState.MID;
            _lateBonusStats.progressionState = NodeEnums.ProgressionState.LATE;
        }
    }
    
    

    [Header("DAMAGE")]
    [SerializeField] private ByStat _bonusStats_Damage;
    [Space(20)]
    
    [Header("SHOTS PER SECOND")]
    [SerializeField] private ByStat _bonusStats_ShotsPerSecond;
    [Space(20)]
    
    [Header("RANGE")]
    [SerializeField] private ByStat _bonusStats_Range;

    
    
    private void OnValidate()
    {
        _bonusStats_Damage.OnValidate();
        _bonusStats_ShotsPerSecond.OnValidate();
        _bonusStats_Range.OnValidate();
    }
    
    
    public PartsLibrary.PartsByProgressionState<TurretStatsUpgradeModel>[] GetArrayByProgression_Damage()
    {
        return new PartsLibrary.PartsByProgressionState<TurretStatsUpgradeModel>[3]
        {
            _bonusStats_Damage.EarlyBonusStats, 
            _bonusStats_Damage.MidBonusStats, 
            _bonusStats_Damage.LateBonusStats
        };
    }
    
    public PartsLibrary.PartsByProgressionState<TurretStatsUpgradeModel>[] GetArrayByProgression_ShotsPerSecond()
    {
        return new PartsLibrary.PartsByProgressionState<TurretStatsUpgradeModel>[3]
        {
            _bonusStats_ShotsPerSecond.EarlyBonusStats, 
            _bonusStats_ShotsPerSecond.MidBonusStats, 
            _bonusStats_ShotsPerSecond.LateBonusStats
        };
    }
    
    public PartsLibrary.PartsByProgressionState<TurretStatsUpgradeModel>[] GetArrayByProgression_Range()
    {
        return new PartsLibrary.PartsByProgressionState<TurretStatsUpgradeModel>[3]
        {
            _bonusStats_Range.EarlyBonusStats, 
            _bonusStats_Range.MidBonusStats, 
            _bonusStats_Range.LateBonusStats
        };
    }
}
