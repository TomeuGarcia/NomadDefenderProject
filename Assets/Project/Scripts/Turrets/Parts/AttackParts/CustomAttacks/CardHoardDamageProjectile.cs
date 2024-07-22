using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardHoardDamageProjectile : HomingProjectile
{
    [Header("TRAIL")]
    [SerializeField] private TrailRenderer trailRenderer;

    [Header("STATS")]
    [SerializeField, Range(0f, 1f)] private float damageIncrementPer1 = 0.1f;



    public override void ProjectileShotInit(Enemy targetEnemy, TurretBuilding owner)
    {
        turretOwner = owner;

        if (owner.baseDamagePassive != null)
            SetPassiveDamageModifier(owner.baseDamagePassive);

        this.targetEnemy = targetEnemy;
        this.damage = ComputeDamage();
        this.damage = targetEnemy.ComputeDamageWithPassive(this, this.damage, passiveDamageModifier);

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

    private int ComputeDamage()
    {        
        int numberOfCardsInHand = ServiceLocator.GetInstance().CardDrawer.GetCardsInHand().Length;

        trailRenderer.widthMultiplier = 0.2f + (numberOfCardsInHand * 0.4f);

        int baseDamage = turretOwner.Stats.Damage;
        int bonusDamage = (int)(turretOwner.Stats.Damage * damageIncrementPer1  * numberOfCardsInHand);

        return baseDamage + bonusDamage;
    }

}
