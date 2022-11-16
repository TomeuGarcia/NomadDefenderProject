using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class HomingProjectile : TurretAttack
{

    protected override void DoUpdate()
    {
        MoveTowardsEnemyTarget();

        //IF ROTATION IS NEEDED
        //RotateTowardsEnemyTarget();
    }

    public override void Init(Enemy targetEnemy, Turret owner)
    {
        this.targetEnemy = targetEnemy;
        this.damage = owner.stats.damage;

        targetEnemy.QueueDamage(damage);

        StartCoroutine(Lifetime());
    }

    protected override void OnEnemyTriggerEnter(Enemy enemy)
    {
        if (enemy == targetEnemy)
        {
            enemy.TakeDamage(damage);
            StopAllCoroutines();
            Disappear();
        }
    }

    protected override void ActivateParticles()
    {
        hitParticles.transform.position = lastHit.ClosestPointOnBounds(gameObject.transform.position);
        hitParticles.transform.forward = hitParticles.transform.position - lastHit.transform.position;

        hitParticles.Play();
    }
}
