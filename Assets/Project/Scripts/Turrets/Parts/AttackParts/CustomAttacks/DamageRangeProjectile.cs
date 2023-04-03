using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DamageRangeProjectile : HomingProjectile
{
    public override void ProjectileShotInit(Enemy targetEnemy, TurretBuilding owner)
    {
        float distance = Vector3.Distance(targetEnemy.Position, transform.position);

        int baseDamage = (int)(owner.stats.damage * 0.75f);
        int bonusDamage = owner.stats.damage - baseDamage;
        int bonus = (int)(bonusDamage * distance);

        this.targetEnemy = targetEnemy;
        this.damage = baseDamage + bonus;

        if (owner.baseDamagePassive != null)
            SetPassiveDamageModifier(owner.baseDamagePassive);

        targetEnemy.QueueDamage(damage, passiveDamageModifier);

        lerp.LerpPosition(targetEnemy.MeshTransform, bulletSpeed);
        StartCoroutine(WaitForLerpFinish());

        //Debug.Log(owner.stats.damage + " -> " + this.damage);
    }

}
