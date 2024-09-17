using System;
using System.Collections.Generic;
using UnityEngine;
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
    [SerializeField] private Transform upgradeVisualsHolder;

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


    private Enemy targetedEnemy;

    bool allEnemiesInRangeAreFake = false;

    private int currentLvl = 0;

    private RangeBuilding _ownerBuilding;

    private class EnemyInDamageQueue
    {
        public EnemyInDamageQueue(TurretDamageAttack damageAttack)
        {
            DamageAttack = damageAttack;
        }

        public readonly TurretDamageAttack DamageAttack;
    }
    private List<EnemyInDamageQueue> enemiesInDamageQueue = new List<EnemyInDamageQueue>();


    private void Awake()
    {
        AwakeInit();
        fakeEnemy.gameObject.SetActive(false);
        fakeEnemy.SetupColliderBounds();

        HideAllTurretBinders();
        currentDamagePer1Increment = 0f;
        repeatAreaPlaneMaterial = repeatAreaPlane.materials[0];
    }

    private void OnEnable()
    {
        fakeEnemy.OnWillBeAttackedByTurret += ComputeTargetAndComputeDamage;        
        fakeEnemy.OnAttackedByProjectile += RepeatProjectile;
        fakeEnemy.OnGetPosition += ComputeTargetAndAssignFakeEnemyPosition;

        BuildingPlacer.OnPlacingBuildingsDisabled += HideFirstTurretBinder;
        BuildingPlacer.OnPreviewTurretBuildingHoversTile += ConnectFirstBinderWithBuilding;
    }

    private void OnDisable()
    {
        fakeEnemy.OnWillBeAttackedByTurret -= ComputeTargetAndComputeDamage;
        fakeEnemy.OnAttackedByProjectile -= RepeatProjectile;
        fakeEnemy.OnGetPosition -= ComputeTargetAndAssignFakeEnemyPosition;

        BuildingPlacer.OnPlacingBuildingsDisabled -= HideFirstTurretBinder;
        BuildingPlacer.OnPreviewTurretBuildingHoversTile -= ConnectFirstBinderWithBuilding;
    }

    private void Update()
    {
        ComputeNextTargetedEnemy();
        if (targetedEnemy != null)
        {
            LookAtTargetEnemy();
        }
    }


    override public void Init(TurretBuilding turretOwner, float turretRange)
    {
        base.Init(turretOwner, turretRange);

        _ownerBuilding = turretOwner;
        turretOwner.OnEnemyEnterRange += AddEnemyToRepeatTargets;
        turretOwner.OnEnemyExitRange += RemoveEnemyFromRepeatTargets;

        repeatAreaPlane.transform.localScale = Vector3.one * ((float)turretRange / 10.0f);
    }
    override public void InitAsSupportBuilding(SupportBuilding supportOwner, float supportRange)
    {
        base.InitAsSupportBuilding(supportOwner, supportRange);

        _ownerBuilding = supportOwner;
        supportOwner.OnEnemyEnterRange += AddEnemyToRepeatTargets;
        supportOwner.OnEnemyExitRange += RemoveEnemyFromRepeatTargets;

        UpdateAreaPlaneSize(supportOwner, repeatAreaPlane, repeatAreaPlaneMaterial);
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

    public override void GotHoveredWhenPlaced()
    {
        ConnectWithAlreadyPlacedBuildings();
    }
    public override void GotUnoveredWhenPlaced()
    {
        HideAllTurretBinders();
    }

    public override void Upgrade(SupportBuilding ownerSupportBuilding, int newStatLevel)
    {
        base.Upgrade(ownerSupportBuilding, newStatLevel);
        currentLvl = newStatLevel;

        if (newStatLevel == 3)
        {
            ownerSupportBuilding.UpgradeRangeIncrementingLevel();
            UpdateAreaPlaneSize(ownerSupportBuilding, repeatAreaPlane, repeatAreaPlaneMaterial);
        }

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
        if (enemy.IsFakeEnemy)
        {
            _ownerBuilding.Enemies.Remove(enemy);
        }
        else
        {
            fakeEnemy.SetCanBeTargeted(true);
        }
    }

    private void RemoveEnemyFromRepeatTargets(Enemy enemy)
    {
        if (_ownerBuilding.Enemies.Count <= 1)
        {
            fakeEnemy.SetCanBeTargeted(false);
        }        
    }


    private void ComputeTargetAndComputeDamage(TurretDamageAttack damageAttack)
    {
        if (targetedEnemy == null)
        {
            return;
        }

        int resultDamage = damageAttack.Damage + (int)(damageAttack.Damage * currentDamagePer1Increment);

        TurretDamageAttack repeatedDamageAttack = 
            new TurretDamageAttack(damageAttack.ProjectileSource, targetedEnemy, resultDamage);

        if (damageAttack.ProjectileSource.QueuesDamageToEnemies())
        {
            targetedEnemy.QueueDamage(repeatedDamageAttack);
        }        

        enemiesInDamageQueue.Add(new EnemyInDamageQueue(repeatedDamageAttack));
    }


    private EnemyInDamageQueue PopEnemyInDamageQueue(ATurretProjectileBehaviour projectileSource)
    {
        for (int i = 0; i < enemiesInDamageQueue.Count; ++i)
        {
            if (enemiesInDamageQueue[i].DamageAttack.ProjectileSource == projectileSource)
            {
                EnemyInDamageQueue temp = enemiesInDamageQueue[i];
                enemiesInDamageQueue.RemoveAt(i);
                return temp;
            }
        }

        return null;
    }



    private void RepeatProjectile(ATurretProjectileBehaviour projectileSource, 
        Action<TurretDamageAttackResult> takeDamageResultCallback)
    {        
        EnemyInDamageQueue enemyInDamageQueue = PopEnemyInDamageQueue(projectileSource);
        TurretDamageAttack damageAttackToRepeat = null;
        
        if (enemyInDamageQueue == null)
        {
            if (projectileSource.QueuesDamageToEnemies())
            {
                return;
            }

            damageAttackToRepeat = projectileSource.DamageAttack;
        }
        else
        {
            damageAttackToRepeat = enemyInDamageQueue.DamageAttack;
        }
        
        

        Shoot(damageAttackToRepeat);

        /*
        if (currentLvl > 0)
        {
            upgradeVisualsHolder.DOBlendableLocalRotateBy(180f * Vector3.up, 0.2f);
        }
        */

        repeatParticles.Play();
    }

    private void ComputeNextTargetedEnemy()
    {
        targetedEnemy = _ownerBuilding.GetBestEnemyTarget(targetedEnemy);
    }


    private void Shoot(TurretDamageAttack projectileToRepeat)
    {
        ATurretProjectileBehaviour projectileSource = projectileToRepeat.ProjectileSource;
        
        Vector3 spawnPosition = shootPoint.position;

        ATurretProjectileBehaviour newProjectile = ProjectileAttacksFactory.GetInstance()
            .Create(projectileSource.ProjectileType, spawnPosition, Quaternion.identity);

        TurretBuilding projectileTurret = projectileSource.TurretOwner;

        newProjectile.transform.parent = projectileSource.TurretOwner.BaseHolder;
        newProjectile.ProjectileShotInit_PrecomputedAndQueued(projectileTurret.CardData.PassiveAbilitiesController,
            projectileTurret, projectileToRepeat, _ownerBuilding.Position);
        newProjectile.AddEnemyToIgnore(fakeEnemy);


        // Spawn particle
        GameObject particles = ProjectileParticleFactory.GetInstance()
            .CreateParticlesGameObject(projectileSource.HitParticlesType, spawnPosition, Quaternion.identity);
        particles.transform.parent = gameObject.transform.parent;


        // Audio
        GameAudioManager.GetInstance().PlayProjectileShot(BodyType.SENTRY);
    }

    private void LookAtTargetEnemy()
    {
        Vector3 lookPosition = targetedEnemy != null ? targetedEnemy.Position : targetedEnemy.Position;

        Vector3 lookDirection = Vector3.ProjectOnPlane(lookPosition - rotateTransform.position, Vector3.up).normalized;

        if (Vector3.Dot(lookDirection, rotateTransform.forward) > 0.95f)
        {
            return;
        }

        Quaternion targetRot = Quaternion.LookRotation(lookDirection, rotateTransform.up);

        Quaternion endRotation = Quaternion.RotateTowards(rotateTransform.rotation, targetRot, 600.0f * Time.deltaTime * GameTime.TimeScale);
        Vector3 endEuler = endRotation.eulerAngles;

        rotateTransform.rotation = Quaternion.Euler(0f, endEuler.y, 0f);
    }


    private void ComputeTargetAndAssignFakeEnemyPosition(out Vector3 enemyPosition, out bool foundTarget)
    {
        //ComputeNextTargetedEnemy();

        foundTarget = targetedEnemy != null;
        enemyPosition = foundTarget
            ? targetedEnemy.GetPosition()
            : Vector3.zero;
    }




    private void HideFirstTurretBinder()
    {
        HideTurretBinder(turretBinderMeshes[0]);
    }
    private void HideTurretBinder(MeshRenderer binderMesh)
    {
        binderMesh.gameObject.SetActive(false);
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


    private void ConnectWithAlreadyPlacedBuildings() 
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
            TurretBuilding turretBuilding = building as TurretBuilding;
            bool isTurretWithinRange = turretBuilding.GetBasePart().IsBoundsWithinRange(fakeEnemy.GetColliderBounds());

            if (isTurretWithinRange)
            {
                ShowTurretBinder(binderMesh);
                ConnectBinderWithTurretBuilding(binderMesh, turretBuilding, isTurretWithinRange);
            }
            else
            {
                HideTurretBinder(binderMesh);
            }
        }        
    }

    private void ConnectBinderWithTurretBuilding(MeshRenderer binderMesh, TurretBuilding turretBuilding, bool isTurretWithinRange)
    {
        TurretBinderUtils.UpdateTurretBinder(binderMesh.transform, turretBuilding.BodyPartTransform, bindOriginTransform);


        if (isTurretWithinRange)
        {
            binderMesh.material = withinRangeMaterial;
        }
        else
        {
            binderMesh.material = outsideRangeMaterial;
        }
    }
        
    
}
