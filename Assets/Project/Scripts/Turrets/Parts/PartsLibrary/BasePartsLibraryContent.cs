using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BasePartsLibraryContent", 
    menuName = SOAssetPaths.TURRET_PARTS_LIBRARIES + "BasePartsLibraryContent")]
public class BasePartsLibraryContent : ScriptableObject
{
    [Header("BASES & PASSIVES")]
    [SerializeField] private PartsLibrary.BasesAndPassivesByProgressionState earlyBases;
    [SerializeField] private PartsLibrary.BasesAndPassivesByProgressionState midBases;
    [SerializeField] private PartsLibrary.BasesAndPassivesByProgressionState lateBases;

    private void OnValidate()
    {
        earlyBases.progressionState = NodeEnums.ProgressionState.EARLY;
        midBases.progressionState = NodeEnums.ProgressionState.MID;
        lateBases.progressionState = NodeEnums.ProgressionState.LATE;
    }

    public PartsLibrary.BasesAndPassivesByProgressionState[] GetArrayByProgression()
    {
        return new PartsLibrary.BasesAndPassivesByProgressionState[3] { earlyBases, midBases, lateBases };
    }

}