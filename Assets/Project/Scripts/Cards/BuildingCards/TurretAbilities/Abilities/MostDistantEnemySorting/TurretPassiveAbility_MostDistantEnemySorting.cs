using UnityEngine;

public class TurretPassiveAbility_MostDistantEnemySorting : ATurretPassiveAbility
{
    private TurretBuilding _turretOwner;
    
    public TurretPassiveAbility_MostDistantEnemySorting(ATurretPassiveAbilityDataModel originalModel) 
        : base(originalModel)
    {
    }

    public override void OnTurretCreated(TurretBuilding turretOwner)
    {
        _turretOwner = turretOwner;
    }

    protected override void OnTurretPlaced()
    {
        _turretOwner.SetNewTargetingController(new FurthestEnemyProjectileTargetingController(_turretOwner));
    }
}