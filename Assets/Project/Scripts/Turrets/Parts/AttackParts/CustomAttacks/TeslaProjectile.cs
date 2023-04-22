using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaProjectile : TurretPartAttack_Prefab
{
    [SerializeField] private Lerp lerp;

    [Header("STATS")]
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField, Range(0f, 1f)] private float damageMultiplier = 0.5f;
    [SerializeField, Min(1)] private int maxChainedTargets = 1;
    [SerializeField, Min(0f)] private float chainRadius;
    [SerializeField, Min(0f)] private float stunDuration;

    private int currentChainedTarget;
    private Enemy[] chainTargetedEnemies;
    private int[] chainTargetedDamage;


    protected override void DoUpdate()
    {
    }

    public override void ProjectileShotInit(Enemy targetEnemy, TurretBuilding owner)
    {
        turretOwner = owner;

        this.targetEnemy = targetEnemy;


        this.currentChainedTarget = 0;

        this.damage = owner.stats.damage;
        this.damage = (int)((float)this.damage * damageMultiplier);
        this.damage = targetEnemy.ComputeDamageWithPassive(this, this.damage, passiveDamageModifier);

        targetEnemy.QueueDamage(this.damage);


        if (owner.baseDamagePassive != null)
            SetPassiveDamageModifier(owner.baseDamagePassive);

        lerp.LerpPosition(targetEnemy.MeshTransform, bulletSpeed);
        StartCoroutine(WaitForLerpFinish(false));
    }

    public override void ProjectileShotInit_PrecomputedAndQueued(Enemy targetEnemy, TurretBuilding owner, int precomputedDamage)
    {
        turretOwner = owner;

        this.targetEnemy = targetEnemy;


        this.currentChainedTarget = 0;

        this.damage = precomputedDamage;


        if (owner.baseDamagePassive != null)
            SetPassiveDamageModifier(owner.baseDamagePassive);

        lerp.LerpPosition(targetEnemy.MeshTransform, bulletSpeed);
        StartCoroutine(WaitForLerpFinish(false));
    }


    IEnumerator WaitForLerpFinish(bool isChaining)
    {
        yield return new WaitUntil(() => lerp.finishedPositionLerp == true);
        EnemyHit(isChaining);
    }


    void EnemyHit(bool isChaining)
    {
        GameObject hitParticles = ProjectileParticleFactory.GetInstance().GetAttackParticlesGameObject(attackType, targetEnemy.MeshTransform.position, Quaternion.identity);
        hitParticles.gameObject.SetActive(true);
        hitParticles.transform.parent = gameObject.transform.parent;

        
        targetEnemy.GetStunned(stunDuration);

        if (isChaining)
        {
            targetEnemy.TakeDamage(this, chainTargetedDamage[currentChainedTarget]);
            ++currentChainedTarget;
        }
        else
        {
            targetEnemy.TakeDamage(this, damage);


            chainTargetedEnemies = GetNearestEnemiesToTargetedEnemy(targetEnemy, maxChainedTargets, chainRadius, enemyLayerMask);
            chainTargetedDamage = new int[chainTargetedEnemies.Length];
            for (int i = 0; i < chainTargetedEnemies.Length; i++)
            {
                chainTargetedDamage[i] = chainTargetedEnemies[i].ComputeDamageWithPassive(this, this.damage, passiveDamageModifier);
                chainTargetedEnemies[i].QueueDamage(chainTargetedDamage[i]);
            }

            
            Vector3 wavePosition = Position;
            wavePosition.y = turretOwner.Position.y;

            //GameObject waveParticles = ProjectileParticleFactory.GetInstance()
            //    .GetAttackParticlesGameObject(ProjectileParticleFactory.ProjectileParticleType.TESLA_WAVE, wavePosition, Quaternion.identity);
            //waveParticles.gameObject.SetActive(true);
            //waveParticles.transform.parent = gameObject.transform.parent;
            //waveParticles.transform.localScale = Vector3.one * (chainRadius * 2f);
        }


        if (currentChainedTarget < chainTargetedEnemies.Length)
        {
            targetEnemy = chainTargetedEnemies[currentChainedTarget];

            lerp.LerpPosition(targetEnemy.MeshTransform, bulletSpeed / 2.0f);
            StartCoroutine(WaitForLerpFinish(true));            
        }
        else
        {
            //StopAllCoroutines();
            Disappear();
        }
    }



}
