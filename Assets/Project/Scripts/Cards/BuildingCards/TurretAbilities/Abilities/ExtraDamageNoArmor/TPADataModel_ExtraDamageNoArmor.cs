
using UnityEngine;

[CreateAssetMenu(fileName = "TurretAbility_CleanStrike", 
    menuName = SOAssetPaths.CARDS_ABILITIES + "CleanStrike")]
public class TPADataModel_ExtraDamageNoArmor : ATurretPassiveAbilityDataModel
{
    [Header("ABILITY CONFIG")] 
    [SerializeField] private AbilityDescriptionVariable _damagePercentBonus;
    
    public AbilityDescriptionVariable DamagePercentBonus => _damagePercentBonus;

    
    public override ATurretPassiveAbility MakePassiveAbility()
    {
        return new TurretPassiveAbility_ExtraDamageNoArmor(this);
    }
}