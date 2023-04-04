using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class PiercingProjectile : TurretPartAttack_Prefab
{
    [SerializeField] private Lerp lerp;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private Collider damageCollider;
    [SerializeField] private float extraDistanceCoef;
    [SerializeField] private float travelTime;

    [SerializeField] private Transform projectile;
    [SerializeField] private GameObject arrow;
    [SerializeField] private GameObject disableParticles;

    [Header("STATS")]
    [SerializeField, Min(0)] private float startDamageMultiplier = 0.5f;
    [SerializeField, Min(1)] private float maxDamageMultiplier = 2.0f;
    [SerializeField, Min(0)] private float damageMultiplierIncrement = 0.25f;
    private float currentDamageMultiplier = 0f;
    private TurretBuilding owner;


    private Vector3 goalPos;
    private bool hitTargetEnemy;
    private int queueDamageAmount;

    public override void ProjectileShotInit(Enemy targetEnemy, TurretBuilding owner)
    {
        hitTargetEnemy = false;
        trailRenderer.Clear();
        arrow.SetActive(true);
        damageCollider.enabled = true;

        this.owner = owner;
        this.targetEnemy = targetEnemy;

        this.currentDamageMultiplier = startDamageMultiplier;
        this.damage = (int)((float)owner.stats.damage * currentDamageMultiplier);

        queueDamageAmount = targetEnemy.QueueDamage(damage, passiveDamageModifier);

        if (owner.baseDamagePassive != null)
            SetPassiveDamageModifier(owner.baseDamagePassive);

        goalPos = transform.position + ((targetEnemy.Position - transform.position).normalized * (owner.stats.range + owner.stats.range * extraDistanceCoef));
        goalPos.y = transform.position.y;

        transform.LookAt(goalPos);

        lerp.LerpPosition(goalPos, travelTime);
        StartCoroutine(WaitForFinish());
    }

    protected IEnumerator WaitForFinish()
    {
        yield return new WaitUntil(() => lerp.finishedPositionLerp == true);

        if(!targetEnemy.IsDead() && !hitTargetEnemy)
        {
            targetEnemy.RemoveQueuedDamage(queueDamageAmount);
        }

        arrow.SetActive(false);
        damageCollider.enabled = false;
        disableParticles.SetActive(true);
        yield return new WaitUntil(() => disableParticles.activeInHierarchy == false);

        Disable();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            EnemyHit(enemy);

            if (targetEnemy == enemy)
            {
                hitTargetEnemy = true;
            }
        }
    }

    protected void EnemyHit(Enemy enemy)
    {
        GameObject temp = ProjectileParticleFactory.GetInstance().GetAttackParticlesGameObject(attackType, enemy.MeshTransform.position, Quaternion.identity);
        temp.gameObject.SetActive(true);
        temp.transform.parent = gameObject.transform.parent;

        enemy.TakeDamage(damage, passiveDamageModifier);

        currentDamageMultiplier += damageMultiplierIncrement;
        currentDamageMultiplier = Mathf.Min(currentDamageMultiplier, maxDamageMultiplier);

        this.damage = (int)((float)owner.stats.damage * currentDamageMultiplier);
    }
}