
using System;
using UnityEngine;

public class OrbitingProjectile : ATurretProjectileBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private GameObject _sphereHolder;
    [SerializeField] private TrailRenderer _trailRenderer;

    private const float TOTAL_DAMAGE_MULTIPLIER = 0.3f;

    
    private void Awake()
    {
        disappearing = true;
    }

    protected override void ProjectileShotInit(Enemy targetEnemy, TurretBuilding owner)
    {
        base.ProjectileShotInit(targetEnemy, owner);
        
        _damageAttack = CreateDamageAttack(targetEnemy);
        SharedInitEnd(targetEnemy);
    }

    protected sealed override void ProjectileShotInit_PrecomputedAndQueued(TurretBuilding owner,
        TurretDamageAttack precomputedDamageAttack)
    {
        base.ProjectileShotInit_PrecomputedAndQueued(owner, precomputedDamageAttack);
        
        _damageAttack = precomputedDamageAttack;
        SharedInitEnd(precomputedDamageAttack.Target);
    }

    private void SharedInitEnd(Enemy targetEnemy)
    {
        _timeSinceSpawn = 0.001f;
        _startPosition = Position;
        _orbitOriginPosition = _spawnerObjectPosition + (Vector3.up * 0.5f);
        disappearing = false;
        
        _sphereHolder.SetActive(true);
        _trailRenderer.emitting = true;

        TurretOwner.OnDestroyed += DisappearStart;
        
        OnShotInitialized();
    }
    

    private void EnemyHit()
    {
        GameObject temp = ProjectileParticleFactory.GetInstance()
            .CreateParticlesGameObject(HitParticlesType, _targetEnemy.MeshTransform.position, Quaternion.identity);
        temp.transform.parent = gameObject.transform.parent;

        DamageTargetEnemy(_damageAttack);
        DisappearStart();
    }

    private void DisappearStart()
    {
        if (!gameObject.activeInHierarchy) return;
        
        TurretOwner.OnDestroyed -= DisappearStart;
        
        _sphereHolder.SetActive(false);
        _trailRenderer.emitting = false;
        Disappear();
    }
    
    protected override int ComputeDamage()
    {
        return Mathf.CeilToInt(TurretOwner.Stats.Damage * TOTAL_DAMAGE_MULTIPLIER);
    }

    protected override ITurretProjectileView MakeTurretProjectileView()
    {
        return new TurretSingleProjectileView(_viewAddOnsParent);
    }

    public override bool QueuesDamageToEnemies()
    {
        return false;
    }

    private void OnEnemyHit(Enemy enemy)
    {
        _targetEnemy = enemy;
        _damageAttack.UpdateTarget(_targetEnemy);
        EnemyHit();
    }


    private Vector3 _startPosition;
    private Vector3 _orbitOriginPosition;
    private float MaximumRadius => TurretOwner.Stats.RadiusRange + 0.5f;
    private float MinimumOrbitRadius => 0.5f;
    private float OrbitRadiusIncrementPerSecond => 1f;
    private float _timeSinceSpawn;
    
    
    protected override void DoUpdate()
    {
        UpdatePosition();
        _timeSinceSpawn += GameTime.DeltaTime;
    }
    
    private void UpdatePosition()
    {
        float orbitRadius = Mathf.Min(MaximumRadius, MinimumOrbitRadius + _timeSinceSpawn * OrbitRadiusIncrementPerSecond);
        
        float angle = _timeSinceSpawn * MovementSpeed * Mathf.Deg2Rad;

        float sin = Mathf.Sin(angle);
        float cos = Mathf.Cos(angle);
        
        Vector3 circularPosition = new Vector3(sin, 0, cos);
        Vector3 circularTangent = new Vector3(cos, 0, -sin);

        Vector3 orbitPosition = _orbitOriginPosition + (circularPosition * orbitRadius);
        orbitPosition = Vector3.Lerp(_startPosition, orbitPosition, _timeSinceSpawn);
        
        Quaternion orbitRotation = Quaternion.LookRotation(circularTangent, Vector3.up);
        
        _rigidbody.Move(orbitPosition, orbitRotation);
    }
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (CheckEnemyOnTriggerEnter(other, out Enemy enemy))
        {
            OnEnemyHit(enemy);
        }
    }

    
}