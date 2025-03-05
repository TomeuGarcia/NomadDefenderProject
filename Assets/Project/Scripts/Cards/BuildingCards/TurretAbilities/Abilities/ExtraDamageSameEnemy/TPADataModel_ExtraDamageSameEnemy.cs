using UnityEngine;

[CreateAssetMenu(fileName = "TurretAbility_ExtraDamageSameEnemy", 
    menuName = SOAssetPaths.CARDS_ABILITIES + "ExtraDamageSameEnemy")]
public class TPADataModel_ExtraDamageSameEnemy : ATurretPassiveAbilityDataModel
{
    [Header("ABILITY CONFIG")] 
    [SerializeField] private AbilityDescriptionVariable _damagePercentBonus;
    
    public AbilityDescriptionVariable DamagePercentBonus => _damagePercentBonus;
    
    public override ATurretPassiveAbility MakePassiveAbility()
    {
        return new TurretPassiveAbility_ExtraDamageSameEnemy(this);
    }
}