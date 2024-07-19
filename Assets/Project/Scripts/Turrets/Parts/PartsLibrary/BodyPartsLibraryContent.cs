using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BodyPartsLibraryContent", 
    menuName = SOAssetPaths.TURRET_PARTS_LIBRARIES + "BodyPartsLibraryContent")]
public class BodyPartsLibraryContent : ScriptableObject
{
    [Header("BODIES")]
    [SerializeField] private PartsLibrary.BodiesByProgressionState earlyBodies;
    [SerializeField] private PartsLibrary.BodiesByProgressionState midBodies;
    [SerializeField] private PartsLibrary.BodiesByProgressionState lateBodies;

    private void OnValidate()
    {
        earlyBodies.progressionState = NodeEnums.ProgressionState.EARLY;
        midBodies.progressionState = NodeEnums.ProgressionState.MID;
        lateBodies.progressionState = NodeEnums.ProgressionState.LATE;
    }

    public PartsLibrary.BodiesByProgressionState[] GetArrayByProgression()
    {
        return new PartsLibrary.BodiesByProgressionState[3] { earlyBodies, midBodies, lateBodies };
    }

}
