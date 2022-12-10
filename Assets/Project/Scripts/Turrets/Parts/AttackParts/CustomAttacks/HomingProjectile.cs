using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class HomingProjectile : TurretPartAttack_Prefab
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
            GameObject temp = ProjectileParticleFactory.GetInstance().GetAttackParticlesGameObject(attackType, enemy.MeshTransform.position, Quaternion.identity);
            temp.gameObject.SetActive(true);

            enemy.TakeDamage(damage);
            StopAllCoroutines();
            Disappear();
        }
    }
}
