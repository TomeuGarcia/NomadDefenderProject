using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageRangeProjectile : HomingProjectile
{
    public override void Init(Enemy targetEnemy, Turret owner)
    {
        float distance = Vector3.Distance(targetEnemy.Position, transform.position);

        int halfDamage = owner.stats.damage / 2;
        int bonus = (int)(halfDamage * distance);

        this.targetEnemy = targetEnemy;
        this.damage = halfDamage + bonus;

        targetEnemy.QueueDamage(damage);

        StartCoroutine(Lifetime());
    }

}
