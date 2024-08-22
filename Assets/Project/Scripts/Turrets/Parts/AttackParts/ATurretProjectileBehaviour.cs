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
    }
    
    public TurretBuilding TurretOwner { get; private set; }


    private TurretPartProjectileDataModel _dataModel;
    protected TurretDamageAttack _damageAttack;
    
    
    protected Enemy targetEnemy;
    protected int damage;
    public Type ProjectileType => _dataModel.ProjectileType;
    
    [SerializeField] protected float bulletSpeed = 2.0f;

    [SerializeField] public Material materialForTurret;

    public Vector3 Position => transform.position;


    
    protected bool disappearing = false;

    private ITurretShootingLifetimeCycle _shootingLifetimeCycle;


    public void InstantiatedInit(TurretPartProjectileDataModel dataModel)
    {
        _dataModel = dataModel;
    }

    public void ProjectileShotInit(ITurretShootingLifetimeCycle shootingLifetimeCycle, 
        Enemy targetEnemy, TurretBuilding owner)
    {
        ProjectileShotInit(targetEnemy, owner);
        
        _shootingLifetimeCycle = shootingLifetimeCycle;
        _shootingLifetimeCycle.OnBeforeShootingEnemy();
    }
    protected virtual void ProjectileShotInit(Enemy targetEnemy, TurretBuilding owner)
    {
        TurretOwner = owner;
    }

    public void ProjectileShotInit_PrecomputedAndQueued(ITurretShootingLifetimeCycle shootingLifetimeCycle, 
        TurretBuilding owner, TurretDamageAttack precomputedDamageAttack)
    {
        ProjectileShotInit_PrecomputedAndQueued(owner, precomputedDamageAttack);
        
        _shootingLifetimeCycle = shootingLifetimeCycle;
        _shootingLifetimeCycle.OnBeforeShootingEnemy();
    }
    protected virtual void ProjectileShotInit_PrecomputedAndQueued(TurretBuilding owner, 
        TurretDamageAttack precomputedDamageAttack)
    {
        TurretOwner = owner;
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

    protected virtual void Disappear()
    {
        StartCoroutine(WaitToDisable());
    }

    private IEnumerator WaitToDisable()
    {
        disappearing = true;

        yield return new WaitForSeconds(0.5f);
        Disable();
    }

    protected void Disable()
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


    protected void DamageTargetEnemy(TurretDamageAttack damageAttack)
    {
        _shootingLifetimeCycle.OnBeforeDamagingEnemy(_damageAttack);
        TurretDamageAttackResult damageAttackResult = targetEnemy.TakeDamage(damageAttack);
        _shootingLifetimeCycle.OnAfterDamagingEnemy(damageAttackResult);
    }
    
    protected virtual void OnShotInitialized()
    {
        targetEnemy.OnWillBeAttacked(_damageAttack);
    }
}
