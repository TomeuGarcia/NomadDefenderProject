using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BodyPartsLibraryContent", 
    menuName = SOAssetPaths.TURRET_PARTS_LIBRARIES + "BodyPartsLibraryContent")]
public class BodyPartsLibraryContent : ScriptableObject
{
    [Header("BODIES")]
    [SerializeField] private PartsLibrary.PartsByProgressionState<TurretPartBody> _earlyBodies;
    [SerializeField] private PartsLibrary.PartsByProgressionState<TurretPartBody> _midBodies;
    [SerializeField] private PartsLibrary.PartsByProgressionState<TurretPartBody> _lateBodies;

    private void OnValidate()
    {
        _earlyBodies.progressionState = NodeEnums.ProgressionState.EARLY;
        _midBodies.progressionState = NodeEnums.ProgressionState.MID;
        _lateBodies.progressionState = NodeEnums.ProgressionState.LATE;
    }

    public PartsLibrary.PartsByProgressionState<TurretPartBody>[] GetArrayByProgression()
    {
        return new PartsLibrary.PartsByProgressionState<TurretPartBody>[3] { _earlyBodies, _midBodies, _lateBodies };
    }

}
