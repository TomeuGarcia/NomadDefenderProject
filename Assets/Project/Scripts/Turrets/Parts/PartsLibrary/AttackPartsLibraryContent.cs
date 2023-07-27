using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AttackPartsLibraryContent", menuName = "TurretParts/AttackPartsLibraryContent")]
public class AttackPartsLibraryContent : ScriptableObject
{
    [Header("ATTACKS")]
    [SerializeField] private PartsLibrary.AttacksByProgressionState earlyAttacks;
    [SerializeField] private PartsLibrary.AttacksByProgressionState midAttacks;
    [SerializeField] private PartsLibrary.AttacksByProgressionState lateAttacks;

    private void OnValidate()
    {
        earlyAttacks.progressionState = NodeEnums.ProgressionState.EARLY;
        midAttacks.progressionState = NodeEnums.ProgressionState.MID;
        lateAttacks.progressionState = NodeEnums.ProgressionState.LATE;
    }

    public PartsLibrary.AttacksByProgressionState[] GetArrayByProgression()
    {
        return new PartsLibrary.AttacksByProgressionState[3] { earlyAttacks, midAttacks, lateAttacks };
    }


}
