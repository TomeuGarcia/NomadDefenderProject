using UnityEngine;

[CreateAssetMenu(fileName = "TurretAbility_ExtraDamagePerCurrency", 
    menuName = SOAssetPaths.CARDS_ABILITIES + "ExtraDamagePerCurrency")]
public class TPADataModel_ExtraDamagePerCurrency : ATurretPassiveAbilityDataModel
{
    [Header("ABILITY CONFIG")] 
    [SerializeField] private AbilityDescriptionVariable _currencyStepForBonus;
    [SerializeField] private AbilityDescriptionVariable _damagePercentBonus;
    
    public AbilityDescriptionVariable CurrencyStepForBonus => _currencyStepForBonus;
    public AbilityDescriptionVariable DamagePercentBonus => _damagePercentBonus;
    
    
    public override ATurretPassiveAbility MakePassiveAbility()
    {
        return new TurretPassiveAbility_ExtraDamagePerCurrency(this);
    }
}