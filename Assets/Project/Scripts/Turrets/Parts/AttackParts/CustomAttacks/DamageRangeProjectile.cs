using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class DamageRangeProjectile : HomingProjectile
{
    [Header("TRAIL")]
    [SerializeField] private TrailRenderer trailRenderer;
    private Coroutine growInSizeCoroutine = null;

    [Header("STATS")]
    [SerializeField] private AnimationCurve _damageMultiplierOverDistance = AnimationCurve.EaseInOut(0,0,10,4);


    private void Start()
    {
        _damageMultiplierOverDistance.postWrapMode = WrapMode.Clamp;
    }

    private void OnDisable()
    {
        if (growInSizeCoroutine != null)
        {
            StopCoroutine(growInSizeCoroutine);
            growInSizeCoroutine = null;
        }
    }

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

        growInSizeCoroutine = StartCoroutine(GrowSizeOvertime());
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

        growInSizeCoroutine = StartCoroutine(GrowSizeOvertime());
    }

    private int ComputeDamage()
    {
        float distance = Vector3.Distance(targetEnemy.GetPosition(), transform.position);
        int baseDamage = turretOwner.Stats.Damage;
        return (int)(baseDamage * _damageMultiplierOverDistance.Evaluate(distance));
    }


    private IEnumerator GrowSizeOvertime()
    {        
        trailRenderer.widthMultiplier = 1f;
        float scale = 1.1f;

        while (Vector3.Distance(targetEnemy.Position, transform.position) > 0.1f)
        {
            scale += scale * Time.deltaTime * 10f;
            trailRenderer.widthMultiplier = scale;

            yield return null;
        }
        growInSizeCoroutine = null;
    }

}
