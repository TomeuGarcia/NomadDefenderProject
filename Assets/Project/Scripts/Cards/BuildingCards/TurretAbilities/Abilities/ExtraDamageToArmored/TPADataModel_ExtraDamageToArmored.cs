using UnityEngine;

[CreateAssetMenu(fileName = "TurretAbility_MetalCruncher", 
    menuName = SOAssetPaths.CARDS_ABILITIES + "MetalCruncher")]
public class TPADataModel_ExtraDamageToArmored : ATurretPassiveAbilityDataModel
{
    [Header("ABILITY CONFIG")] 
    [SerializeField] private AbilityDescriptionVariable _damagePercentBonus;
    
    public AbilityDescriptionVariable DamagePercentBonus => _damagePercentBonus;
    
    
    public override ATurretPassiveAbility MakePassiveAbility()
    {
        return new TurretPassiveAbility_ExtraDamageToArmored(this);
    }
}