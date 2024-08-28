

public class TurretPassiveAbility_ExtraDamageToArmored : ATurretPassiveAbility
{
    private readonly float _damageMultiplier;

    
    public TurretPassiveAbility_ExtraDamageToArmored(TPADataModel_ExtraDamageToArmored originalModel) 
        : base(originalModel)
    {
        _damageMultiplier = 1f + (originalModel.DamagePercentBonus.Value / 100f);
        
        ApplyDescriptionCorrection(originalModel.DamagePercentBonus);
    }

    

    public override void OnBeforeDamagingEnemy(TurretDamageAttack damageAttack)
    {
        if (damageAttack.Target.HealthSystem.HasArmor())
        {
            damageAttack.UpdateDamage((int)(damageAttack.Damage * _damageMultiplier));
            _projectileParticleFactory.CreateParticlesGameObject(ProjectileParticleType.MetalCruncher_Hit,
                damageAttack.Target.Position, damageAttack.ProjectileSource.Rotation);
        }
    }
}