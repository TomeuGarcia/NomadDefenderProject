

using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShotgunProjectile : ATurretProjectileBehaviour, ShotgunBullet.IListener
{

    [SerializeField] private ShotgunBullet[] _bullets;
    private int _activeBulletsCounter;

    private const float RADIUS_DISTANCE_MULTIPLIER = 2.5f;
    private const float HALF_SHOOT_ANGLE = 35f;
    private const float TOTAL_DAMAGE_MULTIPLIER = 1.5f;
    
    
    private void Awake()
    {
        foreach (ShotgunBullet bullet in _bullets)
        {
            bullet.Configure(this);
        }
    }

    protected override void ProjectileShotInit(Enemy targetEnemy, TurretBuilding owner)
    {
        base.ProjectileShotInit(targetEnemy, owner);
        
        _damageAttack = CreateDamageAttack(targetEnemy);
        SharedInitEnd(targetEnemy);
    }

    protected sealed override void ProjectileShotInit_PrecomputedAndQueued(TurretBuilding owner,
        TurretDamageAttack precomputedDamageAttack)
    {
        base.ProjectileShotInit_PrecomputedAndQueued(owner, precomputedDamageAttack);
        
        _damageAttack = precomputedDamageAttack;
        SharedInitEnd(precomputedDamageAttack.Target);
    }

    private void SharedInitEnd(Enemy targetEnemy)
    {
        _targetEnemy = targetEnemy;
        float bulletMoveDistance = TurretOwner.Stats.RadiusRange * RADIUS_DISTANCE_MULTIPLIER;
        float bulletMoveDuration = bulletMoveDistance / MovementSpeed;

        Vector3 directionToTarget =
            Vector3.ProjectOnPlane(targetEnemy.Position - TurretOwner.Position, Vector3.up).normalized;
        Quaternion rotationToTarget = Quaternion.FromToRotation(Vector3.forward, directionToTarget);
        

        float angleStep = (HALF_SHOOT_ANGLE * 2) / _bullets.Length;
        float accumulatedAngles = -HALF_SHOOT_ANGLE;
        for (int i = 0; i < _bullets.Length; ++i)
        {
            float randomAngle = accumulatedAngles + Random.Range(0, angleStep);
            Quaternion bulletRotation = Quaternion.AngleAxis(randomAngle, Vector3.up) * rotationToTarget;
            
            ShotgunBullet bullet = _bullets[i];
            bullet.StartMoving(bulletRotation, bulletMoveDistance, bulletMoveDuration);

            accumulatedAngles += angleStep;
        }

        OnShotInitialized();
    }


    protected override void OnShotInitialized()
    {
        base.OnShotInitialized();
        _activeBulletsCounter = _bullets.Length;
    }

    private void EnemyHit()
    {
        GameObject temp = ProjectileParticleFactory.GetInstance()
            .CreateParticlesGameObject(HitParticlesType, _targetEnemy.MeshTransform.position, Quaternion.identity);
        temp.transform.parent = gameObject.transform.parent;

        DamageTargetEnemy(_damageAttack);
    }


    protected override int ComputeDamage()
    {
        return (int)(TurretOwner.Stats.Damage * TOTAL_DAMAGE_MULTIPLIER / _bullets.Length);
    }
    
    public override bool QueuesDamageToEnemies()
    {
        return false;
    }


    public bool DoCheckEnemyOnTriggerEnter(Collider other, out Enemy enemy)
    {
        return CheckEnemyOnTriggerEnter(other, out enemy);
    }

    public void OnEnemyHit(Enemy enemy)
    {
        _targetEnemy = enemy;
        EnemyHit();
    }
    public void OnDisappearCompleted()
    {
        DecrementAndCheckActiveBullets();
    }

    private void DecrementAndCheckActiveBullets()
    {
        _activeBulletsCounter--;
        if (_activeBulletsCounter > 0)
        {
            return;
        }
        
        Disappear();
    }
    
    
    
    protected override ITurretProjectileView MakeTurretProjectileView()
    {
        return new TurretMultipleProjectileView(_bullets);
    }
}