using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class PiercingProjectile : ATurretProjectileBehaviour
{
    [SerializeField] private Lerp lerp;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private Collider damageCollider;
    [SerializeField] private float travelTime;

    [SerializeField] private GameObject arrow;
    [SerializeField] private GameObject disableParticles;

    [Header("STATS")]
    [SerializeField, Min(0)] private float startDamageMultiplier = 0.5f;
    [SerializeField, Min(1)] private float maxDamageMultiplier = 2.0f;
    [SerializeField, Min(0)] private float damageMultiplierIncrement = 0.25f;
    [SerializeField] private float _distance = 15;
    
    private float _currentDamageMultiplier = 0f;
    private Vector3 _goalPosition;

    protected override void ProjectileShotInit(Enemy targetEnemy, TurretBuilding owner)
    {
        base.ProjectileShotInit(base.targetEnemy, owner);

        trailRenderer.Clear();
        arrow.SetActive(true);
        damageCollider.enabled = true;

        this.targetEnemy = targetEnemy;

        ComputeGoalPosition();
        transform.LookAt(_goalPosition);

        lerp.LerpPosition(_goalPosition, travelTime);
        StartCoroutine(WaitForFinish());
        
        OnShotInitialized();
    }

    protected override void ProjectileShotInit_PrecomputedAndQueued(TurretBuilding owner, TurretDamageAttack precomputedDamageAttack)
    {
        ProjectileShotInit(precomputedDamageAttack.Target, owner);
    }

    private void ComputeGoalPosition()
    {
        _goalPosition = transform.position + ((targetEnemy.Position - transform.position).normalized *_distance);
        _goalPosition.y = transform.position.y;
    }


    private IEnumerator WaitForFinish()
    {
        yield return new WaitUntil(() => lerp.finishedPositionLerp);

        arrow.SetActive(false);
        damageCollider.enabled = false;
        disableParticles.SetActive(true);
        yield return new WaitUntil(() => !disableParticles.activeInHierarchy);

        Disable();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.CompareTag("Enemy"))
        {
            return;
        }
        
        Enemy enemy = other.gameObject.GetComponent<Enemy>();
        EnemyHit(enemy);
    }

    private void EnemyHit(Enemy enemy)
    {
        GameObject temp = ProjectileParticleFactory.GetInstance()
            .GetAttackParticlesGameObject(ProjectileType, enemy.MeshTransform.position, Quaternion.identity);
        temp.gameObject.SetActive(true);
        temp.transform.parent = gameObject.transform.parent;

        _damageAttack = new TurretDamageAttack(this, enemy, ComputeDamage());
        DamageTargetEnemy(_damageAttack);

        _currentDamageMultiplier += damageMultiplierIncrement;
        _currentDamageMultiplier = Mathf.Min(_currentDamageMultiplier, maxDamageMultiplier);
    }

    public override bool QueuesDamageToEnemies()
    {
        return false;
    }

    private int ComputeDamage()
    {
        return this.damage;
        this._currentDamageMultiplier = startDamageMultiplier;
        this.damage = (int)((float)TurretOwner.Stats.Damage * _currentDamageMultiplier);

        return this.damage;
    }
}