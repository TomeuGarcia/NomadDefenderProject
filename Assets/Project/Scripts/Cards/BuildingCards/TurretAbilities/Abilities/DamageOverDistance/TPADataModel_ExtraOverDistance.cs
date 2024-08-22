using UnityEngine;

[CreateAssetMenu(fileName = "TurretAbility_Generic-DamageOverDistance", 
    menuName = SOAssetPaths.CARDS_ABILITIES + "Generic_DamageOverDistance")]
public class TPADataModel_ExtraOverDistance : ATurretPassiveAbilityDataModel
{
    [Header("ABILITY CONFIG")] 
    [SerializeField] private AnimationCurve _damageMultiplierOverDistance;
    
    public AnimationCurve DamageMultiplierOverDistance => _damageMultiplierOverDistance;
    
    
    public override ATurretPassiveAbility MakePassiveAbility()
    {
        return new TurretPassiveAbility_DamageOverDistance(this);
    }
}