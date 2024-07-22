using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "DecodingParameters", menuName = "UI/Text/DecodingParameters")]
public class DecodingParameters : ScriptableObject
{
    [Header("GENERAL PARAMETERS")]
    [SerializeField] public float updateDecodeTime;
    [SerializeField] public float updateCharIndexTime;
    [SerializeField] public bool respectSpaces;
    [SerializeField] public bool ignoreTags;
    [SerializeField] public bool cutCodedText;
    [SerializeField] public int forwardRevealAmount;
    [SerializeField] public int startDecodingIndex;

    [Header("DECODING DICTIONERIES")]
    [SerializeField] public bool lowerCase;
    [SerializeField] public bool upperCase;
    [SerializeField] public bool digits;
    [SerializeField] public bool symbols;
}
