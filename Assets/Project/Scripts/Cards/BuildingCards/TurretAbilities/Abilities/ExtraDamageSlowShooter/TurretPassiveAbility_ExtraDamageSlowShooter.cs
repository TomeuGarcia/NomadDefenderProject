using UnityEngine;

public class TurretPassiveAbility_ExtraDamageSlowShooter : ATurretPassiveAbility,
    ProjectileViewAddOn_ChargedBlast.IConfigurationSource
{
    private readonly TPADataModel_ExtraDamageSlowShooter _abilityDataModel;
    private TurretBuilding _turretOwner;

    private SlowFireRateTurretBuildingVisuals _slowFireRateVisuals;

    private float _timeSinceLastShot;
    
    public TurretPassiveAbility_ExtraDamageSlowShooter(TPADataModel_ExtraDamageSlowShooter originalModel) 
        : base(originalModel)
    {
        _abilityDataModel = originalModel;
    }


    public override void OnTurretCreated(TurretBuilding turretOwner)
    {
        _turretOwner = turretOwner;
    }

    protected override void OnTurretPlaced()
    {
        _slowFireRateVisuals = GameObject.Instantiate(_abilityDataModel.VisualsPrefab, _turretOwner.transform);
        _slowFireRateVisuals.TurretPlacedInit(_turretOwner, _abilityDataModel.MaxTimeBetweenShots);
    }

    protected override void DoOnBeforeShootingEnemyStart()
    {
        _timeSinceLastShot = _turretOwner.TimeSinceLastShot;
        ProjectileViewAddOn_ChargedBlast.ConfigurationSource = this;
    }
    
    public override void OnBeforeDamagingEnemy(TurretDamageAttack damageAttack)
    {
        float currentTime = Mathf.Min(_timeSinceLastShot, _abilityDataModel.MaxTimeBetweenShots);
        float curveValue = _abilityDataModel.DamageMultiplierOverTimePer1.Evaluate(currentTime);

        int damage = (int)Mathf.Max((curveValue * damageAttack.Damage), 1);

        damageAttack.UpdateDamage(damage);
    }

    bool ProjectileViewAddOn_ChargedBlast.IConfigurationSource.IsFullyCharged()
    {
        return _timeSinceLastShot > _abilityDataModel.MaxTimeBetweenShots;
    }
}