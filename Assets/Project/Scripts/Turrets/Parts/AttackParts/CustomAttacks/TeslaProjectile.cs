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
    private bool attackChained = false;


    protected override void DoUpdate()
    {
    }

    public override void ProjectileShotInit(Enemy targetEnemy, TurretBuilding owner)
    {
        base.ProjectileShotInit(targetEnemy, owner);

        this.damage = owner.stats.damage;
        this.damage = (int)((float)this.damage * damageMultiplier);

        this.attackChained = false;


        this.targetEnemy = targetEnemy;
        targetEnemy.QueueDamage(this.damage, passiveDamageModifier);

        this.currentChainedTarget = 0;

        if (owner.baseDamagePassive != null)
            SetPassiveDamageModifier(owner.baseDamagePassive);

        lerp.LerpPosition(targetEnemy.MeshTransform, bulletSpeed);
        StartCoroutine(WaitForLerpFinish());
    }

    IEnumerator WaitForLerpFinish()
    {
        yield return new WaitUntil(() => lerp.finishedPositionLerp == true);
        EnemyHit();
    }

    void EnemyHit()
    {
        GameObject hitParticles = ProjectileParticleFactory.GetInstance().GetAttackParticlesGameObject(attackType, targetEnemy.MeshTransform.position, Quaternion.identity);
        hitParticles.gameObject.SetActive(true);
        hitParticles.transform.parent = gameObject.transform.parent;

        targetEnemy.TakeDamage(this, damage, passiveDamageModifier);
        targetEnemy.GetStunned(stunDuration);

        if (!attackChained)
        {
            chainTargetedEnemies = GetNearestEnemiesToTargetedEnemy(targetEnemy, maxChainedTargets, chainRadius, enemyLayerMask);
            for (int i = 0; i < chainTargetedEnemies.Length; i++)
            {
                chainTargetedEnemies[i].QueueDamage(this.damage, passiveDamageModifier);
            }

            attackChained = true;
            
            Vector3 wavePosition = Position;
            wavePosition.y = turretOwner.Position.y;

            GameObject waveParticles = ProjectileParticleFactory.GetInstance()
                .GetAttackParticlesGameObject(ProjectileParticleFactory.ProjectileParticleType.TESLA_WAVE, wavePosition, Quaternion.identity);
            waveParticles.gameObject.SetActive(true);
            waveParticles.transform.parent = gameObject.transform.parent;
            waveParticles.transform.localScale = Vector3.one * (chainRadius * 2f);
        }

        if (currentChainedTarget < chainTargetedEnemies.Length)
        {
            targetEnemy = chainTargetedEnemies[currentChainedTarget];

            lerp.LerpPosition(targetEnemy.MeshTransform, bulletSpeed / 2.0f);
            StartCoroutine(WaitForLerpFinish());

            ++currentChainedTarget;
        }
        else
        {
            //StopAllCoroutines();
            Disappear();
        }
    }



}
