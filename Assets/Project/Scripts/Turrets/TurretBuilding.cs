using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBuilding : RangeBuilding
{
    public struct TurretBuildingStats
    {
        public int playCost;
        public int damage;
        [SerializeField, Min(1)] public float range;
        public float cadence;
    }

    [HideInInspector] public TurretBuildingStats stats;

    public TurretCardParts TurretCardParts { get; private set; }

    private TurretPartBody_Prefab bodyPart;
    public Transform BodyPartTransform => bodyPart.transform;
    public Transform BinderPointTransform => bodyPart.binderPoint;

    public PassiveDamageModifier baseDamagePassive;
    private BasePassive basePassive;


    public float TimeSinceLastShot { get; private set; }

    private float currentShootTimer;
    private Vector3 lastTargetedPosition;
    public Vector3 Position => transform.position;

    private Enemy targetedEnemy;


    public TurretPartBody.BodyType BodyType { get; private set; }

    private TurretPartAttack_Prefab turretAttack;
    public TurretPartAttack TurretPartAttack { get; private set; }
    

    [Header("HOLDERS")]
    [SerializeField] protected Transform bodyHolder;
    [SerializeField] protected Transform baseHolder;
    public Transform BaseHolder => baseHolder;

    [Header("PARTICLES")]
    [SerializeField] protected ParticleSystem placedParticleSystem;
    [SerializeField] private ParticleSystem upgradeParticles;


    public int CardLevel { get; private set; }


    public const int MIN_PLAY_COST = 10;

    public delegate void TurretBuildingEvent();
    public event TurretBuildingEvent OnGotPlaced;
    public event TurretBuildingEvent OnTimeSinceLastShotSet;
    public event TurretBuildingEvent OnGotEnabledPlacing;
    public event TurretBuildingEvent OnGotDisabledPlacing;
    public event TurretBuildingEvent OnGotMovedWhenPlacing;


    void Awake()
    {
        AwakeInit();
    }
    protected override void AwakeInit()
    {
        base.AwakeInit();
        CardBuildingType = BuildingCard.CardBuildingType.TURRET;
        TimeSinceLastShot = 0.0f;
        currentShootTimer = 0.0f;
        placedParticleSystem.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!isFunctional) return;

        ComputeNextTargetedEnemy();

        UpdateShoot();
        if (bodyPart.lookAtTarget)
        {
            if (TargetEnemyExists())
            {
                lastTargetedPosition = targetedEnemy.Position;
            }
            LookAtTarget();
        }
    }


    private void LookAtTarget()
    {
        Vector3 lookDirection = Vector3.ProjectOnPlane(lastTargetedPosition - bodyPart.transform.position, Vector3.up).normalized;
        Quaternion targetRot = Quaternion.LookRotation(lookDirection, Vector3.up);
        bodyPart.transform.rotation = Quaternion.RotateTowards(bodyPart.transform.rotation, targetRot, 600.0f * GameTime.DeltaTime);
    }

    public void Init(TurretBuildingStats turretStats, TurretCardParts turretCardParts, CurrencyCounter currencyCounter)
    {
        TurretCardParts = turretCardParts;
        this.TurretPartAttack = turretCardParts.turretPartAttack;
        TurretPartBody turretPartBody = turretCardParts.turretPartBody;
        TurretPartBase turretPartBase = turretCardParts.turretPartBase;
        TurretPassiveBase turretPassiveBase = turretCardParts.turretPassiveBase;
        bool hasBasePassive = !(turretPassiveBase.passive is BaseNullPassive);

        CardLevel = turretCardParts.cardLevel;

        InitStats(turretStats);
        BodyType = turretCardParts.turretPartBody.bodyType;


        this.turretAttack = TurretPartAttack.prefab.GetComponent<TurretPartAttack_Prefab>();
        
        bodyPart = Instantiate(turretPartBody.prefab, bodyHolder).GetComponent<TurretPartBody_Prefab>();
        bodyPart.Init(turretAttack.materialForTurret);

        basePart = Instantiate(turretPartBase.prefab, baseHolder).GetComponent<TurretPartBase_Prefab>();
        basePart.Init(this, stats.range);

        TimeSinceLastShot = currentShootTimer = Mathf.Max(stats.cadence - 0.2f, 0f);
        UpdateRange();
        SetUpTriggerNotifier(basePart.baseCollider.triggerNotifier);

        Upgrader.InitTurret(this, turretPartBody.damageLvl, turretPartBody.cadenceLvl, turretPartBase.rangeLvl, currencyCounter,
                            hasBasePassive, turretPassiveBase.visualInformation.sprite, turretPassiveBase.visualInformation.color);
        Upgrader.OnUpgrade += PlayUpgradeAnimation;

        //PASSIVE
        basePassive = turretCardParts.turretPassiveBase.passive;
        basePassive.ApplyEffects(this);

        DisableFunctionality();
    }

    public void ResetAttackPart(TurretPartAttack turretPartAttack)
    {
        this.TurretPartAttack = turretPartAttack;

        this.turretAttack = TurretPartAttack.prefab.GetComponent<TurretPartAttack_Prefab>();
        bodyPart.ResetProjectileMaterial(turretAttack.materialForTurret);
    }
    public void ResetBodyMaterial(Material newMaterial)
    {        
        bodyPart.ResetProjectileMaterial(newMaterial);
        bodyPart.SetDefaultMaterial();
    }

    protected override void UpdateRange()
    {
        basePart.baseCollider.UpdateRange(stats.range);
    }


    private void InitStats(TurretBuildingStats stats)
    {
        this.stats = stats;
    }
    public void UpdateStats(TurretBuildingStats stats)
    {
        InitStats(stats);
        UpdateRange();
    }

    public override void Upgrade(TurretUpgradeType upgradeType, int newStatLevel)
    {
        switch(upgradeType)
        {
            case TurretUpgradeType.ATTACK:
                stats.damage = TurretPartBody.damagePerLvl[newStatLevel - 1];
                break;
            case TurretUpgradeType.CADENCE:
                stats.cadence = TurretPartBody.cadencePerLvl[newStatLevel - 1];
                break;
            case TurretUpgradeType.RANGE:
                stats.range = TurretPartBase.rangePerLvl[newStatLevel - 1];
                UpdateRange();
                break;
        }

        InvokeOnBuildingUpgraded();
    }

    private void UpdateShoot()
    {
        TimeSinceLastShot += Time.deltaTime * GameTime.TimeScale;

        if (currentShootTimer < stats.cadence)
        {            
            currentShootTimer += Time.deltaTime * GameTime.TimeScale;
            return;
        }

        
        if (!TargetEnemyExists()) return;

        currentShootTimer = 0.0f;

        DoShootEnemyLogic(targetedEnemy);


        //// Code used when turrets used to have targetAmount stat:
        /*
        for (int i = 0; i < stats.targetAmount; i++)
        {
            if (i <= enemies.Count - 1)
            {
                DoShootEnemyLogic(enemies[i]);
            }
        }
        */
    }


    private void ComputeNextTargetedEnemy()
    {
        targetedEnemy = null;

        int enemyI = 0;
        while (enemyI < enemies.Count && !enemies[enemyI].CanBeTargeted())
        {
            ++enemyI;
        }

        if (enemyI < enemies.Count)
        {
            targetedEnemy = enemies[enemyI];
        }       
    }

    private bool TargetEnemyExists()
    {
        return targetedEnemy != null;
    }


    private void DoShootEnemyLogic(Enemy enemyTarget)
    {
        if (enemyTarget.DiesFromQueuedDamage())
        {
            RemoveEnemy(enemyTarget);
        }
        else
        {
            Shoot(enemyTarget);
            bodyPart.transform.DOComplete();
            bodyPart.transform.DOPunchPosition(bodyPart.transform.forward * -0.1f, 0.25f, 5, 1.0f, false);
        }
    }

    private void Shoot(Enemy enemyTarget)
    {
        Vector3 shootPoint = bodyPart.GetNextShootingPoint();
        TurretPartAttack_Prefab currentAttack = ProjectileAttacksFactory.GetInstance().GetAttackGameObject(turretAttack.GetAttackType, shootPoint, Quaternion.identity)
            .GetComponent<TurretPartAttack_Prefab>();

        currentAttack.transform.parent = BaseHolder;
        currentAttack.gameObject.SetActive(true);
        currentAttack.ProjectileShotInit(enemyTarget, this);


        // Spawn particle
        GameObject particles = ProjectileParticleFactory.GetInstance().GetAttackParticlesGameObject(currentAttack.GetAttackType, shootPoint, bodyPart.transform.rotation);
        particles.SetActive(true);
        particles.transform.parent = BaseHolder;


        // Audio
        GameAudioManager.GetInstance().PlayProjectileShot(BodyType);
        TimeSinceLastShot = 0.0f;
    }



    protected override void DisableFunctionality()
    {
        base.DisableFunctionality();

        basePart.baseCollider.DisableCollisions();

        bodyPart.SetPreviewMaterial();
        basePart.SetPreviewMaterial();
    }

    protected override void EnableFunctionality()
    {
        base.EnableFunctionality();

        basePart.baseCollider.EnableCollisions();

        bodyPart.SetDefaultMaterial();
        basePart.SetDefaultMaterial();
    }

    public override void GotPlaced()
    {
        HideRangePlane();
        EnableFunctionality();

        bodyHolder.DOPunchScale(Vector3.up * -0.3f, 0.7f, 7);
     
        placedParticleSystem.gameObject.SetActive(true);
        placedParticleSystem.Play();

        Upgrader.OnBuildingOwnerPlaced();

        InvokeOnPlaced();

        TurretPartAttack.OnTurretPlaced(this, turretAttack.materialForTurret);
        if (OnGotPlaced != null) OnGotPlaced();
    }
    public override void GotEnabledPlacing()
    {
        basePart.GotEnabledPlacing();

        if (OnGotEnabledPlacing != null) OnGotEnabledPlacing();
    }
    public override void GotDisabledPlacing()
    {
        basePart.GotDisabledPlacing();

        if (OnGotDisabledPlacing != null) OnGotDisabledPlacing();
    }
    public override void GotMovedWhenPlacing()
    {
        basePart.GotMovedWhenPlacing();

        if (OnGotMovedWhenPlacing != null) OnGotMovedWhenPlacing();
    }

    public override void ShowQuickLevelUI()
    {
        Upgrader.ShowQuickLevelDisplay();
    }

    public override void HideQuickLevelUI()
    {
        Upgrader.HideQuickLevelDisplay();
    }

    private void PlayUpgradeAnimation(TurretUpgradeType upgradeType, int upgradeLevel)
    {
        StartCoroutine(UpgradeAnimation(upgradeType, upgradeLevel));
    }
    
    private IEnumerator UpgradeAnimation(TurretUpgradeType upgradeType, int upgradeLevel)
    {
        bodyHolder.DOComplete();
        bodyHolder.DOPunchScale(Vector3.up * 0.5f, 0.7f, 5);
        

        ParticleSystemRenderer particleRenderer = upgradeParticles.GetComponentInChildren<ParticleSystemRenderer>();
        if (upgradeType == TurretUpgradeType.ATTACK)
        {            
            particleRenderer.sharedMaterial = buildingsUtils.AttackUpgradeParticleMat;
        }
        else if (upgradeType == TurretUpgradeType.CADENCE)
        {
            particleRenderer.sharedMaterial = buildingsUtils.CadencyUpgradeParticleMat;
        }
        else //(upgradeType == TurretUpgradeType.RANGE)
        {
            particleRenderer.sharedMaterial = buildingsUtils.RangeUpgradeParticleMat;
        }


        GameAudioManager.GetInstance().PlayInBattleBuildingUpgrade();
        yield return new WaitForSeconds(0.25f);
        upgradeParticles.Play();

        bodyPart.PlayUpgradeAnimation(upgradeLevel);
    }


    public override void SetBuildingPartsColor(Color color) 
    {
        bodyPart.SetMaterialColor(color);
        basePart.SetMaterialColor(color);
    }

    public override void SetPreviewCanBePlacedColor() 
    {
        previewColorInUse = buildingsUtils.PreviewCanBePlacedColor;
        SetBuildingPartsColor(buildingsUtils.PreviewCanBePlacedColor);
    }

    public override void SetPreviewCanNOTBePlacedColor() 
    {
        previewColorInUse = buildingsUtils.PreviewCanNOTBePlacedColor;
        SetBuildingPartsColor(buildingsUtils.PreviewCanNOTBePlacedColor);
    }


}
