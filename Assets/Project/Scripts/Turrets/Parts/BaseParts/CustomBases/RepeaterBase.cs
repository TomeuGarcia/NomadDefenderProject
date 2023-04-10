using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static SlowBase;
using static TurretPartBody;

public class RepeaterBase : TurretPartBase_Prefab
{
    [Header("REPEATER")]
    [SerializeField] private FakeEnemy fakeEnemy;
    [SerializeField] private MeshRenderer repeatAreaPlane;
    private Material repeatAreaPlaneMaterial;
    [SerializeField] private Transform shootPoint;

    private List<Enemy> repeatTargetEnemies = new List<Enemy>();
    private Enemy targetedEnemy;

    private int currentLvl = 0;

    private struct EnemyInDamageQueue
    {
        public EnemyInDamageQueue(TurretPartAttack_Prefab projectile, Enemy enemy, int damage)
        {
            this.projectile = projectile;
            this.enemy = enemy;
            this.damage = damage;
        }

        public TurretPartAttack_Prefab projectile;
        public Enemy enemy;
        public int damage;
    }
    private Queue<EnemyInDamageQueue> enemyDamageQueue = new Queue<EnemyInDamageQueue>();


    private void Awake()
    {
        fakeEnemy.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        fakeEnemy.OnAttackedByProjectile += BounceProjectile;
    }

    private void OnDisable()
    {
        fakeEnemy.OnAttackedByProjectile -= BounceProjectile;
    }



    override public void Init(TurretBuilding turretOwner, float turretRange)
    {
        base.Init(turretOwner, turretRange);

        turretOwner.OnEnemyEnterRange += AddEnemyToRepeatTargets;
        turretOwner.OnEnemyExitRange += RemoveEnemyFromRepeatTargets;

        repeatAreaPlane.transform.localScale = Vector3.one * ((float)turretRange / 10.0f);
    }
    override public void InitAsSupportBuilding(SupportBuilding supportOwner, float supportRange)
    {
        base.InitAsSupportBuilding(supportOwner, supportRange);

        supportOwner.OnEnemyEnterRange += AddEnemyToRepeatTargets;
        supportOwner.OnEnemyExitRange += RemoveEnemyFromRepeatTargets;

        float planeRange = supportOwner.stats.range * 2 + 1; //only for square
        float range = supportOwner.stats.range;

        repeatAreaPlane.transform.localScale = Vector3.one * ((float)planeRange / 10.0f);
        repeatAreaPlaneMaterial = repeatAreaPlane.materials[0];
        repeatAreaPlaneMaterial.SetFloat("_TileNum", planeRange);
    }

    public override void OnGetPlaced()
    {
        fakeEnemy.gameObject.SetActive(true);
        fakeEnemy.SetCanBeTargeted(false);
    }

    public override void Upgrade(int newStatLevel)
    {
        base.Upgrade(newStatLevel);
        currentLvl = newStatLevel;

        //foreach (KeyValuePair<Enemy, SlowData> slowedEnemy in slowedEnemies)
        //{
        //    if (slowedEnemy.Value.slowCoefApplied > slowSpeedCoefs[currentLvl])
        //    {
        //        slowedEnemy.Key.SetMoveSpeed(slowSpeedCoefs[currentLvl]);
        //        slowedEnemy.Value.slowCoefApplied = slowSpeedCoefs[currentLvl];
        //    }
        //}
    }


    private void AddEnemyToRepeatTargets(Enemy enemy)
    {
        if (enemy == fakeEnemy) return;

        repeatTargetEnemies.Add(enemy);
        fakeEnemy.SetCanBeTargeted(true);
    }

    private void RemoveEnemyFromRepeatTargets(Enemy enemy)
    {
        repeatTargetEnemies.Remove(enemy);

        if (repeatTargetEnemies.Count == 0)
        {
            fakeEnemy.SetCanBeTargeted(false);
        }
    }

    private void BounceProjectile(TurretPartAttack_Prefab projectile)
    {
        ComputeNextTargetedEnemy();
        if (targetedEnemy == null) return;

        Shoot(targetedEnemy, projectile);
    }

    private void PrecomputeProjectileBounce(TurretPartAttack_Prefab projectile, int baseDamage, PassiveDamageModifier modifier, out int finalDamage)
    {
        ComputeNextTargetedEnemy();
        
        if (targetedEnemy != null)
        {
            finalDamage = targetedEnemy.QueueDamage(baseDamage, modifier);
            enemyDamageQueue.Enqueue(new EnemyInDamageQueue(projectile, targetedEnemy, finalDamage));
        }
        else
        {
            finalDamage = -1;
        }
        
    }

    private void ComputeNextTargetedEnemy()
    {
        targetedEnemy = null;

        int enemyI = 0;
        while (enemyI < repeatTargetEnemies.Count && (!repeatTargetEnemies[enemyI].CanBeTargeted() || repeatTargetEnemies[enemyI].DiesFromQueuedDamage()) )
        {
            ++enemyI;
        }

        if (enemyI < repeatTargetEnemies.Count)
        {
            targetedEnemy = repeatTargetEnemies[enemyI];
        }
    }

    private void Shoot(Enemy enemyTarget, TurretPartAttack_Prefab projectile)
    {
        TurretPartAttack_Prefab.AttackType attackType = projectile.GetAttackType;
        Vector3 spawnPosition = shootPoint.position;

        /*
        TurretPartAttack_Prefab currentAttack = projectile;
        */
        TurretPartAttack_Prefab currentAttack = ProjectileAttacksFactory.GetInstance().GetAttackGameObject(attackType, spawnPosition, Quaternion.identity)
            .GetComponent<TurretPartAttack_Prefab>();
        

        currentAttack.transform.parent = projectile.GetTurretOwner().BodyPartTransform;
        currentAttack.gameObject.SetActive(true);
        currentAttack.ProjectileShotInit(enemyTarget, projectile.GetTurretOwner());


        // Spawn particle
        GameObject particles = ProjectileParticleFactory.GetInstance().GetAttackParticlesGameObject(currentAttack.GetAttackType, spawnPosition, Quaternion.identity);
        particles.SetActive(true);
        particles.transform.parent = gameObject.transform.parent;


        // Audio
        GameAudioManager.GetInstance().PlayProjectileShot(BodyType.SENTRY);        
    }


}
