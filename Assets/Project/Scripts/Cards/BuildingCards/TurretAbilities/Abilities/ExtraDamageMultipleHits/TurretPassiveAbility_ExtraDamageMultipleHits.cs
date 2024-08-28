

public class TurretPassiveAbility_ExtraDamageMultipleHits : ATurretPassiveAbility
{
    private readonly TPADataModel_ExtraDamageMultipleHits _abilityDataModel;
    private int _hitCount;
    
    public TurretPassiveAbility_ExtraDamageMultipleHits(TPADataModel_ExtraDamageMultipleHits originalModel) 
        : base(originalModel)
    {
        _abilityDataModel = originalModel;
    }


    protected override void DoOnBeforeShootingEnemy(ATurretProjectileBehaviour projectile)
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