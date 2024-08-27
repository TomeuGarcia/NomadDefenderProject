

using UnityEngine;

public class TurretPassiveAbility_ExtraDamageNoArmor : ATurretPassiveAbility
{
    private readonly TPADataModel_ExtraDamageNoArmor _originalModel;
    private readonly float _damageMultiplier;

    
    public TurretPassiveAbility_ExtraDamageNoArmor(TPADataModel_ExtraDamageNoArmor originalModel) 
        : base(originalModel)
    {
        _originalModel = originalModel;
        _damageMultiplier = 1f + (originalModel.DamagePercentBonus.Value / 100f);
        
        ApplyDescriptionCorrection(originalModel.DamagePercentBonus);
    }


    public override void OnBeforeDamagingEnemy(TurretDamageAttack damageAttack)
    {
        if (!damageAttack.Target.HealthSystem.HasArmor())
        {
            damageAttack.UpdateDamage((int)(damageAttack.Damage * _damageMultiplier));
            
            // Option A
            _projectileParticleFactory.CreateParticlesGameObject(ProjectileParticleType.CleanStrikeHit, 
                damageAttack.Target.Position, damageAttack.ProjectileSource.Rotation);
        }
    }

    public override void OnBeforeShootingEnemy(ATurretProjectileBehaviour projectile)
    {
        // Option B
        //AddViewAddOnToProjectile(_originalModel.ViewAddOn, projectile);
    }
    
}