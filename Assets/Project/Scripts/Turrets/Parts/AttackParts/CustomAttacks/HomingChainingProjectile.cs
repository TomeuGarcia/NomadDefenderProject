using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingChainingProjectile : HomingProjectile
{
    [Header("STATS")]
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField, Range(0f, 1f)] private float damageMultiplier = 0.5f;
    [SerializeField, Min(1)] private int maxChainedTargets = 1;
    [SerializeField, Min(0f)] private float chainRadius;

    private int _currentChainedTarget;
    private Enemy[] _chainTargetedEnemies;
    private TurretDamageAttack[] _chainTargetedDamage;

/*
    protected override void DoUpdate()
    {
    }

    public override void ProjectileShotInit(Enemy targetEnemy, TurretBuilding owner)
    {
        turretOwner = owner;

        this.targetEnemy = targetEnemy;


        this.currentChainedTarget = 0;

        this.damage = owner.Stats.Damage;
        this.damage = (int)((float)this.damage * damageMultiplier);
        this.damage = targetEnemy.ComputeDamageWithPassive(this, this.damage);

        targetEnemy.QueueDamage(this.damage);
        

        lerp.LerpPosition(targetEnemy.MeshTransform, bulletSpeed);
        StartCoroutine(WaitForLerpFinish(false));
    }

    public override void ProjectileShotInit_PrecomputedAndQueued(Enemy targetEnemy, TurretBuilding owner, int precomputedDamage)
    {
        turretOwner = owner;

        this.targetEnemy = targetEnemy;


        this.currentChainedTarget = 0;

        this.damage = precomputedDamage;


        lerp.LerpPosition(targetEnemy.MeshTransform, bulletSpeed);
        StartCoroutine(WaitForLerpFinish(false));
    }


    IEnumerator WaitForLerpFinish(bool isChaining)
    {
        yield return new WaitUntil(() => lerp.finishedPositionLerp == true);
        EnemyHit(isChaining);
    }
    */

    private bool _isChaining;

    protected override void OnShotInitialized()
    {
        base.OnShotInitialized();
        _isChaining = false;
        _currentChainedTarget = 0;
    }

    protected override void OnEnemyReached()
    {
        GameObject hitParticles = ProjectileParticleFactory.GetInstance()
            .GetAttackParticlesGameObject(ProjectileType, targetEnemy.MeshTransform.position, Quaternion.identity);
        hitParticles.gameObject.SetActive(true);
        hitParticles.transform.parent = gameObject.transform.parent;
        
        //targetEnemy.GetStunned(stunDuration); // Already on HighVoltage ability

        
        if (_isChaining)
        {
            DoChainedEnemyHit();
        }
        else
        {
            DoTargetedEnemyHit();
        }


        if (_currentChainedTarget < _chainTargetedEnemies.Length)
        {
            targetEnemy = _chainTargetedEnemies[_currentChainedTarget];
            lerp.LerpPosition(targetEnemy.MeshTransform, bulletSpeed / 2.0f);
            StartCoroutine(WaitForLerpFinish());            
        }
        else
        {
            Disappear();
        }
    }

    private void DoTargetedEnemyHit()
    {
        DamageTargetEnemy(_damageAttack);


        _chainTargetedEnemies = GetNearestEnemiesToTargetedEnemy(targetEnemy, maxChainedTargets, chainRadius, enemyLayerMask);
        _chainTargetedDamage = new TurretDamageAttack[_chainTargetedEnemies.Length];
        for (int i = 0; i < _chainTargetedEnemies.Length; i++)
        {
            Enemy chainedEnemy = _chainTargetedEnemies[i];
            
            _chainTargetedDamage[i] =
                new TurretDamageAttack(_damageAttack.ProjectileSource, chainedEnemy, _damageAttack.Damage);
            chainedEnemy.QueueDamage(_chainTargetedDamage[i]);
        }
        
        _isChaining = true;
    }
    
    private void DoChainedEnemyHit()
    {
        DamageTargetEnemy(_chainTargetedDamage[_currentChainedTarget]);
        ++_currentChainedTarget;
    }


    protected override int ComputeDamage()
    {
        this.damage = TurretOwner.Stats.Damage;
        this.damage = (int)((float)this.damage * damageMultiplier);

        return this.damage;
    }
    
}
