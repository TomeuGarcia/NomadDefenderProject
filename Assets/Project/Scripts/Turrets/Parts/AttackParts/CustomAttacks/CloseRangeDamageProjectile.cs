using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseRangeDamageProjectile : HomingProjectile
{
    [Header("TRAIL")]
    [SerializeField] private TrailRenderer trailRenderer;
    private Coroutine decreaseInSizeCoroutine;

    [Header("STATS")]
    [SerializeField, Range(0f, 1f)] private float baseDamagePer1 = 0.25f;
    [SerializeField, Min(0f)] private float distanceInverseMultiplier = 1.0f;
    [SerializeField] private AnimationCurve _damageMultiplierOverDistance = AnimationCurve.EaseInOut(0,2,10,0);

    private void OnDisable()
    {
        if (decreaseInSizeCoroutine != null)
        {
            StopCoroutine(decreaseInSizeCoroutine);
            decreaseInSizeCoroutine = null;
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

        decreaseInSizeCoroutine = StartCoroutine(DecreaseSizeOvertime());
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

        decreaseInSizeCoroutine = StartCoroutine(DecreaseSizeOvertime());
    }

    private int ComputeDamage()
    {
        float distance = Vector3.Distance(targetEnemy.GetPosition(), transform.position);
        int baseDamage = turretOwner.Stats.Damage;
        return (int)(baseDamage * _damageMultiplierOverDistance.Evaluate(distance));
    }


    private IEnumerator DecreaseSizeOvertime()
    {
        trailRenderer.widthMultiplier = 1f;
        float scale = 2f;

        while (Vector3.Distance(targetEnemy.Position, transform.position) > 0.1f)
        {
            scale -= Time.deltaTime * 16f;
            scale = Mathf.Clamp(scale, 0.15f, 10f);
            trailRenderer.widthMultiplier = scale;

            yield return null;
        }
        decreaseInSizeCoroutine = null;
    }

}