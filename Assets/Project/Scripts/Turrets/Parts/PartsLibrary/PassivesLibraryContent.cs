using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PassivesLibraryContent", 
    menuName = SOAssetPaths.TURRET_PARTS_LIBRARIES + "PassivesLibraryContent")]
public class PassivesLibraryContent : ScriptableObject
{
    [Header("BASES & PASSIVES")]
    [SerializeField] private PartsLibrary.PartsByProgressionState<TurretPassiveBase> _earlyBases;
    [SerializeField] private PartsLibrary.PartsByProgressionState<TurretPassiveBase> _midBases;
    [SerializeField] private PartsLibrary.PartsByProgressionState<TurretPassiveBase> _lateBases;

    private void OnValidate()
    {
        _earlyBases.progressionState = NodeEnums.ProgressionState.EARLY;
        _midBases.progressionState = NodeEnums.ProgressionState.MID;
        _lateBases.progressionState = NodeEnums.ProgressionState.LATE;
    }

    public PartsLibrary.PartsByProgressionState<TurretPassiveBase>[] GetArrayByProgression()
    {
        return new PartsLibrary.PartsByProgressionState<TurretPassiveBase>[3] { _earlyBases, _midBases, _lateBases };
    }

}