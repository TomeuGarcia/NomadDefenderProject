using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewOWMapGenerationSettings", menuName = "Map/OWMapGenerationSettings")]
public class OWMapGenerationSettings : ScriptableObject
{
    [Header("MAP LENGTH")]
    [SerializeField, Min(3)] public int numberOfLevels = 15;    

    [Header("WIDTH & GROW/SHRINK")]
    [SerializeField, Min(1)] public int maxWidthGrowStep = 1;
    [SerializeField, Min(1)] public int maxWidthShrinkStep = 1;
    [SerializeField, Min(2)] public int maxWidth = 3;
    [SerializeField, Min(1)] public int minWidth = 1;

    [Header("1 WIDTH CONTROL")]
    [SerializeField, Min(1)] public int numberOf1WidthStartLevels = 1;
    [SerializeField, Min(1)] public int numberOf1WidthEndLevels = 2;
    [SerializeField, Min(1), Tooltip("Excluding numberOf1WidthStartLevels & numberOf1WidthEndLevels")] public int maxNum1Width = 2;
    [SerializeField, Min(1)] public int maxChained1Width = 1;

    [Header("ADDITIONAL WIDTH CONTROL")]
    [SerializeField, Min(2)] public int maxConnectionsPerNode = 2;
    [SerializeField, Min(2)] public int maxSameWidthRepeatTimes = 3;

    [Header("\n\nDIAGONAL CONNECTIONS")]
    [SerializeField, Range(0.0f, 1.0f)] public float diagonalNoConnectionThreshold = 0.20f;
    [SerializeField, Range(0.0f, 1.0f)] public float diagonalUpRightConnectionThreshold = 0.60f;
    [SerializeField, Range(0.0f, 1.0f)] public float diagonalUpLeftConnectionThreshold = 1.0f;

    [Header("REMOVE CONNECTION")]
    [SerializeField, Range(0.0f, 1.0f)] public float removeConnectionThreshold = 0.30f;
}
