using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AttackPartsLibraryContent", 
    menuName = SOAssetPaths.TURRET_PARTS_LIBRARIES + "AttackPartsLibraryContent")]
public class AttackPartsLibraryContent : ScriptableObject
{
    [Header("ATTACKS")]
    [SerializeField] private PartsLibrary.PartsByProgressionState<TurretPartAttack> _earlyAttacks;
    [SerializeField] private PartsLibrary.PartsByProgressionState<TurretPartAttack> _midAttacks;
    [SerializeField] private PartsLibrary.PartsByProgressionState<TurretPartAttack> _lateAttacks;

    private void OnValidate()
    {
        _earlyAttacks.progressionState = NodeEnums.ProgressionState.EARLY;
        _midAttacks.progressionState = NodeEnums.ProgressionState.MID;
        _lateAttacks.progressionState = NodeEnums.ProgressionState.LATE;
    }

    public PartsLibrary.PartsByProgressionState<TurretPartAttack>[] GetArrayByProgression()
    {
        return new PartsLibrary.PartsByProgressionState<TurretPartAttack>[3] { _earlyAttacks, _midAttacks, _lateAttacks };
    }


}
