

public class TurretPassiveAbility_ExtraDamageNoArmor : ATurretPassiveAbility
{
    private readonly float _damageMultiplier;

    
    public TurretPassiveAbility_ExtraDamageNoArmor(TPADataModel_ExtraDamageNoArmor originalModel) 
        : base(originalModel)
    {
        _damageMultiplier = 1f + (originalModel.DamagePercentBonus.Value / 100f);
        
        ApplyDescriptionCorrection(originalModel.DamagePercentBonus);
    }


    public override void OnBeforeDamagingEnemy(TurretDamageAttack damageAttack)
    {
        if (!damageAttack.Target.HealthSystem.HasArmor())
        {
            damageAttack.UpdateDamage((int)(damageAttack.Damage * _damageMultiplier));
        }
    }
}