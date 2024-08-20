using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BonusStatsPartsLibraryContents",
    menuName = SOAssetPaths.TURRET_PARTS_LIBRARIES + "BonusStatsPartsLibraryContent")]
public class BonusStatsPartsLibraryContent : ScriptableObject
{
    [Header("BONUS STATS")]
    [SerializeField] private PartsLibrary.PartsByProgressionState<TurretStatsUpgradeModel> _earlyBonusStats;
    [SerializeField] private PartsLibrary.PartsByProgressionState<TurretStatsUpgradeModel> _midBonusStats;
    [SerializeField] private PartsLibrary.PartsByProgressionState<TurretStatsUpgradeModel> _lateBonusStats;

    private void OnValidate()
    {
        _earlyBonusStats.progressionState = NodeEnums.ProgressionState.EARLY;
        _midBonusStats.progressionState = NodeEnums.ProgressionState.MID;
        _lateBonusStats.progressionState = NodeEnums.ProgressionState.LATE;
    }

    public PartsLibrary.PartsByProgressionState<TurretStatsUpgradeModel>[] GetArrayByProgression()
    {
        return new PartsLibrary.PartsByProgressionState<TurretStatsUpgradeModel>[3] { _earlyBonusStats, _midBonusStats, _lateBonusStats };
    }
}
