
using UnityEngine;

public class ProjectileShootingController_EnemyRequired : AProjectileShootingController
{
    private readonly TurretStatsSnapshot _stats;
    private float _shootTimer;
    private readonly ProjectileTargetingController _targetingController;
    
    public ProjectileShootingController_EnemyRequired(CreateData createData)
        : base(createData)
    {
        _stats = createData.Stats;
        _targetingController = new ProjectileTargetingController(createData.TurretOwner);
        
        TimeSinceLastShot = 0;
        _shootTimer = _stats.ShotsPerSecondInverted;
    }


    public override void UpdateShoot()
    {
        _targetingController.ComputeNextTargetedEnemy();
        bool targetEnemyExists = _targetingController.TargetEnemyExists();
        
        float deltaTime = GameTime.DeltaTime;
        TimeSinceLastShot += deltaTime;
        
        if (targetEnemyExists)
        {
            LastTargetedPosition = _targetingController.TargetedEnemy.Position;
        }

        if (_shootTimer < _stats.ShotsPerSecondInverted)
        {            
            _shootTimer += deltaTime;
            return;
        }

        if (!targetEnemyExists)
        {
            return;
        }

        Shoot(_targetingController.TargetedEnemy);
        ResetShootState();
    }
    
    



    private void ResetShootState()
    {
        _shootTimer = 0f;
        TimeSinceLastShot = 0f;
    }
}