
using UnityEngine;

[CreateAssetMenu(fileName = "TurretAbility_CleanStrike", 
    menuName = SOAssetPaths.CARDS_ABILITIES + "CleanStrike")]
public class TPADataModel_ExtraDamageNoArmor : ATurretPassiveAbilityDataModel
{
    [Header("ABILITY CONFIG")] 
    [SerializeField] private AbilityDescriptionVariable _damagePercentBonus;

    [Header("VIEW CONFIG")] 
    [SerializeField] private ProjectileViewAddOnConfig _viewAddOn;

    public AbilityDescriptionVariable DamagePercentBonus => _damagePercentBonus;
    public ProjectileViewAddOnConfig ViewAddOn => _viewAddOn;

    
    public override ATurretPassiveAbility MakePassiveAbility()
    {
        return new TurretPassiveAbility_ExtraDamageNoArmor(this);
    }
}