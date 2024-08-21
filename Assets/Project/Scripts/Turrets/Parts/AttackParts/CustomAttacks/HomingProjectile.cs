using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class HomingProjectile : TurretPartAttack_Prefab
{
    [SerializeField] protected Lerp lerp;

    protected override void DoUpdate()
    {
    }


    public void NEW_ProjectileShotInit(Enemy targetEnemy, TurretBuilding turretOwner)
    {
        base.ProjectileShotInit(targetEnemy, turretOwner);

        this.targetEnemy = targetEnemy;

        _damageAttack = new TurretDamageAttack(this, targetEnemy, turretOwner.Stats.Damage);
        
        
        
        
        targetEnemy.QueueDamage(_damageAttack);
        
        
    }
    

    public sealed override void ProjectileShotInit(Enemy targetEnemy, TurretBuilding owner)
    {
        base.ProjectileShotInit(targetEnemy, owner);
        this.targetEnemy = targetEnemy;

        // TODO: NOTIFY ABILITIES
        
        /*
        this.damage = targetEnemy.ComputeDamageWithPassive(this, owner.Stats.Damage);
        targetEnemy.QueueDamage(damage);
        */

        _damageAttack = new TurretDamageAttack(this, targetEnemy, ComputeDamage());
        targetEnemy.QueueDamage(_damageAttack);

        lerp.LerpPosition(targetEnemy.MeshTransform, bulletSpeed);
        StartCoroutine(WaitForLerpFinish());

        OnShotInitialized();
    }

    public sealed override void ProjectileShotInit_PrecomputedAndQueued(TurretBuilding owner, TurretDamageAttack precomputedDamageAttack)
    {
        turretOwner = owner;

        this.targetEnemy = precomputedDamageAttack.Target;
        _damageAttack = precomputedDamageAttack;

        lerp.LerpPosition(targetEnemy.MeshTransform, bulletSpeed);
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
            .GetAttackParticlesGameObject(attackType, targetEnemy.MeshTransform.position, Quaternion.identity);
        temp.gameObject.SetActive(true);
        temp.transform.parent = gameObject.transform.parent;

        DamageTargetEnemy(_damageAttack);

        Disappear();
    }


    protected virtual int ComputeDamage()
    {
        return turretOwner.Stats.Damage;
    }

    
}
