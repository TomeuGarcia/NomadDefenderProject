
using UnityEngine;

[CreateAssetMenu(fileName = "TurretAbility_MultiCrash", 
    menuName = SOAssetPaths.CARDS_ABILITIES + "MultiCrash")]
public class TPADataModel_ExtraDamageMultipleHits : ATurretPassiveAbilityDataModel
{
    [Header("ABILITY CONFIG")] 
    [SerializeField, Min(0)] private float _startingDamageMultiplier = 0.5f;
    [SerializeField, Min(1)] private float _maxDamageMultiplier = 2.0f;
    [SerializeField, Min(0)] private float _damageMultiplierIncrementPerHit = 0.25f;
    
    public override ATurretPassiveAbility MakePassiveAbility()
    {
        return new TurretPassiveAbility_ExtraDamageMultipleHits(this);
    }


    public float DamageMultiplierByHitCount(int hitCount)
    {
        return Mathf.Max(_maxDamageMultiplier, _startingDamageMultiplier + (_damageMultiplierIncrementPerHit * hitCount));
    }
    
}