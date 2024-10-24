using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class HomingProjectile : TurretPartAttack_Prefab
{
    [SerializeField] protected Lerp lerp;

    protected override void DoUpdate()
    {
    }

    public override void Init(Enemy targetEnemy, TurretBuilding owner)
    {
        this.targetEnemy = targetEnemy;
        this.damage = owner.stats.damage;

        targetEnemy.QueueDamage(damage, passiveDamageModifier);

        if(owner.baseDamagePassive != null)
            SetPassiveDamageModifier(owner.baseDamagePassive);

        lerp.LerpPosition(targetEnemy.MeshTransform, bulletSpeed);
        StartCoroutine(WaitForLerpFinish());
    }

    protected IEnumerator WaitForLerpFinish()
    {
        yield return new WaitUntil(() => lerp.finishedPositionLerp == true);
        EnemyHit();
    }

    protected void EnemyHit()
    {
        GameObject temp = ProjectileParticleFactory.GetInstance().GetAttackParticlesGameObject(attackType, targetEnemy.MeshTransform.position, Quaternion.identity);
        temp.gameObject.SetActive(true);
        temp.transform.parent = gameObject.transform.parent;

        targetEnemy.TakeDamage(damage, passiveDamageModifier);
        //StopAllCoroutines();
        Disappear();
    }
}
