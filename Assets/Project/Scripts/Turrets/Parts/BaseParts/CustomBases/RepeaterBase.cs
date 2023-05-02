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

    [Header("REPEATER UPGRADES")]
    [SerializeField] private float[] damagePer1Increments;
    private float currentDamagePer1Increment;

    [Header("ADDITIONAL MESHES")]
    [SerializeField] private MeshRenderer[] extraMeshes;
    private List<Material>[] extraMeshesDefaultMaterials;
    private List<Material>[] extraMeshesPreviewMaterials;

    [Header("REPEAT PARTICLES")]
    [SerializeField] private ParticleSystem repeatParticles;

    [Header("REPEAT TURRET BINDER")]
    [SerializeField] private Transform bindOriginTransform;
    [SerializeField] private MeshRenderer[] turretBinderMeshes;
    [SerializeField] private Material withinRangeMaterial;
    [SerializeField] private Material outsideRangeMaterial;


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
        HideAllTurretBinders();
    }

    private void OnEnable()
    {
        fakeEnemy.OnDamageCompute += FindTargetAndComputeDamage;        
        fakeEnemy.OnAttackedByProjectile += RepeatProjectile;

        BuildingPlacer.OnPlacingBuildingsDisabled += HideTurretBinder;
        BuildingPlacer.OnPreviewTurretBuildingHoversTile += ConnectFirstBinderWithBuilding;
    }

    private void OnDisable()
    {
        fakeEnemy.OnDamageCompute -= FindTargetAndComputeDamage;
        fakeEnemy.OnAttackedByProjectile -= RepeatProjectile;

        BuildingPlacer.OnPlacingBuildingsDisabled -= HideTurretBinder;
        BuildingPlacer.OnPreviewTurretBuildingHoversTile -= ConnectFirstBinderWithBuilding;
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
        HideAllTurretBinders();
    }

    public override void GotEnabledPlacing()
    {
        ConnectWithAlreadyPlacedBuildings();
    }
    public override void GotDisabledPlacing()
    {
        HideAllTurretBinders();
    }
    public override void GotMovedWhenPlacing()
    {
        ConnectWithAlreadyPlacedBuildings();
    }

    public override void Upgrade(int newStatLevel)
    {
        base.Upgrade(newStatLevel);
        currentLvl = newStatLevel;

        currentDamagePer1Increment = damagePer1Increments[currentLvl - 1];
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
        resultDamage += (int)(resultDamage * currentDamagePer1Increment);

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

        repeatParticles.Play();
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

        rotateTransform.rotation = Quaternion.Euler(0f, endEuler.y, 0f);
    }






    private void HideTurretBinder()
    {
        turretBinderMeshes[0].gameObject.SetActive(false);
    }
    private void ShowTurretBinder(MeshRenderer binderMesh)
    {
        binderMesh.gameObject.SetActive(true);
    }

    private void HideAllTurretBinders()
    {
        foreach (MeshRenderer turretBinderMesh in turretBinderMeshes)
        {
            turretBinderMesh.gameObject.SetActive(false);
        }        
    }


    private void ConnectWithAlreadyPlacedBuildings() // TODO Call this when trying to play
    {
        Building[] currentPlacedBuildings = BuildingPlacer.GetCurrentPlacedBuildings();

        for (int i = 0; i < currentPlacedBuildings.Length && i < turretBinderMeshes.Length; ++i)
        {
            ConnectBinderWithBuilding(currentPlacedBuildings[i], turretBinderMeshes[i]);
        }
    }

    private void ConnectFirstBinderWithBuilding(Building building)
    {
        ConnectBinderWithBuilding(building, turretBinderMeshes[0]);
    }

    private void ConnectBinderWithBuilding(Building building, MeshRenderer binderMesh)
    {
        if (building.CardBuildingType == BuildingCard.CardBuildingType.TURRET)
        {
            ShowTurretBinder(binderMesh);
            ConnectBinderWithTurretBuilding(binderMesh, building as TurretBuilding);
        }        
    }

    private void ConnectBinderWithTurretBuilding(MeshRenderer binderMesh, TurretBuilding turretBuilding)
    {
        UpdateTurretBinder(binderMesh.transform, turretBuilding.BodyPartTransform);

        float turretRange = ((turretBuilding.stats.range + 0.5f) / 2f) + 0.2f; // Weird formula...
        if (IsBinderTargetWithinRange(binderMesh.transform, turretBuilding.BodyPartTransform, turretRange))
        {
            binderMesh.material = withinRangeMaterial;
        }
        else
        {
            binderMesh.material = outsideRangeMaterial;
        }
    }
        


    private void UpdateTurretBinder(Transform binderTransform, Transform targetTransform)
    {       
        // Find scale
        float distance = Vector3.Distance(bindOriginTransform.position, targetTransform.position);
        Vector3 binderScale = binderTransform.lossyScale;
        float scaleFactor = 1f;
        binderScale.z = distance * scaleFactor;


        // Find center position
        Vector3 centerPosition = Vector3.Lerp(bindOriginTransform.position, targetTransform.position, 0.5f);


        // Find rotation
        Vector3 directionToTarget = (targetTransform.position - bindOriginTransform.position).normalized;
        Quaternion binderRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);


        // Apply changes
        binderTransform.localScale = binderScale;
        binderTransform.position = centerPosition;
        binderTransform.rotation = binderRotation;
    }

    private bool IsBinderTargetWithinRange(Transform binderTransform, Transform targetTransform, float range)
    {       
        Vector3 binderPosition = binderTransform.position;
        binderPosition.y = 0f;

        Vector3 targetPosition = targetTransform.position;
        targetPosition.y = 0f;

        float distance = Vector3.Distance(binderPosition, targetPosition);

        
        //Debug.Log("D-" + distance + " <= R-" + range);

        return distance <= range;
    }
}
