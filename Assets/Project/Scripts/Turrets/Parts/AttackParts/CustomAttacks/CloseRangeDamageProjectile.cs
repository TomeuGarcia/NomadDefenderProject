using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseRangeDamageProjectile : HomingProjectile
{
    [Header("STATS")]
    [SerializeField, Range(0f, 1f)] private float baseDamagePer1 = 0.25f;
    [SerializeField, Min(0f)] private float distanceInverseMultiplier = 1.0f;

    public override void ProjectileShotInit(Enemy targetEnemy, TurretBuilding owner)
    {
        base.ProjectileShotInit(targetEnemy, owner);

        float distance = Vector3.Distance(targetEnemy.Position, transform.position);
        float distanceInverse = 1f / distance;

        int baseDamage = (int)(owner.stats.damage * baseDamagePer1);
        int baseDamageRemaining = (int)(owner.stats.damage * (1f - baseDamagePer1));
        int bonusDamage = (int)(baseDamageRemaining * distanceInverse * distanceInverseMultiplier);

        this.targetEnemy = targetEnemy;
        this.damage = baseDamage + bonusDamage;

        if (owner.baseDamagePassive != null)
            SetPassiveDamageModifier(owner.baseDamagePassive);

        targetEnemy.QueueDamage(damage, passiveDamageModifier);

        lerp.LerpPosition(targetEnemy.MeshTransform, bulletSpeed);
        StartCoroutine(WaitForLerpFinish());

        //Debug.Log(owner.stats.damage + " -> " + this.damage);
    }

}