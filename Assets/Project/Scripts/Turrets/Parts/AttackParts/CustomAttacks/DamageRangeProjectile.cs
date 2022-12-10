using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageRangeProjectile : HomingProjectile
{
    public override void Init(Enemy targetEnemy, Turret owner)
    {
        float distance = Vector3.Distance(targetEnemy.Position, transform.position);

        int baseDamage = (int)(owner.stats.damage * 0.75f);
        int bonusDamage = owner.stats.damage - baseDamage;
        int bonus = (int)(bonusDamage * distance);

        this.targetEnemy = targetEnemy;
        this.damage = baseDamage + bonus;

        targetEnemy.QueueDamage(damage);

        StartCoroutine(Lifetime());

        Debug.Log(owner.stats.damage + " -> " + this.damage);
    }

}
