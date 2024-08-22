using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BerserkerProjectile : HomingProjectile
{
    [Header("VISUALS")]
    [SerializeField] private TrailRenderer trailRenderer;    

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
        int damage = turretOwner.Stats.Damage;
        return damage;
    }

    protected override void OnShotInitialized()
    {
        base.OnShotInitialized();
        //trailRenderer.widthMultiplier = BerserkerTurretBuildingVisuals.IsBerserkEnabled ? 2.5f : 1.0f;
    }
}