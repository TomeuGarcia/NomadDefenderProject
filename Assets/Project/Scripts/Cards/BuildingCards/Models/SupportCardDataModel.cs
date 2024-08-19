using System;
using UnityEngine;


[CreateAssetMenu(fileName = "NewSupportCardDataModel", 
    menuName = SOAssetPaths.CARDS_BUILDINGS + "SupportCardDataModel")]
public class SupportCardDataModel : ScriptableObject
{
    [Header("COST")]
    [SerializeField, Min(0)] public int cardCost;

    [Header("PARTS")] 
    [SerializeField] private SupportCardPartsGroup _partsGroup;
    public SupportCardPartsGroup SharedPartsGroup => _partsGroup;
    public SupportCardPartsGroup MakePartsGroup() => new SupportCardPartsGroup(_partsGroup);
}