
using UnityEngine;

[CreateAssetMenu(fileName = "TurretAbility_StackedPower", 
    menuName = SOAssetPaths.CARDS_ABILITIES + "StackedPower")]
public class TPADataModel_ExtraDamagePerCardInHand : ATurretPassiveAbilityDataModel
{
    [Header("ABILITY CONFIG")] 
    [SerializeField] private AbilityDescriptionVariable _damagePercentBonusPerCard;
    
    public AbilityDescriptionVariable DamagePercentBonusPerCard => _damagePercentBonusPerCard;

    
    public override ATurretPassiveAbility MakePassiveAbility()
    {
        return new TurretPassiveAbility_ExtraDamagePerCardInHand(this);
    }
}