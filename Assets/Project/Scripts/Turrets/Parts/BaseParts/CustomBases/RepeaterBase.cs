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
    [SerializeField] private Transform rotateTransform;

    [Header("ADDITIONAL MESHES")]
    [SerializeField] private MeshRenderer[] extraMeshes;
    private List<Material>[] extraMeshesDefaultMaterials;
    private List<Material>[] extraMeshesPreviewMaterials;

    private List<Enemy> repeatTargetEnemies = new List<Enemy>();
    private Enemy targetedEnemy;

    bool allEnemiesInRangeAreFake = false;

    private int currentLvl = 0;

    private class EnemyInDamageQueue
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
    private List<EnemyInDamageQueue> enemiesInDamageQueue = new List<EnemyInDamageQueue>();


    private void Awake()
    {
        fakeEnemy.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        fakeEnemy.OnDamageCompute += FindTargetAndComputeDamage;
        
        fakeEnemy.OnAttackedByProjectile += RepeatProjectile;
    }

    private void OnDisable()
    {
        fakeEnemy.OnDamageCompute -= FindTargetAndComputeDamage;

        fakeEnemy.OnAttackedByProjectile -= RepeatProjectile;
    }

    private void Update()
    {
        if (repeatTargetEnemies.Count > 0)
        {
            LookAtTargetEnemy();
        }
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



    protected override void InitMaterials()
    {
        base.InitMaterials();

        extraMeshesDefaultMaterials = new List<Material>[extraMeshes.Length];
        extraMeshesPreviewMaterials = new List<Material>[extraMeshes.Length];

        for (int extraMeshI = 0; extraMeshI < extraMeshes.Length; ++extraMeshI) 
        {
            extraMeshesDefaultMaterials[extraMeshI] = new List<Material>();
            extraMeshesPreviewMaterials[extraMeshI] = new List<Material>();

            for (int materialI = 0; materialI < extraMeshes[extraMeshI].materials.Length; ++materialI)
            {
                extraMeshesDefaultMaterials[extraMeshI].Add(extraMeshes[extraMeshI].materials[materialI]);
                extraMeshesPreviewMaterials[extraMeshI].Add(previewMaterial);
            }

        }
    }
    public override void SetDefaultMaterial()
    {
        base.SetDefaultMaterial();

        for (int i = 0; i< extraMeshes.Length; ++i)
        {
            extraMeshes[i].materials = extraMeshesDefaultMaterials[i].ToArray();
        }
    }

    public override void SetPreviewMaterial()
    {
        base.SetPreviewMaterial();
        
        for (int i = 0; i < extraMeshes.Length; ++i)
        {
            extraMeshes[i].materials = extraMeshesPreviewMaterials[i].ToArray();
        }
    }



    private void AddEnemyToRepeatTargets(Enemy enemy)
    {
        if (enemy.IsFakeEnemy) return;

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


    private void FindTargetAndComputeDamage(int damageAmount, PassiveDamageModifier modifier, out int resultDamage, TurretPartAttack_Prefab projectileSource)
    {
        ComputeNextTargetedEnemy();

        if (targetedEnemy == null)
        {
            resultDamage = 0;
            return;
        }

        resultDamage = targetedEnemy.ComputeDamageWithPassive(projectileSource, damageAmount, modifier);
        targetedEnemy.QueueDamage(resultDamage);

        enemiesInDamageQueue.Add(new EnemyInDamageQueue(projectileSource, targetedEnemy, resultDamage));
    }


    private EnemyInDamageQueue PopEnemyInDamageQueue(TurretPartAttack_Prefab projectileSource)
    {
        for (int i = 0; i < enemiesInDamageQueue.Count; ++i)
        {
            if (enemiesInDamageQueue[i].projectile == projectileSource)
            {
                EnemyInDamageQueue temp = enemiesInDamageQueue[i];
                enemiesInDamageQueue.RemoveAt(i);
                return temp;
            }
        }

        return null;
    }



    private void RepeatProjectile(TurretPartAttack_Prefab projectileSource)
    {        
        EnemyInDamageQueue enemyInDamageQueue = PopEnemyInDamageQueue(projectileSource);

        if (enemyInDamageQueue == null) return;

        Shoot(enemyInDamageQueue.enemy, enemyInDamageQueue.projectile, enemyInDamageQueue.damage);
    }

    private void ComputeNextTargetedEnemy()
    {
        targetedEnemy = null;

        for (int i = 0; i < repeatTargetEnemies.Count; ++i)
        {
            if (repeatTargetEnemies[i].CanBeTargeted() && !repeatTargetEnemies[i].DiesFromQueuedDamage())
            {
                targetedEnemy = repeatTargetEnemies[i];
                break;
            }
        }
    }


    private void Shoot(Enemy enemyTarget, TurretPartAttack_Prefab projectileSource, int precomputedDamage)
    {
        TurretPartAttack_Prefab.AttackType attackType = projectileSource.GetAttackType;
        Vector3 spawnPosition = shootPoint.position;

        TurretPartAttack_Prefab newProjectile = ProjectileAttacksFactory.GetInstance().GetAttackGameObject(attackType, spawnPosition, Quaternion.identity)
            .GetComponent<TurretPartAttack_Prefab>();


        newProjectile.transform.parent = projectileSource.GetTurretOwner().BaseHolder;
        newProjectile.gameObject.SetActive(true);
        newProjectile.ProjectileShotInit_PrecomputedAndQueued(enemyTarget, projectileSource.GetTurretOwner(), precomputedDamage);


        // Spawn particle
        GameObject particles = ProjectileParticleFactory.GetInstance().GetAttackParticlesGameObject(newProjectile.GetAttackType, spawnPosition, Quaternion.identity);
        particles.SetActive(true);
        particles.transform.parent = gameObject.transform.parent;


        // Audio
        GameAudioManager.GetInstance().PlayProjectileShot(BodyType.SENTRY);        
    }

    private void LookAtTargetEnemy()
    {
        Vector3 lookPosition = targetedEnemy != null ? targetedEnemy.Position : repeatTargetEnemies[0].Position;    

        Quaternion targetRot = Quaternion.LookRotation((lookPosition - rotateTransform.position).normalized, rotateTransform.up);

        Quaternion endRotation = Quaternion.RotateTowards(rotateTransform.rotation, targetRot, 600.0f * Time.deltaTime * GameTime.TimeScale);
        Vector3 endEuler = endRotation.eulerAngles;

        rotateTransform.rotation = Quaternion.Euler(0f, endEuler.y, endEuler.z);
    }
}
