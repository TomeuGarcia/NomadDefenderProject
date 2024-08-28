using System.Collections;
using UnityEngine;

public class HomingProjectile : ATurretProjectileBehaviour
{
    [SerializeField] protected Lerp lerp;
    
    
    protected sealed override void ProjectileShotInit(Enemy targetEnemy, TurretBuilding owner)
    {
        base.ProjectileShotInit(targetEnemy, owner);
        _targetEnemy = targetEnemy;
        
        _damageAttack = CreateDamageAttack(_targetEnemy);

        targetEnemy.QueueDamage(_damageAttack);

        lerp.LerpPosition(targetEnemy.MeshTransform, MovementSpeed);
        StartCoroutine(WaitForLerpFinish());

        OnShotInitialized();
    }

    protected sealed override void ProjectileShotInit_PrecomputedAndQueued(TurretBuilding owner, TurretDamageAttack precomputedDamageAttack)
    {
        base.ProjectileShotInit_PrecomputedAndQueued(owner, precomputedDamageAttack);

        _targetEnemy = precomputedDamageAttack.Target;
        _damageAttack = precomputedDamageAttack;

        lerp.LerpPosition(_targetEnemy.MeshTransform, MovementSpeed);
        StartCoroutine(WaitForLerpFinish());
        
        OnShotInitialized();
    }


    protected IEnumerator WaitForLerpFinish()
    {
        yield return new WaitUntil(() => lerp.finishedPositionLerp);
        OnEnemyReached();
    }

    protected virtual void OnEnemyReached()
    {
        EnemyHit();
    }
    
    private void EnemyHit()
    {
        GameObject temp = ProjectileParticleFactory.GetInstance()
            .CreateParticlesGameObject(HitParticlesType, _targetEnemy.MeshTransform.position, Quaternion.identity);
        temp.transform.parent = gameObject.transform.parent;

        DamageTargetEnemy(_damageAttack);

        Disappear();
    }


    protected override int ComputeDamage()
    {
        return TurretOwner.Stats.Damage;
    }

    
}
