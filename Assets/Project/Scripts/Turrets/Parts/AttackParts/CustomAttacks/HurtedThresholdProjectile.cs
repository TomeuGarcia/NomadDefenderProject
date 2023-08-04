using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtedThresholdProjectile : HomingProjectile
{
    [Header("STATS")]
    [SerializeField, Range(0f, 5f)] private float damageMultiplier = 2.0f;
    [SerializeField, Range(0, 5)] private int healthThresholdApplyMultiplier = 2;
    public int HealthThreshold => healthThresholdApplyMultiplier;


    public override void ProjectileShotInit(Enemy targetEnemy, TurretBuilding owner)
    {
        turretOwner = owner;

        if (owner.baseDamagePassive != null)
            SetPassiveDamageModifier(owner.baseDamagePassive);

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

        if (owner.baseDamagePassive != null)
            SetPassiveDamageModifier(owner.baseDamagePassive);

        this.targetEnemy = targetEnemy;

        this.damage = precomputedDamage;

        lerp.LerpPosition(targetEnemy.MeshTransform, bulletSpeed);
        StartCoroutine(WaitForLerpFinish());
    }

    private int ComputeDamage()
    {
        int damage = (int)(turretOwner.stats.damage);

        int highestHealth = ServiceLocator.GetInstance().TDLocationsUtils.GetHighestLocationHealth();

        if (highestHealth <= healthThresholdApplyMultiplier)
        {
            damage += (int)(damageMultiplier * damage);
        }

        return damage;
    }
}
