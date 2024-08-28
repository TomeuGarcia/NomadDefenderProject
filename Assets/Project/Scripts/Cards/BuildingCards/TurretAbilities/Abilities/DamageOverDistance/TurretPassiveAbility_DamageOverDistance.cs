

using UnityEngine;

public class TurretPassiveAbility_DamageOverDistance : ATurretPassiveAbility
{
    private readonly TPADataModel_DamageOverDistance _abilityDataModel;
    private TurretBuilding _turretOwner;
    
    public TurretPassiveAbility_DamageOverDistance(TPADataModel_DamageOverDistance originalModel) 
        : base(originalModel)
    {
        _abilityDataModel = originalModel;
    }

    public override void OnTurretCreated(TurretBuilding turretOwner)
    {
        _turretOwner = turretOwner;
    }
    
    public override void OnBeforeDamagingEnemy(TurretDamageAttack damageAttack)
    {
        float distance = Vector3.Distance(damageAttack.Target.GetPosition(), _turretOwner.Position);

        int damage = (int)(damageAttack.Damage * _abilityDataModel.DamageMultiplierOverDistance.Evaluate(distance));
        damageAttack.UpdateDamage(damage);
    }
}