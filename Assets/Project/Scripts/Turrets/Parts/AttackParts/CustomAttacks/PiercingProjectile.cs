using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.ProBuilder;

public class PiercingProjectile : ATurretProjectileBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private Collider damageCollider;

    [SerializeField] private GameObject arrow;
    [SerializeField] private GameObject disableParticles;

    [SerializeField] private float _distance = 15;
    
    private float _currentDamageMultiplier = 0f;
    private Vector3 _goalPosition;

    protected override void ProjectileShotInit(Enemy targetEnemy, TurretBuilding owner)
    {
        base.ProjectileShotInit(_targetEnemy, owner);

        trailRenderer.Clear();
        arrow.SetActive(true);
        damageCollider.enabled = true;

        _targetEnemy = targetEnemy;

        ComputeGoalPosition();
        transform.LookAt(_goalPosition);

        _rigidbody.DOMove(_goalPosition, _distance / MovementSpeed)
            .OnComplete(OnGoalPositionReached);
        
        
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


    private void OnGoalPositionReached()
    {
        StartCoroutine(WaitForFinish());
    }
    private IEnumerator WaitForFinish()
    {
        arrow.SetActive(false);
        damageCollider.enabled = false;
        disableParticles.SetActive(true);
        yield return new WaitUntil(() => !disableParticles.activeInHierarchy);

        Disable();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (CheckEnemyOnTriggerEnter(other, out Enemy enemy))
        {
            EnemyHit(enemy);
        }
    }
    
    private void EnemyHit(Enemy enemy)
    {
        GameObject temp = ProjectileParticleFactory.GetInstance()
            .CreateParticlesGameObject(HitParticlesType, enemy.MeshTransform.position, Quaternion.identity);
        temp.transform.parent = gameObject.transform.parent;

        _damageAttack = CreateDamageAttack(_targetEnemy);
        DamageTargetEnemy(_damageAttack);
        AddEnemyToIgnore(enemy);
    }

    public override bool QueuesDamageToEnemies()
    {
        return false;
    }

    protected override int ComputeDamage()
    {
        return TurretOwner.Stats.Damage;
    }
    
    protected override ITurretProjectileView MakeTurretProjectileView()
    {
        return new TurretSingleProjectileView(_viewAddOnsParent);
    }
}