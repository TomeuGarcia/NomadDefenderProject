using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;


[CreateAssetMenu(fileName = "NewSupportCardDataModel", 
    menuName = SOAssetPaths.CARDS_BUILDINGS + "SupportCardDataModel")]
public class SupportCardDataModel : ScriptableObject
{
    [Header("PLAY COST")]
    [SerializeField, Min(0)] private int _cardPlayCost;
    [SerializeField] private BuildingSellingModel _buildingSelling;

    [Header("PARTS")] 
    [SerializeField] private SupportCardPartsGroup _partsGroup;
    
    
    public int CardPlayCost => _cardPlayCost;
    public SupportCardPartsGroup SharedPartsGroup => _partsGroup;
    public BuildingSellingConfig MakeBuildingSellingConfig() => new BuildingSellingConfig(_buildingSelling.Config);

    public SupportCardPartsGroup MakePartsGroup()
    {
        return new SupportCardPartsGroup(_partsGroup);
    }
}