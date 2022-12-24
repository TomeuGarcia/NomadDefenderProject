using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaProjectile : TurretPartAttack_Prefab
{
    [SerializeField] private Lerp lerp;

    [SerializeField] private int maxTargetAmount = 3;
    [SerializeField] private float range;
    private int currentTarget;
    private Enemy[] targetedEnemies;


    protected override void DoUpdate()
    {
    }

    public override void Init(Enemy targetEnemy, TurretBuilding owner)
    {      
        this.damage = owner.stats.damage;

        targetedEnemies = owner.GetNearestEnemies(maxTargetAmount, range);
        for (int i = 0; i < targetedEnemies.Length; i++)
        {
            targetedEnemies[i].QueueDamage(damage, passiveDamageModifier);
        }
        currentTarget = 0;
        this.targetEnemy = targetedEnemies[currentTarget];

        if (owner.baseDamagePassive != null)
            SetPassiveDamageModifier(owner.baseDamagePassive);

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

        targetEnemy.TakeDamage(damage, passiveDamageModifier);

        if (++currentTarget < targetedEnemies.Length)
        {
            Debug.Log(currentTarget);
            targetEnemy = targetedEnemies[currentTarget];

            lerp.LerpPosition(targetedEnemies[currentTarget].MeshTransform, bulletSpeed / 2.0f);
            StartCoroutine(WaitForLerpFinish());
        }
        else
        {
            //StopAllCoroutines();
            Disappear();
        }
    }
}
