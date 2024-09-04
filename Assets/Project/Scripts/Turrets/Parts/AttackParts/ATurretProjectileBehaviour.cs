using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class ATurretProjectileBehaviour : MonoBehaviour
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
    
    public interface IDisappearListener
    {
        void OnProjectileDisappeared(ATurretProjectileBehaviour projectileBehaviour);
    }
    

    public void InstantiatedInit(TurretPartProjectileDataModel dataModel)
    {
        _dataModel = dataModel;
        _turretProjectileView = MakeTurretProjectileView();
        _enemiesToIgnore = new List<Enemy>();
    }

    public void ProjectileShotInit(ITurretShootingLifetimeCycle shootingLifetimeCycle, 
        Enemy targetEnemy, TurretBuilding owner)
    {
        SharedInit(shootingLifetimeCycle, targetEnemy);
        ProjectileShotInit(targetEnemy, owner);
    }
    protected virtual void ProjectileShotInit(Enemy targetEnemy, TurretBuilding owner)
    {
        TurretOwner = owner;
        _turretProjectileView.OnProjectileSpawned();
    }

    public void ProjectileShotInit_PrecomputedAndQueued(ITurretShootingLifetimeCycle shootingLifetimeCycle, 
        TurretBuilding owner, TurretDamageAttack precomputedDamageAttack)
    {
        SharedInit(shootingLifetimeCycle, precomputedDamageAttack.Target);
        ProjectileShotInit_PrecomputedAndQueued(owner, precomputedDamageAttack);
    }
    protected virtual void ProjectileShotInit_PrecomputedAndQueued(TurretBuilding owner, 
        TurretDamageAttack precomputedDamageAttack)
    {
        TurretOwner = owner;
    }

    private void SharedInit(ITurretShootingLifetimeCycle shootingLifetimeCycle, Enemy targetEnemy)
    {
        transform.rotation = Quaternion.LookRotation(
            Vector3.ProjectOnPlane(targetEnemy.Position - Position, Vector3.up).normalized);
        _shootingLifetimeCycle = shootingLifetimeCycle;
        _shootingLifetimeCycle.OnBeforeShootingEnemy(this);
        
        _enemiesToIgnore.Clear();
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
        _turretProjectileView.OnProjectileDisappear();
        DisappearListener?.OnProjectileDisappeared(this);
    }

    private IEnumerator WaitToDisable()
    {
        disappearing = true;

        yield return new WaitForSeconds(0.5f);
        Disable();
    }

    public void Disable()
    {
        gameObject.SetActive(false);
        disappearing = false;
    }


    protected Enemy[] GetNearestEnemiesToTargetedEnemy(Enemy targetedEnemy, int maxEnemies, float radius, LayerMask enemyLayerMask)
    {
        Collider[] colliders = Physics.OverlapSphere(targetedEnemy.Position, radius, enemyLayerMask, QueryTriggerInteraction.Collide);


        List<Enemy> enemies = new List<Enemy>(colliders.Length);

        for (int collidersI = 0; collidersI < colliders.Length; ++collidersI)
        {
            Enemy enemy = colliders[collidersI].gameObject.GetComponent<Enemy>();

            if (enemy != targetedEnemy && 
                enemy.CanBeAttackedByMultiCastProjectiles() &&
                !enemy.DiesFromQueuedDamage())
            {
                enemies.Add(enemy);
            }
        }

        if (enemies.Count == 0) return enemies.ToArray();

        enemies.Sort(SortByClosestToProjectile);

        Enemy[] nearestEnemies = new Enemy[Mathf.Min(maxEnemies, enemies.Count)];
        for (int i = 0; i < nearestEnemies.Length; ++i)
        {
            nearestEnemies[i] = enemies[i];
        }

        return nearestEnemies;
    }


    private int SortByClosestToProjectile(Enemy e1, Enemy e2)
    {
        return Vector3.Distance(e1.Position, Position).CompareTo(Vector3.Distance(e2.Position, Position));
    }


    protected TurretDamageAttack CreateDamageAttack(Enemy targetEnemy)
    { 
        TurretDamageAttack damageAttack = new TurretDamageAttack(this, targetEnemy, ComputeDamage());

        if (QueuesDamageToEnemies())
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

    protected abstract int ComputeDamage();

    protected abstract ITurretProjectileView MakeTurretProjectileView();



    private List<Enemy> _enemiesToIgnore;
    public void AddEnemyToIgnore(Enemy enemy)
    {
        _enemiesToIgnore.Add(enemy);
    }
    
    protected bool CheckEnemyOnTriggerEnter(Collider other, out Enemy enemy)
    {
        if(!other.gameObject.CompareTag("Enemy") || disappearing)
        {
            enemy = null;
            return false;
        }

        enemy = other.gameObject.GetComponent<Enemy>();

        if (_enemiesToIgnore.Contains(enemy))
        {
            enemy = null;
            return false;
        }
        
        return true;
    }
    
}
