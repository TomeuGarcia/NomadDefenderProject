using UnityEngine;

public class TurretPassiveAbility_ExtraDamageSlowShooter : ATurretPassiveAbility
{
    private readonly TPADataModel_ExtraDamageSlowShooter _abilityDataModel;
    private TurretBuilding _turretOwner;

    private SlowFireRateTurretBuildingVisuals _slowFireRateVisuals;
    
    public TurretPassiveAbility_ExtraDamageSlowShooter(TPADataModel_ExtraDamageSlowShooter originalModel) 
        : base(originalModel)
    {
        _abilityDataModel = originalModel;
    }


    public override void OnTurretCreated(TurretBuilding turretOwner)
    {
        _turretOwner = turretOwner;
    }

    public override void OnTurretPlaced()
    {
        _slowFireRateVisuals = GameObject.Instantiate(_abilityDataModel.VisualsPrefab, _turretOwner.transform);
        _slowFireRateVisuals.TurretPlacedInit(_turretOwner, _abilityDataModel.MaxTimeBetweenShots);
    }


    public override void OnBeforeDamagingEnemy(TurretDamageAttack damageAttack)
    {
        float currentTime = Mathf.Min(_turretOwner.TimeSinceLastShot, _abilityDataModel.MaxTimeBetweenShots);
        
        float curveCoefficient = currentTime / _abilityDataModel.MaxTimeBetweenShots;
        float curveValue = _abilityDataModel.DamageMultiplierOverTimePer1.Evaluate(curveCoefficient);

        int damage = (int)(curveValue * damageAttack.Damage);
        
        damageAttack.UpdateDamage(damage);
        
    }
}