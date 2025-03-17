
using UnityEngine;

public class ProjectileShootingController_EnemyRequired : AProjectileShootingController
{
    private readonly TurretStatsSnapshot _stats;
    private float _shootTimer;
    private IProjectileTargetingController _targetingController;

    private bool _isTargetLocked = false;
    
    public ProjectileShootingController_EnemyRequired(CreateData createData)
        : base(createData)
    {
        _stats = createData.Stats;
        SetTargetingController(new DefaultProjectileTargetingController(createData.TurretOwner));
        
        TimeSinceLastShot = 0;
        _shootTimer = _stats.ShotsPerSecondInverted;
    }

    public void SetTargetingController(IProjectileTargetingController newTargetingController)
    {
        _targetingController = newTargetingController;
    }

    public override void UpdateShoot(float deltaTime)
    {
        if (!_isTargetLocked)
        {
            _targetingController.ClearTargetedEnemy();
        }
        
        _targetingController.ComputeNextTargetedEnemy();
        bool targetEnemyExists = _targetingController.TargetEnemyExists();
        
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

        DoShoot();
        ResetShootState();
    }

    public override void OnEnemyKilled(Enemy killedEnemy)
    {
        if (killedEnemy != _targetingController.TargetedEnemy)
        {
            return;
        }

        _isTargetLocked = false; // Not target locked until the first (or any) shot
    }

    public override void DoShoot()
    {
        _isTargetLocked = true; // Become target locked whenever a shot happens 
        Shoot(_targetingController.TargetedEnemy);
    }


    private void ResetShootState()
    {
        _shootTimer = 0f;
        TimeSinceLastShot = 0f;
    }
}