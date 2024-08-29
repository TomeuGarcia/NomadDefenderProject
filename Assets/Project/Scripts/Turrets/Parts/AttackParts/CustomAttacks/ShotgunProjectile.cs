

using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShotgunProjectile : ATurretProjectileBehaviour, ShotgunBullet.IListener
{

    [SerializeField] private ShotgunBullet[] _bullets;
    private int _activeBulletsCounter;
    

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
        targetEnemy.QueueDamage(_damageAttack);
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
        float bulletMoveDistance = TurretOwner.Stats.RadiusRange;
        float bulletMoveDuration = bulletMoveDistance / MovementSpeed;
        float HalfShootAngle = 50f;

        Vector3 directionToTarget =
            Vector3.ProjectOnPlane(targetEnemy.Position - TurretOwner.Position, Vector3.up).normalized;
        Quaternion rotationToTarget = Quaternion.FromToRotation(Vector3.forward, directionToTarget);
        
        _activeBulletsCounter = _bullets.Length;
        for (int i = 0; i < _bullets.Length; ++i)
        {
            bool goRight = i % 2 == 0;
            float randomAngle = Random.Range(0f, (goRight ? HalfShootAngle : -HalfShootAngle));
            Quaternion bulletRotation = Quaternion.AngleAxis(randomAngle, Vector3.up) * rotationToTarget;
            
            ShotgunBullet bullet = _bullets[i];
            bullet.StartMoving(bulletRotation, bulletMoveDistance, bulletMoveDuration);
        }

        OnShotInitialized();
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
        return (int)(TurretOwner.Stats.Damage * 1.5f / _bullets.Length);
    }
    
    public override bool QueuesDamageToEnemies()
    {
        return false;
    }
    
    

    public void OnEnemyHit(Enemy enemy)
    {
        _targetEnemy = enemy;
        EnemyHit();

        DecrementAndCheckActiveBullets();
    }

    public void OnEndReached()
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