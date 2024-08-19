using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BerserkerProjectile : HomingProjectile
{
    [Header("VISUALS")]
    [SerializeField] private TrailRenderer trailRenderer;    


    public override void ProjectileShotInit(Enemy targetEnemy, TurretBuilding owner)
    {
        turretOwner = owner;

        if (owner.BaseDamagePassive != null)
            SetPassiveDamageModifier(owner.BaseDamagePassive);

        this.targetEnemy = targetEnemy;

        this.damage = ComputeDamage();
        this.damage = targetEnemy.ComputeDamageWithPassive(this, this.damage, passiveDamageModifier);

        targetEnemy.QueueDamage(damage);

        lerp.LerpPosition(targetEnemy.MeshTransform, bulletSpeed);
        StartCoroutine(WaitForLerpFinish());
    }

    public override void ProjectileShotInit_PrecomputedAndQueued(Enemy targetEnemy, TurretBuilding owner, int precomputedDamage)
    {
        turretOwner = owner;

        if (owner.BaseDamagePassive != null)
            SetPassiveDamageModifier(owner.BaseDamagePassive);

        this.targetEnemy = targetEnemy;

        this.damage = precomputedDamage;

        lerp.LerpPosition(targetEnemy.MeshTransform, bulletSpeed);
        StartCoroutine(WaitForLerpFinish());
    }

    private int ComputeDamage()
    {
        int damage = turretOwner.Stats.Damage;

        trailRenderer.widthMultiplier = BerserkerTurretBuildingVisuals.IsBerserkEnabled ? 2.5f : 1.0f;

        return damage;
    }

}