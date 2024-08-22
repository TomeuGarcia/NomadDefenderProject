using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardHoardDamageProjectile : HomingProjectile
{
    [Header("TRAIL")]
    [SerializeField] private TrailRenderer trailRenderer;

    [Header("STATS")]
    [SerializeField, Range(0f, 1f)] private float damageIncrementPer1 = 0.1f;


/*
    public override void ProjectileShotInit(Enemy targetEnemy, TurretBuilding owner)
    {
        turretOwner = owner;

        this.targetEnemy = targetEnemy;
        this.damage = ComputeDamage();
        this.damage = targetEnemy.ComputeDamageWithPassive(this, this.damage);

        targetEnemy.QueueDamage(damage);

        lerp.LerpPosition(targetEnemy.MeshTransform, bulletSpeed);
        StartCoroutine(WaitForLerpFinish());
    }

    public override void ProjectileShotInit_PrecomputedAndQueued(Enemy targetEnemy, TurretBuilding owner, int precomputedDamage)
    {
        turretOwner = owner;
        
        this.targetEnemy = targetEnemy;
        this.damage = precomputedDamage;

        lerp.LerpPosition(targetEnemy.MeshTransform, bulletSpeed);
        StartCoroutine(WaitForLerpFinish());
    }
*/

    protected override void OnShotInitialized()
    {
        int numberOfCardsInHand = ServiceLocator.GetInstance().CardDrawer.GetCardsInHand().Length;
        trailRenderer.widthMultiplier = 0.2f + (numberOfCardsInHand * 0.4f);
    }

    /*
    protected override int ComputeDamage()
    {
        int numberOfCardsInHand = ServiceLocator.GetInstance().CardDrawer.GetCardsInHand().Length;

        trailRenderer.widthMultiplier = 0.2f + (numberOfCardsInHand * 0.4f);

        int baseDamage = turretOwner.Stats.Damage;
        int bonusDamage = (int)(baseDamage * damageIncrementPer1 * numberOfCardsInHand);

        return baseDamage + bonusDamage;
    }
    */

}
