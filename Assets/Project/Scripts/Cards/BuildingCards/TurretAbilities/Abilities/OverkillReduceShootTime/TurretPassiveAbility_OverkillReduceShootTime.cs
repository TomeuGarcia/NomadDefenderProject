
using UnityEngine;

public class TurretPassiveAbility_OverkillReduceShootTime : ATurretPassiveAbility
{
    private readonly TPADataModel_OverkillReduceShootTime _abilityDataModel;
    private TurretBuilding _turretOwner;

    public TurretPassiveAbility_OverkillReduceShootTime(TPADataModel_OverkillReduceShootTime originalModel) 
        : base(originalModel)
    {
        _abilityDataModel = originalModel;
    }


    public override void OnTurretCreated(TurretBuilding turretOwner)
    {
        _turretOwner = turretOwner;
    }

    public override void OnAfterDamagingEnemy(TurretDamageAttackResult damageAttackResult)
    {
        if (damageAttackResult.HitKilled &&
            damageAttackResult.DamageAttackSource.Damage >= damageAttackResult.Target.HealthSystem.GetMaxHealth())
        {
            TriggerReduceShootTime();
        }
    }

    private void TriggerReduceShootTime()
    {
        float shootTimeGain = _turretOwner.Stats.ShotsPerSecondInverted * 0.5f;
        _turretOwner.ShootingController.UpdateShoot(shootTimeGain);
        
        ServiceLocator.GetInstance().ParticleFactory.Create(ParticleTypes.OverkillShootTime,
            _turretOwner.BaseHolder.position, Quaternion.identity);
    }
}