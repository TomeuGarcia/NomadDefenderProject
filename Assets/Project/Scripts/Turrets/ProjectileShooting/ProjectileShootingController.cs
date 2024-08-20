
using UnityEngine;

public class ProjectileShootingController
{
    private readonly TurretBuilding _turretOwner;
    private readonly TurretStatsSnapshot _stats;
    private readonly TurretBuilding.TestingSnapshot _testingSnapshot;
    private readonly TurretPartAttack_Prefab _projectilePrefab;
    private readonly TurretPartBody_Prefab _body;
    private float _shootTimer;
    public float TimeSinceLastShot { get; private set; }
    public Vector3 LastTargetedPosition { get; private set; }

    private readonly ProjectileTargetingController _targetingController;
    
    public ProjectileShootingController(TurretBuilding turretOwner,
        TurretStatsSnapshot stats, 
        TurretPartAttack_Prefab projectilePrefab, TurretPartBody_Prefab body, 
        TurretBuilding.TestingSnapshot testingSnapshot)
    {
        _turretOwner = turretOwner;
        _stats = stats;
        _testingSnapshot = testingSnapshot;
        _projectilePrefab = projectilePrefab;
        _body = body;

        _targetingController = new ProjectileTargetingController(turretOwner);
        
        TimeSinceLastShot = 0;
        _shootTimer = _stats.ShotsPerSecondInverted;
    }
    
    
    public void UpdateShoot(out bool targetEnemyExists)
    {
        _targetingController.ComputeNextTargetedEnemy();
        targetEnemyExists = _targetingController.TargetEnemyExists();
        if (targetEnemyExists)
        {
            LastTargetedPosition = _targetingController.TargetedEnemy.Position;
        }
        
        float deltaTime = GameTime.DeltaTime;
        _testingSnapshot.UpdateTotalTimePlaced(deltaTime);
        TimeSinceLastShot += deltaTime;

        if (_shootTimer < _stats.ShotsPerSecondInverted)
        {            
            _shootTimer += deltaTime;
            _testingSnapshot.UpdateTimeTargetingEnemies(deltaTime);
            return;
        }

        if (!targetEnemyExists)
        {
            return;
        }

        Shoot(_targetingController.TargetedEnemy);
        ResetShootState();
    }
    
    
    private void Shoot(Enemy targetedEnemy)
    {
        Vector3 shootPoint = _body.GetNextShootingPoint();
        TurretPartAttack_Prefab currentAttack = ProjectileAttacksFactory.GetInstance()
            .GetAttackGameObject(_projectilePrefab.GetAttackType, shootPoint, Quaternion.identity)
            .GetComponent<TurretPartAttack_Prefab>();

        currentAttack.transform.parent = _turretOwner.transform;
        currentAttack.gameObject.SetActive(true);
        currentAttack.ProjectileShotInit(targetedEnemy, _turretOwner);


        // Spawn particle
        GameObject particles = ProjectileParticleFactory.GetInstance()
            .GetAttackParticlesGameObject(currentAttack.GetAttackType, shootPoint, _body.transform.rotation);
        particles.SetActive(true);
        particles.transform.parent = _turretOwner.transform;


        // Audio
        GameAudioManager.GetInstance().PlayProjectileShot(_body.BodyType);
        
        _turretOwner.OnEnemyShot(targetedEnemy);
    }


    private void ResetShootState()
    {
        _shootTimer = 0f;
        TimeSinceLastShot = 0f;
    }
}