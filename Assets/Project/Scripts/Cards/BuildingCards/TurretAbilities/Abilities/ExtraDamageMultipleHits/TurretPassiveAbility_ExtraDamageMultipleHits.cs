

using UnityEngine;

public class TurretPassiveAbility_ExtraDamageMultipleHits : ATurretPassiveAbility
{
    private readonly TPADataModel_ExtraDamageMultipleHits _abilityDataModel;
    private int _hitCount;

    private float _damageMultiplierStep;
    private float _maxMultiplierStep;
    
    public TurretPassiveAbility_ExtraDamageMultipleHits(TPADataModel_ExtraDamageMultipleHits originalModel) 
        : base(originalModel)
    {
        _abilityDataModel = originalModel;

        _damageMultiplierStep = _abilityDataModel.DamageMultiplierIncrementVariable.Value / 100f;
        _maxMultiplierStep = (_abilityDataModel.MaxDamageMultiplierVariable.Value + 100f) / 100f;
        
        UpdateDescriptionVariable(_abilityDataModel.DamageMultiplierIncrementVariable);
        UpdateDescriptionVariable(_abilityDataModel.MaxDamageMultiplierVariable);
    }


    protected override void DoOnBeforeShootingEnemyEnd(ATurretProjectileBehaviour projectile)
    {
        _hitCount = 0;
    }

    public override void OnBeforeDamagingEnemy(TurretDamageAttack damageAttack)
    {
        float damageMultiplier = Mathf.Min(_maxMultiplierStep, 1f + (_hitCount * _damageMultiplierStep));
        damageAttack.UpdateDamage((int)(damageAttack.Damage * damageMultiplier));
        
        ++_hitCount;
    }
    
}