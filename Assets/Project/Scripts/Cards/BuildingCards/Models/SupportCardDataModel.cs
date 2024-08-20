using System;
using UnityEngine;
using UnityEngine.Serialization;


[CreateAssetMenu(fileName = "NewSupportCardDataModel", 
    menuName = SOAssetPaths.CARDS_BUILDINGS + "SupportCardDataModel")]
public class SupportCardDataModel : ScriptableObject
{
    [Header("PLAY COST")]
    [SerializeField, Min(0)] private int _cardPlayCost;

    [Header("PARTS")] 
    [SerializeField] private SupportCardPartsGroup _partsGroup;
    
    
    public int CardPlayCost => _cardPlayCost;
    public SupportCardPartsGroup SharedPartsGroup => _partsGroup;
    public SupportCardPartsGroup MakePartsGroup() => new SupportCardPartsGroup(_partsGroup);
}