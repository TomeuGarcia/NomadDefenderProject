using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowFireRateProjectile : HomingProjectile
{
    [Header("STATS")]
    [SerializeField, Range(0f, 1f)] private float baseDamagePer1 = 0.75f;
    [SerializeField, Min(0f)] private float distanceMultiplier = 1.0f;

    [SerializeField, Min(0f)] private float maxDamageMultiplier = 10.0f;
    [SerializeField] private AnimationCurve damageRateCurve;



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
        float maxTime = TurretPartBody.GetSlowestCadence();
        float currentTime = Mathf.Min(turretOwner.TimeSinceLastShot, maxTime);
        
        float curveCoefficient = currentTime / maxTime;
        float curveValue = damageRateCurve.Evaluate(curveCoefficient);

        int damage = (int)(curveValue * turretOwner.stats.damage * maxDamageMultiplier);
        return damage;        
    }



}
