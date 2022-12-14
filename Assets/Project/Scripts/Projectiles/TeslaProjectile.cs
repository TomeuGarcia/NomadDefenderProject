using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaProjectile : TurretAttack
{
    [SerializeField] private Lerp lerp;

    [SerializeField] private int maxTargetAmount = 3;
    [SerializeField] private float range;
    private int currentTarget;
    private Enemy[] targetedEnemies;


    protected override void DoUpdate()
    {
    }

    public override void Init(Enemy targetEnemy, Turret owner)
    {      
        this.damage = owner.stats.damage;

        targetedEnemies = owner.GetNearestEnemies(maxTargetAmount, range);
        for (int i = 0; i < targetedEnemies.Length; i++)
        {
            targetedEnemies[i].QueueDamage(damage);
        }
        currentTarget = 0;
        this.targetEnemy = targetedEnemies[currentTarget];

        lerp.LerpPosition(targetEnemy.MeshTransform, bulletSpeed);
        StartCoroutine(WaitForLerpFinish());
    }

    IEnumerator WaitForLerpFinish()
    {
        yield return new WaitUntil(() => lerp.finishedPositionLerp == true);
        EnemyHit();
    }

    void EnemyHit()
    {
        GameObject temp = ProjectileParticleFactory.GetInstance().GetAttackParticlesGameObject(attackType, targetEnemy.MeshTransform.position, Quaternion.identity);
        temp.gameObject.SetActive(true);

        targetEnemy.TakeDamage(damage);

        if (++currentTarget < targetedEnemies.Length)
        {
            targetEnemy = targetedEnemies[currentTarget];

            lerp.LerpPosition(targetEnemy.Position, bulletSpeed);
            StartCoroutine(WaitForLerpFinish());
        }
        else
        {
            //StopAllCoroutines();
            Disappear();
        }
    }
}
