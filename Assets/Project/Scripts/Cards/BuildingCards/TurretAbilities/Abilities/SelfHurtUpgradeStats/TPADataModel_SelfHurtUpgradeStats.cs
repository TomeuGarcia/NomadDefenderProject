
using UnityEngine;

[CreateAssetMenu(fileName = "TurretAbility_BreakingBoost", 
    menuName = SOAssetPaths.CARDS_ABILITIES + "BreakingBoost")]
public class TPADataModel_SelfHurtUpgradeStats : ATurretPassiveAbilityDataModel
{
    [Header("ABILITY CONFIG")] 
    [SerializeField] private AbilityDescriptionVariable _nodeDamageAmount;
    [SerializeField] private AbilityDescriptionVariable _bonusDamagePercent;
    
    public AbilityDescriptionVariable NodeDamageAmount => _nodeDamageAmount;
    public AbilityDescriptionVariable BonusDamagePercent => _bonusDamagePercent;
    

    
    public override ATurretPassiveAbility MakePassiveAbility()
    {
        return new TurretPassiveAbility_SelfHurtUpgradeStats(this);
    }
}