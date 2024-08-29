using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseRangeDamageProjectile : HomingProjectile
{
    [Header("TRAIL")]
    [SerializeField] private TrailRenderer trailRenderer;
    private Coroutine decreaseInSizeCoroutine;

    [Header("STATS")]
    [SerializeField] private AnimationCurve _damageMultiplierOverDistance = AnimationCurve.EaseInOut(0,2,10,0);

    private void OnDisable()
    {
        if (decreaseInSizeCoroutine != null)
        {
            StopCoroutine(decreaseInSizeCoroutine);
            decreaseInSizeCoroutine = null;
        }
    }

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

        decreaseInSizeCoroutine = StartCoroutine(DecreaseSizeOvertime());
    }

    public override void ProjectileShotInit_PrecomputedAndQueued(Enemy targetEnemy, TurretBuilding owner, int precomputedDamage)
    {
        turretOwner = owner;

        this.targetEnemy = targetEnemy;
        this.damage = precomputedDamage;

        lerp.LerpPosition(targetEnemy.MeshTransform, bulletSpeed);
        StartCoroutine(WaitForLerpFinish());

    }
    

    protected override int ComputeDamage()
    {
        float distance = Vector3.Distance(targetEnemy.GetPosition(), transform.position);
        int baseDamage = turretOwner.Stats.Damage;
        return (int)(baseDamage * _damageMultiplierOverDistance.Evaluate(distance));
    }
    */

    protected override void OnShotInitialized()
    {
        decreaseInSizeCoroutine = StartCoroutine(DecreaseSizeOvertime());
    }


    private IEnumerator DecreaseSizeOvertime()
    {
        trailRenderer.widthMultiplier = 1f;
        float scale = 2f;

        while (Vector3.Distance(_targetEnemy.Position, transform.position) > 0.1f)
        {
            scale -= Time.deltaTime * 16f;
            scale = Mathf.Clamp(scale, 0.15f, 10f);
            trailRenderer.widthMultiplier = scale;

            yield return null;
        }
        decreaseInSizeCoroutine = null;
    }

}