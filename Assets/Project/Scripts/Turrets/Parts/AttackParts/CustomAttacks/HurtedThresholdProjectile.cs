using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtedThresholdProjectile : HomingProjectile
{
    [Header("STATS")]
    [SerializeField, Range(0f, 5f)] private float damageMultiplier = 2.0f;
    [SerializeField, Range(0, 5)] private int healthThresholdApplyMultiplier = 2;

    public int HealthThreshold => healthThresholdApplyMultiplier;


    [Header("VISUALS")]
    [SerializeField] private TrailRenderer trilRenderer;


/*
    public override void ProjectileShotInit(Enemy targetEnemy, TurretBuilding owner)
    {
        turretOwner = owner;
        
        this.targetEnemy = targetEnemy;

        this.damage = ComputeDamage();
        this.damage = targetEnemy.ComputeDamageWithPassive(this, this.damage);

        targetEnemy.QueueDamage(damage);

        lerp.LerpPosition(targetEnemy.MeshTransform, bulletSpeed);
        StartCoroutine(WaitForLerpFinish());
    }

    public override void ProjectileShotInit_PrecomputedAndQueued(Enemy targetEnemy, TurretBuilding owner, int precomputedDamage)
    {
        turretOwner = owner;
        
        this.targetEnemy = targetEnemy;

        this.damage = precomputedDamage;

        lerp.LerpPosition(targetEnemy.MeshTransform, bulletSpeed);
        StartCoroutine(WaitForLerpFinish());
    }
    */

    protected override int ComputeDamage()
    {
        int damage = (int)(TurretOwner.Stats.Damage);

        int highestHealth = ServiceLocator.GetInstance().TDLocationsUtils.GetHighestLocationHealth();

        if (highestHealth <= healthThresholdApplyMultiplier)
        {
            damage += (int)(damageMultiplier * damage);
            trilRenderer.widthMultiplier = 2.5f;
        }
        else
        {
            trilRenderer.widthMultiplier = 1.0f;
        }

        return damage;
    }

}
