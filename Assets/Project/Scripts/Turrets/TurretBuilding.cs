using System;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBuilding : RangeBuilding
{
    private TurretCardStatsController _statsController;
    public TurretStatsSnapshot Stats => _statsController.CurrentStats;
    public ITurretStatsBonusController StatsBonusController => _statsController;

    public TurretCardData CardData { get; private set; }

    private ITurretObjectLiftimeCycle _abilitiesObjectLifetimeCycle;
    private ITurretPlacingLifetimeCycle _abilitiesPlacingLifetimeCycle;
    

    private TurretPartBody_Prefab bodyPart;
    public Transform BodyPartTransform => bodyPart.transform;
    public Transform BinderPointTransform => bodyPart.binderPoint;


    private AProjectileShootingController _shootingController;
    public float TimeSinceLastShot => _shootingController.TimeSinceLastShot;

    public Vector3 Position => transform.position;
    
    public TurretPartBody.BodyType BodyType => bodyPart.BodyType;

    public Material MaterialForTurret => ProjectileDataModel.MaterialForTurret;
    public TurretPartProjectileDataModel ProjectileDataModel { get; private set; }
    
    private TurretViewAddOnController _viewAddOnController;
    public ITurretViewAddOnController ViewAddOnController => _viewAddOnController;
    

    [Header("HOLDERS")]
    [SerializeField] protected Transform bodyHolder;
    [SerializeField] protected Transform baseHolder;
    public Transform BaseHolder => baseHolder;

    [Header("PARTICLES")]
    [SerializeField] private Transform _upgradeParticlesPosition;


    public bool IsDisabled { get; set; } = false;

    
    public int CardLevel { get; private set; }
    public override Vector3 PlacingParticlesPosition => _upgradeParticlesPosition.position;


    public const int MIN_PLAY_COST = 10;


    void Awake()
    {
        AwakeInit();
    }
    protected override void AwakeInit()
    {
        base.AwakeInit();
        CardBuildingType = BuildingCard.CardBuildingType.TURRET;
    }


    private void OnDestroy()
    {
        if (_statsController != null)
        {
            _statsController.OnStatsUpdated -= OnControllerUpdatedStats;
        }

        _abilitiesObjectLifetimeCycle.OnTurretDestroyed();
        _shootingController.ClearAllProjectiles();
        _viewAddOnController.StopViewingAddOns();
    }



    private void Update()
    {
        if (!isFunctional || IsDisabled) return;

        _shootingController.UpdateShoot();
        LookAtTarget();
    }


    private void LookAtTarget()
    {
        Vector3 lookDirection = 
            Vector3.ProjectOnPlane(_shootingController.LastTargetedPosition - bodyPart.transform.position, Vector3.up)
                .normalized;
        
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        Quaternion currentRotation = 
            Vector3.Dot(lookDirection, bodyPart.transform.forward) > 0.95f 
            ? targetRotation 
            : Quaternion.RotateTowards(bodyPart.transform.rotation, targetRotation, 600.0f * GameTime.DeltaTime);

        bodyPart.transform.rotation = currentRotation;
    }

    public void Init(BuildingCard buildingCard, TurretCardStatsController statsController, 
        TurretCardData turretCardData, CurrencyCounter currencyCounter)
    {
        BuildingCard = buildingCard;
        _statsController = statsController;        
        _statsController.OnStatsUpdated += OnControllerUpdatedStats;

        CardData = turretCardData;
        ProjectileDataModel = CardData.SharedPartsGroup.Projectile;
        _abilitiesObjectLifetimeCycle = CardData.PassiveAbilitiesController;
        _abilitiesPlacingLifetimeCycle = CardData.PassiveAbilitiesController;

        TurretCardPartsGroup parts = turretCardData.SharedPartsGroup;
        TurretPartBody turretPartBody = parts.Body;

        CardLevel = turretCardData.CardUpgradeLevel;

        
        bodyPart = Instantiate(turretPartBody.prefab, bodyHolder).GetComponent<TurretPartBody_Prefab>();
        bodyPart.Init(turretPartBody.bodyType, ProjectileDataModel.MaterialForTurret);

        basePart = Instantiate(turretPartBody.BasePartPrimitive.Prefab, baseHolder).GetComponent<TurretPartBase_Prefab>();
        basePart.Init(this, Stats.RadiusRange);
        UpdateRange();
        
        SetUpTriggerNotifier(basePart.baseCollider.triggerNotifier);
        
        Upgrader.InitTurret(this, _statsController, _statsController, CardLevel, currencyCounter,
            CardData.MakeIconsDisplayData());
        Upgrader.OnUpgrade += PlayUpgradeAnimation;


        DisableFunctionality();

        _viewAddOnController = new TurretViewAddOnController(bodyPart.AddOnsParent);

        InitShootingController();
        
        _abilitiesObjectLifetimeCycle.OnTurretCreated(this);
    }

    private void InitShootingController()
    {
        _shootingController = ProjectileDataModel.ShootingControllerCreator.Create(
            new AProjectileShootingController.CreateData(this, Stats, ProjectileDataModel, bodyPart,
                CardData.PassiveAbilitiesController));
    }
    
    public void ResetProjectilePart(TurretPartProjectileDataModel newProjectilePart)
    {
        ProjectileDataModel = newProjectilePart;
        bodyPart.ResetProjectileMaterial(MaterialForTurret);
        
        InitShootingController();
    }
    public void ResetBodyMaterial(Material newMaterial)
    {        
        bodyPart.ResetProjectileMaterial(newMaterial);
    }

    protected override void UpdateRange()
    {
        basePart.baseCollider.UpdateRange(Stats.RadiusRange);
    }


    private void OnControllerUpdatedStats()
    {
        UpdateRange();
        upgrader.OnStatsUpdated();
    }
    
    public void OnEnemyShot(Enemy shotEnemy)
    {
        if (shotEnemy.DiesFromQueuedDamage())
        {
            RemoveEnemy(shotEnemy);
        }
        
        PlayShootAnimation();
    }
    private void PlayShootAnimation()
    {
        bodyPart.transform.DOComplete();
        bodyPart.transform.DOPunchPosition(bodyPart.transform.forward * -0.1f, 0.25f, 5, 1.0f, false);
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

    protected override void DoGotPlaced()
    {
        HideRangePlane();
        EnableFunctionality();

        bodyHolder.DOPunchScale(Vector3.up * -0.3f, 0.7f, 7);

        Upgrader.OnBuildingOwnerPlaced();

        InvokeOnPlaced();
        
        _abilitiesPlacingLifetimeCycle.OnTurretPlaced(this);
        _viewAddOnController.StartViewingAddOns();
    }

    protected override void DoGotUnplaced()
    {
        _statsController.ResetUpgradeLevel();
        bodyPart.ResetUpgradeVisuals();
        upgrader.ResetState();
    }

    public override void GotEnabledPlacing()
    {
        basePart.GotEnabledPlacing();

        _abilitiesPlacingLifetimeCycle.OnTurretPlacingStart();
    }
    public override void GotDisabledPlacing()
    {
        basePart.GotDisabledPlacing();

        _abilitiesPlacingLifetimeCycle.OnTurretPlacingFinish();
    }
    public override void GotMovedWhenPlacing()
    {
        basePart.GotMovedWhenPlacing();

        _abilitiesPlacingLifetimeCycle.OnTurretPlacingMove();
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

        GameAudioManager.GetInstance().PlayInBattleBuildingUpgrade();
        yield return new WaitForSeconds(0.25f);

        ServiceLocator.GetInstance().GeneralParticleFactory
            .SpawnTurretUpgradeParticles(upgradeType, _upgradeParticlesPosition.position, Quaternion.identity);

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
