using System;
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
    [SerializeField] private ParticleSystem _hitParticles;

    [SerializeField] private float _distance = 15;
    
    private List<Enemy> _preemptiveEnemyHits;
    private static readonly List<Enemy> NoPreemptiveEnemyHits = new(0);
    
    private float _currentDamageMultiplier = 0f;
    private Vector3 _directionToGoalPosition;
    private Vector3 _goalPosition;

    protected override void ProjectileShotInit(Enemy targetEnemy, TurretBuilding owner)
    {
        base.ProjectileShotInit(_targetEnemy, owner);

        trailRenderer.Clear();
        arrow.SetActive(true);
        damageCollider.enabled = true;
        _targetEnemy = targetEnemy;

        Vector3 directionToEnemy = Vector3.ProjectOnPlane(_targetEnemy.Position - Position, Vector3.up).normalized;
        transform.forward = directionToEnemy;

        ComputeGoalPosition();
        //transform.LookAt(_goalPosition);

        _rigidbody.DOMove(_goalPosition, _distance / MovementSpeed)
            .OnComplete(OnGoalPositionReached);
        
        _damageAttack = CreateDamageAttack(_targetEnemy);
        ComputePreemptiveHits();
        OnShotInitialized();
    }

    protected override void ProjectileShotInit_PrecomputedAndQueued(TurretBuilding owner, TurretDamageAttack precomputedDamageAttack)
    {
        ProjectileShotInit(precomputedDamageAttack.Target, owner);
    }

    private void ComputeGoalPosition()
    {
        _directionToGoalPosition = _targetEnemy.Position - _spawnerObjectPosition;
        _directionToGoalPosition.y = 0;
        _directionToGoalPosition.Normalize();
        
        _goalPosition = _spawnerObjectPosition + (_directionToGoalPosition *_distance);
        _goalPosition.y = _spawnerObjectPosition.y;
    }

    
    private void ComputePreemptiveHits()
    {
        _preemptiveEnemyHits = SpeedUpButton.UsingBuggyTimeScale
            ? ComputeIntersectingEnemies(Position, _directionToGoalPosition, _distance)
            : NoPreemptiveEnemyHits;
    }
    private void ApplyPreemptiveHits()
    {
        foreach (Enemy preemptiveEnemyHit in _preemptiveEnemyHits)
        {
            if (CheckEnemy(preemptiveEnemyHit))
            {
                EnemyHit(preemptiveEnemyHit);
            }
        }
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
        ApplyPreemptiveHits();
        yield return new WaitUntil(() => !disableParticles.activeInHierarchy);
        
        Disable();
    }

    private void OnTriggerEnter(Collider other)
    {
        TryDamageEnemyFromCollider(other);
    }

    private void TryDamageEnemyFromCollider(Collider collider)
    {
        if (CheckEnemyOnTriggerEnter(collider, out Enemy enemy))
        {
            EnemyHit(enemy);
        }
    }

    private void EnemyHit(Enemy enemy)
    {
        GameObject temp = ProjectileParticleFactory.GetInstance()
            .CreateParticlesGameObject(HitParticlesType, enemy.MeshTransform.position, Quaternion.identity);
        temp.transform.parent = gameObject.transform.parent;
        
        _targetEnemy = enemy;
        _damageAttack = CreateDamageAttack(_targetEnemy);
        DamageTargetEnemy(_damageAttack);
        AddEnemyToIgnore(enemy);
        _hitParticles.Play();
    }

    public override bool QueuesDamageToEnemies()
    {
        return false;
    }

    protected override int ComputeDamage()
    {
        return Mathf.RoundToInt(TurretOwner.Stats.Damage * _damageMultiplier);
    }
    
    protected override ITurretProjectileView MakeTurretProjectileView()
    {
        return new TurretSingleProjectileView(_viewAddOnsParent);
    }
}