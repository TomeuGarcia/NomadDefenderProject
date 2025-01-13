
using UnityEngine;

[CreateAssetMenu(fileName = "TurretAbility_MultiCrash", 
    menuName = SOAssetPaths.CARDS_ABILITIES + "MultiCrash")]
public class TPADataModel_ExtraDamageMultipleHits : ATurretPassiveAbilityDataModel
{
    [Header("ABILITY CONFIG")] 
    [SerializeField, Min(0)] private float _startingDamageMultiplier = 0.5f;

    [SerializeField] private AbilityDescriptionVariable _damageMultiplierIncrementVariable;
    [SerializeField] private AbilityDescriptionVariable _maxDamageMultiplierVariable;
    
    public AbilityDescriptionVariable DamageMultiplierIncrementVariable => _damageMultiplierIncrementVariable;
    public AbilityDescriptionVariable MaxDamageMultiplierVariable => _maxDamageMultiplierVariable;
    
    public override ATurretPassiveAbility MakePassiveAbility()
    {
        return new TurretPassiveAbility_ExtraDamageMultipleHits(this);
    }


    public float DamageMultiplierByHitCount(int hitCount)
    {
        return Mathf.Min(
            _maxDamageMultiplierVariable.FloatValue,
            _startingDamageMultiplier + (_damageMultiplierIncrementVariable.FloatValue * hitCount)
        );
    }
    
}