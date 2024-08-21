using System;
using UnityEngine;
using UnityEngine.Serialization;


[CreateAssetMenu(fileName = "NewTurretCardDataModel", 
    menuName = SOAssetPaths.CARDS_BUILDINGS + "TurretCardDataModel")]
public class TurretCardDataModel : ScriptableObject
{
    [Header("LEVEL")]
    [SerializeField, Range(MIN_CARD_LEVEL, MAX_CARD_LEVEL)] private int _cardLevel = 1;
    
    [Header("PLAY COST")]
    [SerializeField, Min(0)] private int _cardPlayCost;

    [Header("PARTS")] 
    [SerializeField] private TurretCardPartsGroup _partsGroup;
    [SerializeField] private TurretPassiveAbilityDataModel[] _passiveAbilities = Array.Empty<TurretPassiveAbilityDataModel>();
    
    
    public int CardLevel => _cardLevel;
    public int CardPlayCost => _cardPlayCost;
    public TurretCardPartsGroup SharedPartsGroup => _partsGroup;
    public TurretCardPartsGroup MakePartsGroup() => new TurretCardPartsGroup(_partsGroup);

    public TurretPassiveAbilityDataModel[] PassiveAbilityModels => _passiveAbilities;

    private const int MIN_CARD_LEVEL = 1;
    public const int MAX_CARD_LEVEL = 3;
}