using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowFireRateProjectile : HomingProjectile
{
    [Header("STATS")]
    [SerializeField, Min(0f)] private float maxDamageMultiplier = 10.0f;
    [SerializeField] private AnimationCurve damageRateCurve;

    public const float MAX_CADENCE_TIME = 5.0f;


    public override void ProjectileShotInit(Enemy targetEnemy, TurretBuilding owner)
    {
        turretOwner = owner;

        if (owner.BaseDamagePassive != null)
            SetPassiveDamageModifier(owner.BaseDamagePassive);

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

        if (owner.BaseDamagePassive != null)
            SetPassiveDamageModifier(owner.BaseDamagePassive);

        this.targetEnemy = targetEnemy;
        this.damage = precomputedDamage;

        lerp.LerpPosition(targetEnemy.MeshTransform, bulletSpeed);
        StartCoroutine(WaitForLerpFinish());
    }

    private int ComputeDamage()
    {        
        float currentTime = Mathf.Min(turretOwner.TimeSinceLastShot, MAX_CADENCE_TIME);
        
        float curveCoefficient = currentTime / MAX_CADENCE_TIME;
        float curveValue = damageRateCurve.Evaluate(curveCoefficient);

        int damage = (int)(curveValue * turretOwner.Stats.Damage * maxDamageMultiplier);
        return damage;        
    }



}
