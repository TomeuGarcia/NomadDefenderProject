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

    public override void ProjectileShotInit(Enemy targetEnemy, TurretBuilding owner)
    {
        turretOwner = owner;

        if(owner.baseDamagePassive != null)
            SetPassiveDamageModifier(owner.baseDamagePassive);

        this.targetEnemy = targetEnemy;

        this.damage = targetEnemy.ComputeDamageWithPassive(this, owner.stats.damage, passiveDamageModifier);

        targetEnemy.QueueDamage(damage);


        lerp.LerpPosition(targetEnemy.MeshTransform, bulletSpeed);
        StartCoroutine(WaitForLerpFinish());
    }

    public override void ProjectileShotInit_PrecomputedAndQueued(Enemy targetEnemy, TurretBuilding owner, int precomputedDamage)
    {
        turretOwner = owner;

        if (owner.baseDamagePassive != null)
            SetPassiveDamageModifier(owner.baseDamagePassive);

        this.targetEnemy = targetEnemy;
        this.damage = precomputedDamage;

        lerp.LerpPosition(targetEnemy.MeshTransform, bulletSpeed);
        StartCoroutine(WaitForLerpFinish());
    }


    protected IEnumerator WaitForLerpFinish()
    {
        yield return new WaitUntil(() => lerp.finishedPositionLerp);
        EnemyHit();
    }

    protected void EnemyHit()
    {
        GameObject temp = ProjectileParticleFactory.GetInstance().GetAttackParticlesGameObject(attackType, targetEnemy.MeshTransform.position, Quaternion.identity);
        temp.gameObject.SetActive(true);
        temp.transform.parent = gameObject.transform.parent;

        targetEnemy.TakeDamage(this, damage);

        Disappear();
    }
}
