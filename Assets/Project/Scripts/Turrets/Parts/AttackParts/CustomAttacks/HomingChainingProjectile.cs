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
            .CreateParticlesGameObject(HitParticlesType, _targetEnemy.MeshTransform.position, Quaternion.identity);
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
            _targetEnemy = _chainTargetedEnemies[_currentChainedTarget];
            lerp.LerpPosition(_targetEnemy.MeshTransform, MovementSpeed / 2.0f);
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


        _chainTargetedEnemies = GetNearestEnemiesToTargetedEnemy(_targetEnemy, maxChainedTargets, chainRadius, enemyLayerMask);
        _chainTargetedDamage = new TurretDamageAttack[_chainTargetedEnemies.Length];
        for (int i = 0; i < _chainTargetedEnemies.Length; i++)
        {
            Enemy chainedEnemy = _chainTargetedEnemies[i];
            
            _chainTargetedDamage[i] = CreateDamageAttack(chainedEnemy);
                //new TurretDamageAttack(_damageAttack.ProjectileSource, chainedEnemy, _damageAttack.Damage);
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
        int damage = TurretOwner.Stats.Damage;
        damage = (int)(damage * damageMultiplier);
        return damage;
    }
    
}
