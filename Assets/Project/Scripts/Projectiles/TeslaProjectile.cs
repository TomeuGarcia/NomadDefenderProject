using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaProjectile : TurretAttack
{
    [SerializeField] private int targetAmount = 3;
    private int currentTarget;
    private Enemy[] targetedEnemies;


    private void Update()
    {
        MoveTowardsEnemyTarget();

        //IF ROTATION IS NEEDED
        //RotateTowardsEnemyTarget();
    }

    public override void Init(Enemy targetEnemy, Turret owner)
    {      
        this.damage = owner.stats.damage;

        targetedEnemies = owner.GetEnemies(targetAmount);
        for (int i = 0; i < targetedEnemies.Length; i++)
        {
            targetedEnemies[i].QueueDamage(damage);
        }
        currentTarget = 0;
        this.targetEnemy = targetedEnemies[currentTarget];


        StartCoroutine(Lifetime());
    }

    protected override void OnEnemyTriggerEnter(Enemy enemy)
    {
        if (enemy == targetEnemy)
        {
            enemy.TakeDamage(damage);

            if (++currentTarget < targetedEnemies.Length)
            {
                targetEnemy = targetedEnemies[currentTarget];
            }
            else
            {
                StopAllCoroutines();
                Disappear();
            }

        }
    }


}
