
using UnityEngine;

[CreateAssetMenu(fileName = "TurretAbility_BreakingBoost", 
    menuName = SOAssetPaths.CARDS_ABILITIES + "BreakingBoost")]
public class TPADataModel_SelfHurtUpgradeStats : ATurretPassiveAbilityDataModel
{
    [Header("ABILITY CONFIG")] 
    [SerializeField] private AbilityDescriptionVariable _damageAmount;
    
    public AbilityDescriptionVariable DamageAmount => _damageAmount;
    

    
    public override ATurretPassiveAbility MakePassiveAbility()
    {
        return new TurretPassiveAbility_SelfHurtUpgradeStats(this);
    }
}