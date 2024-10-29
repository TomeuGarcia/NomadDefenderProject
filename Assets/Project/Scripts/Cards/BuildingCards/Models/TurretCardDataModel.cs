using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;


[CreateAssetMenu(fileName = "NewTurretCardDataModel", 
    menuName = SOAssetPaths.CARDS_BUILDINGS + "TurretCardDataModel")]
public class TurretCardDataModel : ScriptableObject
{
    [Header("LEVEL")]
    [SerializeField, Range(MIN_CARD_LEVEL, MAX_CARD_LEVEL)] private int _cardLevel = 1;
    
    [Header("PLAY COST")]
    [SerializeField, Min(-10)] private int _cardPlayCost;
    [SerializeField] private BuildingSellingModel _buildingSelling;

    [Header("PARTS")] 
    [SerializeField] private TurretCardPartsGroup _partsGroup;
    [SerializeField] private ATurretPassiveAbilityDataModel[] _passiveAbilities = Array.Empty<ATurretPassiveAbilityDataModel>();
    
    public int CardLevel => _cardLevel;
    public int CardPlayCost => _cardPlayCost;
    public TurretCardPartsGroup SharedPartsGroup => _partsGroup;
    public BuildingSellingConfig MakeBuildingSellingConfig() => new BuildingSellingConfig(_buildingSelling.Config);
    public TurretCardPartsGroup MakePartsGroup() => new TurretCardPartsGroup(_partsGroup);

    public ATurretPassiveAbilityDataModel[] PassiveAbilityModels => _passiveAbilities;

    private const int MIN_CARD_LEVEL = 1;
    public const int MAX_CARD_LEVEL = 3;


    
    private void OnValidate()
    {
        LimitPassiveAbilitiesCount();
    }
    
    
    private void LimitPassiveAbilitiesCount()
    {
        if (_passiveAbilities.Length <= ATurretPassiveAbility.MAX_AMOUNT_FOR_TURRET)
        {
            return;
        }
        
        ATurretPassiveAbilityDataModel[] tempPassiveAbilities =
            new ATurretPassiveAbilityDataModel[ATurretPassiveAbility.MAX_AMOUNT_FOR_TURRET];
            
        Array.Copy(_passiveAbilities, 0, 
            tempPassiveAbilities, 0, ATurretPassiveAbility.MAX_AMOUNT_FOR_TURRET);
    }
}