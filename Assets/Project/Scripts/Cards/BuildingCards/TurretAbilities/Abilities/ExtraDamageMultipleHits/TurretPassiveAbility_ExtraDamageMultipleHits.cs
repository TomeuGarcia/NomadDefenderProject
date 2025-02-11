

using UnityEngine;

public class TurretPassiveAbility_ExtraDamageMultipleHits : ATurretPassiveAbility
{
    private readonly TPADataModel_ExtraDamageMultipleHits _abilityDataModel;
    private int _hitCount;
    
    public TurretPassiveAbility_ExtraDamageMultipleHits(TPADataModel_ExtraDamageMultipleHits originalModel) 
        : base(originalModel)
    {
        _abilityDataModel = originalModel;
        
        int damageMultiplierIncrementPercent = Mathf.RoundToInt(_abilityDataModel.DamageMultiplierIncrementVariable.FloatValue * 100);
        UpdateDescriptionVariable(_abilityDataModel.DamageMultiplierIncrementVariable.Name, damageMultiplierIncrementPercent);
        
        int maxDamageMultiplierPercent = Mathf.RoundToInt((_abilityDataModel.MaxDamageMultiplierVariable.FloatValue - 1f) * 100);
        UpdateDescriptionVariable(_abilityDataModel.MaxDamageMultiplierVariable.Name, maxDamageMultiplierPercent);
    }


    protected override void DoOnBeforeShootingEnemyEnd(ATurretProjectileBehaviour projectile)
    {
        _hitCount = 0;
    }

    public override void OnBeforeDamagingEnemy(TurretDamageAttack damageAttack)
    {
        float damageMultiplier = _abilityDataModel.DamageMultiplierByHitCount(_hitCount);
        
        damageAttack.UpdateDamage((int)(damageAttack.Damage * damageMultiplier));
        
        ++_hitCount;
    }
    
}