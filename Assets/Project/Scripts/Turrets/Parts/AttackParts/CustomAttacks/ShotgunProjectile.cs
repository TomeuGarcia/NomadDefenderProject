

using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShotgunProjectile : ATurretProjectileBehaviour, ShotgunBullet.IListener
{

    [SerializeField] private ShotgunBullet[] _bullets;
    private int _activeBulletsCounter;
    private List<Enemy> _preemptiveEnemyHits;

    private const float RADIUS_DISTANCE_MULTIPLIER = 2.5f;
    private const float HALF_SHOOT_ANGLE = 30f;

    private int NumberOfBullets => _bullets.Length;
    
    private void Awake()
    {
        foreach (ShotgunBullet bullet in _bullets)
        {
            bullet.Configure(this);
        }

        _preemptiveEnemyHits = new List<Enemy>(NumberOfBullets);
    }
    

    protected override void ProjectileShotInit(Enemy targetEnemy, TurretBuilding owner)
    {
        base.ProjectileShotInit(targetEnemy, owner);
        
        _damageAttack = CreateDamageAttack(targetEnemy);
        Vector3 directionToTarget = Vector3.ProjectOnPlane(targetEnemy.Position - owner.Position, Vector3.up).normalized;
        
        SharedInitEnd(targetEnemy, directionToTarget);
    }

    protected sealed override void ProjectileShotInit_PrecomputedAndQueued(TurretBuilding owner,
        TurretDamageAttack precomputedDamageAttack)
    {
        base.ProjectileShotInit_PrecomputedAndQueued(owner, precomputedDamageAttack);
        
        _damageAttack = precomputedDamageAttack;

        Enemy targetEnemy = precomputedDamageAttack.Target;
        Vector3 directionToTarget = Vector3.ProjectOnPlane(targetEnemy.Position - Position, Vector3.up).normalized;
        
        SharedInitEnd(targetEnemy, directionToTarget);
    }
    
    
    private void SharedInitEnd(Enemy targetEnemy, Vector3 directionToTarget)
    {
        _targetEnemy = targetEnemy;
        float bulletMoveDistance = TurretOwner.Stats.RadiusRange * RADIUS_DISTANCE_MULTIPLIER;
        float bulletMoveDuration = bulletMoveDistance / MovementSpeed;
        
        Quaternion rotationToTarget = Quaternion.FromToRotation(Vector3.forward, directionToTarget);

        InitPreemptiveHits();
        
        float angleStep = (HALF_SHOOT_ANGLE * 2) / NumberOfBullets;
        float accumulatedAngles = -HALF_SHOOT_ANGLE;
        for (int i = 0; i < NumberOfBullets; ++i)
        {
            //float randomAngle = accumulatedAngles + Random.Range(0, angleStep);
            //Quaternion bulletRotation = Quaternion.AngleAxis(randomAngle, Vector3.up) * rotationToTarget;
            Quaternion bulletRotation = Quaternion.AngleAxis(accumulatedAngles, Vector3.up) * rotationToTarget;
            
            ShotgunBullet bullet = _bullets[i];
            bullet.StartMoving(bulletRotation, bulletMoveDistance, bulletMoveDuration);

            ComputePreemptiveHit(bulletRotation, bulletMoveDistance);
            
            accumulatedAngles += angleStep;
        }

        OnShotInitialized();
    }


    protected override void OnShotInitialized()
    {
        base.OnShotInitialized();
        _activeBulletsCounter = NumberOfBullets;
    }
    
    
    

    private void InitPreemptiveHits()
    {
        _preemptiveEnemyHits.Clear();
    }
    private void ComputePreemptiveHit(Quaternion bulletRotation, float distance)
    {
        if (SpeedUpButton.UsingBuggyTimeScale)
        {
            Vector3 directionToGoalPosition = bulletRotation * Vector3.forward;
            Enemy preemptivelyHitEnemy = ComputeClosestIntersectingEnemy(Position, directionToGoalPosition, distance);
            if (preemptivelyHitEnemy != null)
            {
                _preemptiveEnemyHits.Add(preemptivelyHitEnemy);
            }
        }
    }
    private void ApplyPreemptiveHits()
    {
        foreach (Enemy preemptiveEnemyHit in _preemptiveEnemyHits)
        {
            if (CheckEnemy(preemptiveEnemyHit))
            {
                OnEnemyHit(preemptiveEnemyHit);
            }
        }
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
        return Mathf.RoundToInt(TurretOwner.Stats.Damage * _damageMultiplier);
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

        if (enemy.IsFakeEnemy)
        {
            AddEnemyToIgnore(enemy);
        }
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

        ApplyPreemptiveHits();
        Disappear();
    }
    
    
    
    protected override ITurretProjectileView MakeTurretProjectileView()
    {
        return new TurretMultipleProjectileView(_bullets);
    }
}