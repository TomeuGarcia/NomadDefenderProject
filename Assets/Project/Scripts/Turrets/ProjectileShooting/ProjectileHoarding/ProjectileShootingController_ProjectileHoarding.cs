using System.Collections.Generic;
using UnityEngine;

public class ProjectileShootingController_ProjectileHoarding : AProjectileShootingController,
    ATurretProjectileBehaviour.IDisappearListener
{
    private readonly TurretStatsSnapshot _stats;
    private float _shootTimer;
    private readonly int _maxHoardedProjectilesCount;
    private readonly List<ATurretProjectileBehaviour> _hoardedProjectiles;
    

    public ProjectileShootingController_ProjectileHoarding(CreateData createData, int maxHoardedProjectilesCount)
        : base(createData)
    {
        _stats = createData.Stats;
        _shootTimer = 0f;
        _maxHoardedProjectilesCount = maxHoardedProjectilesCount;
        _hoardedProjectiles = new List<ATurretProjectileBehaviour>(_maxHoardedProjectilesCount);
    }
    
    public override void UpdateShoot()
    {
        float deltaTime = GameTime.DeltaTime;
        TimeSinceLastShot += deltaTime;
        
        LastTargetedPosition = _turretOwner.Position + _turretOwner.BodyPartTransform.forward;

        if (_shootTimer < _stats.ShotsPerSecondInverted)
        {            
            _shootTimer += deltaTime;
            return;
        }

        if (_hoardedProjectiles.Count >= _maxHoardedProjectilesCount)
        {
            return;
        }

        ATurretProjectileBehaviour projectileBehaviour = Shoot(EnemyFactory.GetInstance().GetNullEnemy());
        AddToHoardedProjectiles(projectileBehaviour);

        ResetShootState();
    }
    
    private void ResetShootState()
    {
        _shootTimer = 0f;
        TimeSinceLastShot = 0f;
    }

    

    private void AddToHoardedProjectiles(ATurretProjectileBehaviour projectileBehaviour)
    {
        _hoardedProjectiles.Add(projectileBehaviour);
        projectileBehaviour.DisappearListener = this;
    }
    private void RemoveHoardedProjectiles(ATurretProjectileBehaviour projectileBehaviour)
    {
        _hoardedProjectiles.Remove(projectileBehaviour);
        projectileBehaviour.DisappearListener = null;
    }
    
    public void OnProjectileDisappeared(ATurretProjectileBehaviour projectileBehaviour)
    {
        RemoveHoardedProjectiles(projectileBehaviour);
    }

    public override void ClearAllProjectiles()
    {
        foreach (ATurretProjectileBehaviour projectileBehaviour in _hoardedProjectiles)
        {
            projectileBehaviour.DisappearListener = null;
            projectileBehaviour.Disable();
        }
        _hoardedProjectiles.Clear();
    }
}