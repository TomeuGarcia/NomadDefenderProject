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
    
    [SerializeField] private PartsLibrary.PartsByProgressionState<ATurretPassiveAbilityDataModel> _earlyPassives;
    [SerializeField] private PartsLibrary.PartsByProgressionState<ATurretPassiveAbilityDataModel> _midPassives;
    [SerializeField] private PartsLibrary.PartsByProgressionState<ATurretPassiveAbilityDataModel> _latePassives;

    private void OnValidate()
    {
        _earlyPassives.progressionState = NodeEnums.ProgressionState.EARLY;
        _midPassives.progressionState = NodeEnums.ProgressionState.MID;
        _latePassives.progressionState = NodeEnums.ProgressionState.LATE;
    }

    public PartsLibrary.PartsByProgressionState<ATurretPassiveAbilityDataModel>[] GetArrayByProgression()
    {
        return new PartsLibrary.PartsByProgressionState<ATurretPassiveAbilityDataModel>[3] { _earlyPassives, _midPassives, _latePassives };
    }

}