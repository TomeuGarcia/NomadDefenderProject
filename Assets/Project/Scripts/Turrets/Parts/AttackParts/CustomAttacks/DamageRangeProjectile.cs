using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class DamageRangeProjectile : HomingProjectile
{
    [Header("STATS")]
    [SerializeField, Range(0f, 1f)] private float baseDamagePer1 = 0.75f;
    [SerializeField, Min(0f)] private float distanceMultiplier = 1.0f;

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

        //Debug.Log(owner.stats.damage + " -> " + this.damage);
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
        float distance = Vector3.Distance(targetEnemy.GetPosition(), transform.position);

        int baseDamage = (int)(turretOwner.stats.damage * baseDamagePer1);
        int baseDamageRemaining = (int)(turretOwner.stats.damage * (1f - baseDamagePer1));
        int bonusDamage = (int)(baseDamageRemaining * distance * distanceMultiplier);

        return baseDamage + bonusDamage;
    }

}
