using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BonusStatsPartsLibraryContents",
    menuName = SOAssetPaths.TURRET_PARTS_LIBRARIES + "BonusStatsPartsLibraryContent")]
public class BonusStatsPartsLibraryContent : ScriptableObject
{
    [Header("BONUS STATS")]
    [SerializeField] private PartsLibrary.BonusStatsByProgressionState earlyBonusStats;
    [SerializeField] private PartsLibrary.BonusStatsByProgressionState midBonusStats;
    [SerializeField] private PartsLibrary.BonusStatsByProgressionState lateBonusStats;

    private void OnValidate()
    {
        earlyBonusStats.progressionState = NodeEnums.ProgressionState.EARLY;
        midBonusStats.progressionState = NodeEnums.ProgressionState.MID;
        lateBonusStats.progressionState = NodeEnums.ProgressionState.LATE;
    }

    public PartsLibrary.BonusStatsByProgressionState[] GetArrayByProgression()
    {
        return new PartsLibrary.BonusStatsByProgressionState[3] { earlyBonusStats, midBonusStats, lateBonusStats };
    }
}
