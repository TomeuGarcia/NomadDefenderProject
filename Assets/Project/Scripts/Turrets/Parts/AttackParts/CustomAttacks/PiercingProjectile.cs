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

    [SerializeField] private float _distance = 15;
    
    private float _currentDamageMultiplier = 0f;
    private Vector3 _goalPosition;

    protected override void ProjectileShotInit(Enemy targetEnemy, TurretBuilding owner)
    {
        base.ProjectileShotInit(base._targetEnemy, owner);

        trailRenderer.Clear();
        arrow.SetActive(true);
        damageCollider.enabled = true;

        this._targetEnemy = targetEnemy;

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
        _goalPosition = transform.position + ((_targetEnemy.Position - transform.position).normalized *_distance);
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
            .CreateParticlesGameObject(HitParticlesType, enemy.MeshTransform.position, Quaternion.identity);
        temp.transform.parent = gameObject.transform.parent;

        _damageAttack = CreateDamageAttack(_targetEnemy);
        DamageTargetEnemy(_damageAttack);
    }

    public override bool QueuesDamageToEnemies()
    {
        return false;
    }

    protected override int ComputeDamage()
    {
        return TurretOwner.Stats.Damage;
    }
}