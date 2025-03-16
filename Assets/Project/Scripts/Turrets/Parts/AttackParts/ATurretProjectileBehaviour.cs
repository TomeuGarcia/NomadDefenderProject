using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.ObjectPooling;
using Unity.Content;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class ATurretProjectileBehaviour : RecyclableObject
{
    public enum Type { 
        Homing, 
        HomingChaining,
        Piercing,
        Shotgun,
        Orbiting,
    }


    [Header("VIEW")] 
    [SerializeField] protected Transform _viewAddOnsParent;
    private ITurretProjectileView _turretProjectileView;
    public ITurretProjectileViewAddOnController ViewAddOnsController => _turretProjectileView;
    
    
    protected float MovementSpeed => _dataModel.MovementSpeed;
    protected float _damageMultiplier;


    private TurretPartProjectileDataModel _dataModel;
    protected TurretDamageAttack _damageAttack;
    public TurretDamageAttack DamageAttack => _damageAttack;
    protected Enemy _targetEnemy;
    private ITurretShootingLifetimeCycle _shootingLifetimeCycle;
    protected bool disappearing = false;
    
    public Vector3 Position => transform.position;
    public Quaternion Rotation => transform.rotation;
    public TurretBuilding TurretOwner { get; private set; }
    public Type ProjectileType => _dataModel.ProjectileType;
    public ProjectileParticleType HitParticlesType => _dataModel.HitParticlesType;

    public IDisappearListener DisappearListener { get; set; } = null;

    protected Vector3 _spawnerObjectPosition;
    
    public interface IDisappearListener
    {
        void OnProjectileDisappeared(ATurretProjectileBehaviour projectileBehaviour);
    }
    
    
    
    internal override void RecycledInit() { }

    internal override void RecycledReleased()
    {
        _turretProjectileView.OnProjectileDisappear();
        DisappearListener?.OnProjectileDisappeared(this);
    }
    

    public void InstantiatedInit(TurretPartProjectileDataModel dataModel)
    {
        _dataModel = dataModel;
        _turretProjectileView = MakeTurretProjectileView();
        _enemiesToIgnore = new HashSet<Enemy>();
        InitDamageMultiplier();
    }

    public void ProjectileShotInit(ITurretShootingLifetimeCycle shootingLifetimeCycle, 
        Enemy targetEnemy, TurretBuilding owner)
    {
        SharedInit(shootingLifetimeCycle, targetEnemy, owner.Position);
        ProjectileShotInit(targetEnemy, owner);
    }
    protected virtual void ProjectileShotInit(Enemy targetEnemy, TurretBuilding owner)
    {
        TurretOwner = owner;
        _turretProjectileView.OnProjectileSpawned();
    }

    public void ProjectileShotInit_PrecomputedAndQueued(ITurretShootingLifetimeCycle shootingLifetimeCycle, 
        TurretBuilding owner, TurretDamageAttack precomputedDamageAttack, Vector3 spawnerObjectPosition)
    {
        SharedInit(shootingLifetimeCycle, precomputedDamageAttack.Target, spawnerObjectPosition);
        ProjectileShotInit_PrecomputedAndQueued(owner, precomputedDamageAttack);
    }
    protected virtual void ProjectileShotInit_PrecomputedAndQueued(TurretBuilding owner, 
        TurretDamageAttack precomputedDamageAttack)
    {
        TurretOwner = owner;
        _turretProjectileView.OnProjectileSpawned();
    }

    private void SharedInit(ITurretShootingLifetimeCycle shootingLifetimeCycle, Enemy targetEnemy, Vector3 spawnerObjectPosition)
    {
        transform.rotation = Quaternion.LookRotation(
            Vector3.ProjectOnPlane(targetEnemy.Position - Position, Vector3.up).normalized);
        _shootingLifetimeCycle = shootingLifetimeCycle;
        _shootingLifetimeCycle.OnBeforeShootingEnemy(this);
        
        _enemiesToIgnore.Clear();
        _spawnerObjectPosition = spawnerObjectPosition;
    }
    
    
    
    protected virtual void DoUpdate()
    {
    }


    public virtual bool QueuesDamageToEnemies()
    {
        return true;
    }

    void Update()
    {
        if (!disappearing)
        {
            DoUpdate();
        }
    }

    protected void Disappear()
    {
        StartCoroutine(WaitToDisable());
    }

    private IEnumerator WaitToDisable()
    {
        disappearing = true;

        yield return new WaitForSeconds(0.5f);
        Disable();
    }

    public void Disable()
    {
        Recycle();
        disappearing = false;
    }


    protected Enemy[] GetNearestEnemiesToTargetedEnemy(Enemy targetedEnemy, int maxEnemies, float radius, LayerMask enemyLayerMask)
    {
        Collider[] colliders = Physics.OverlapSphere(targetedEnemy.Position, radius, enemyLayerMask, QueryTriggerInteraction.Collide);


        List<Enemy> enemies = new List<Enemy>(colliders.Length);

        for (int collidersI = 0; collidersI < colliders.Length; ++collidersI)
        {
            if (!colliders[collidersI].gameObject.TryGetComponent<Enemy>(out Enemy enemy))
            {
                continue;
            }

            if(targetedEnemy == null)
            {
                Debug.Log("targetedEnemy IS NULL");
            }

            if (enemy != targetedEnemy && 
                enemy.CanBeAttackedByMultiCastProjectiles() &&
                !enemy.DiesFromQueuedDamage())
            {
                enemies.Add(enemy);
            }
        }

        if (enemies.Count > 1)
        {
            enemies.Sort(SortByClosestToProjectile);
        }

        return enemies.ToArray();
    }


    private int SortByClosestToProjectile(Enemy e1, Enemy e2)
    {
        return Vector3.Distance(e1.Position, Position).CompareTo(Vector3.Distance(e2.Position, Position));
    }


    protected TurretDamageAttack CreateDamageAttack(Enemy targetEnemy)
    {
        bool isQueuedDamage = QueuesDamageToEnemies();
        TurretDamageAttack damageAttack = new TurretDamageAttack(this, targetEnemy, ComputeDamage(), isQueuedDamage);

        if (isQueuedDamage)
        {
            _shootingLifetimeCycle.OnBeforeDamagingEnemy(damageAttack);
        }

        return damageAttack;
    }
    
    protected void DamageTargetEnemy(TurretDamageAttack damageAttack)
    {
        if (!QueuesDamageToEnemies())
        {
            _shootingLifetimeCycle.OnBeforeDamagingEnemy(damageAttack);
        }

        _targetEnemy.TakeDamage(damageAttack, DamageTargetEnemyResult);
    }

    private void DamageTargetEnemyResult(TurretDamageAttackResult damageAttackResult)
    {
        _shootingLifetimeCycle.OnAfterDamagingEnemy(damageAttackResult);
        _turretProjectileView.OnProjectileHitsTarget(damageAttackResult.Target.MeshTransform);
    }
    
    protected virtual void OnShotInitialized()
    {
        _targetEnemy?.OnWillBeAttacked(_damageAttack);
    }

    private void InitDamageMultiplier()
    {
        _damageMultiplier = _dataModel.DamageMultiplier;
    }

    protected abstract int ComputeDamage();

    protected abstract ITurretProjectileView MakeTurretProjectileView();



    private HashSet<Enemy> _enemiesToIgnore;
    public IReadOnlyCollection<Enemy> EnemiesToIgnore => _enemiesToIgnore;
    
    public void AddEnemyToIgnore(Enemy enemy)
    {
        _enemiesToIgnore.Add(enemy);
    }
    
    protected bool CheckEnemyOnTriggerEnter(Collider other, out Enemy enemy)
    {
        if(!isActiveAndEnabled || disappearing || !other.gameObject.CompareTag("Enemy"))
        {
            enemy = null;
            return false;
        }

        enemy = other.gameObject.GetComponent<Enemy>();

        return CheckEnemy(enemy);
    }
    protected bool CheckEnemy(Enemy enemy)
    {
        return !_enemiesToIgnore.Contains(enemy) && !enemy.IsDead();
    }


    protected List<Enemy> ComputeIntersectingEnemies(Vector3 origin, Vector3 direction, float distance, float enemyRadius = 0.25f)
    {
        origin.y = 0;
        
        List<Enemy> intersectingEnemies = new List<Enemy>(10);
        IReadOnlyCollection<Enemy> activeEnemies = EnemyFactory.GetInstance().GetActiveEnemies();

        foreach (Enemy activeEnemy in activeEnemies)
        {
            Vector3 enemyPosition = activeEnemy.Position;
            enemyPosition.y = 0;
            if (Vector3.Distance(origin, enemyPosition) > distance)
            {
                continue;
            }
            
            float t = Vector3.Dot(enemyPosition - origin, direction);
            Vector3 p = origin + direction * t;
            float y = Vector3.Distance(enemyPosition, p);

            bool hit = y < enemyRadius;
            if (hit)
            {
                intersectingEnemies.Add(activeEnemy);
            }
        }

        return intersectingEnemies;
    }
    protected Enemy ComputeClosestIntersectingEnemy(Vector3 origin, Vector3 direction, float distance, float enemyRadius = 0.25f)
    {
        List<Enemy> intersectingEnemies = ComputeIntersectingEnemies(origin, direction, distance, enemyRadius);

        Enemy closestIntersectingEnemy = null;
        float closestIntersectingEnemyDistance = float.MaxValue;

        foreach (Enemy intersectingEnemy  in intersectingEnemies)
        {
            float distanceToEnemy = Vector3.Distance(origin, intersectingEnemy.Position);
            if (distanceToEnemy < closestIntersectingEnemyDistance)
            {
                closestIntersectingEnemy = intersectingEnemy;
                closestIntersectingEnemyDistance = distanceToEnemy;
            }
        }
        
        return closestIntersectingEnemy;
    }

    

}
