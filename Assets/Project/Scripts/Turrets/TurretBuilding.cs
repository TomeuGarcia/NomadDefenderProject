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

    private TurretPartBody_Prefab bodyPart;
    public Transform BodyPartTransform => bodyPart.transform;
    public Transform BinderPointTransform => bodyPart.binderPoint;

    private BasePassive basePassive;



    private ProjectileShootingController _shootingController;
    public float TimeSinceLastShot => _shootingController.TimeSinceLastShot;

    public Vector3 Position => transform.position;
    
    public TurretPartBody.BodyType BodyType => bodyPart.BodyType;

    private TurretPartAttack_Prefab turretAttack;
    public Material MaterialForTurret => turretAttack.materialForTurret;
    public TurretPartAttack TurretPartAttack { get; private set; }
    

    [Header("HOLDERS")]
    [SerializeField] protected Transform bodyHolder;
    [SerializeField] protected Transform baseHolder;
    public Transform BaseHolder => baseHolder;

    [Header("PARTICLES")]
    [SerializeField] protected ParticleSystem placedParticleSystem;
    [SerializeField] private Transform _upgradeParticlesPosition;


    public int CardLevel { get; private set; }


    public const int MIN_PLAY_COST = 10;

    public delegate void TurretBuildingEvent();
    public event TurretBuildingEvent OnGotPlaced;
    public event TurretBuildingEvent OnTimeSinceLastShotSet;
    public event TurretBuildingEvent OnGotEnabledPlacing;
    public event TurretBuildingEvent OnGotDisabledPlacing;
    public event TurretBuildingEvent OnGotMovedWhenPlacing;

    private TestingSnapshot _testingSnapshot;
    public class TestingSnapshot
    {
        private string _turretName;
        private float _turretRange;
        private float _totalTimePlaced;
        private float _timeTargetingEnemies;

        public TestingSnapshot(string turretName)
        {
            _turretName = turretName;
            _turretRange = 0;
            _totalTimePlaced = 0;
            _timeTargetingEnemies = 0;
        }

        public void SetRange(float turretRange)
        {
            _turretRange = turretRange;
        }
        
        public void UpdateTotalTimePlaced(float deltaTime)
        {
            _totalTimePlaced += deltaTime;
        }
        
        public void UpdateTimeTargetingEnemies(float deltaTime)
        {
            _timeTargetingEnemies += deltaTime;
        }

        public void Print()
        {
            Debug.Log(_turretName + ": " + 
                "Range: " + _turretRange + " | " +
                "Total time placed: " + _totalTimePlaced + " | " +
                "Time targeting enemies: " + _timeTargetingEnemies);
        }
    }


    void Awake()
    {
        AwakeInit();
    }
    protected override void AwakeInit()
    {
        base.AwakeInit();
        CardBuildingType = BuildingCard.CardBuildingType.TURRET;
        placedParticleSystem.gameObject.SetActive(false);
        
        TDGameManager.OnGameFinishStart += PrintData;
    }

    private void OnDestroy()
    {
        if (_statsController != null)
        {
            _statsController.OnStatsUpdated -= OnControllerUpdatedStats;
        }

        TDGameManager.OnGameFinishStart -= PrintData;
    }

    private void PrintData()
    {
        _testingSnapshot.SetRange(Stats.RadiusRange);
        _testingSnapshot.Print();
    }

    private void Update()
    {
        if (!isFunctional) return;

        _shootingController.UpdateShoot(out bool targetEnemyExists);

        if (bodyPart.lookAtTarget)
        {
            LookAtTarget();
        }
    }


    private void LookAtTarget()
    {
        Vector3 lookDirection = Vector3.ProjectOnPlane(_shootingController.LastTargetedPosition - bodyPart.transform.position, Vector3.up).normalized;
        Quaternion targetRot = Quaternion.LookRotation(lookDirection, Vector3.up);
        bodyPart.transform.rotation = Quaternion.RotateTowards(bodyPart.transform.rotation, targetRot, 600.0f * GameTime.DeltaTime);
    }

    public void Init(TurretCardStatsController statsController, TurretCardData turretCardData, CurrencyCounter currencyCounter)
    {
        _statsController = statsController;        
        _statsController.OnStatsUpdated += OnControllerUpdatedStats;

        CardData = turretCardData;

        TurretCardPartsGroup parts = turretCardData.SharedPartsGroup;
        TurretPartAttack = parts.Projectile;
        TurretPartBody turretPartBody = parts.Body;
        TurretPassiveBase turretPassiveBase = parts.Passive;
        bool hasBasePassive = !(turretPassiveBase.passive is BaseNullPassive);

        CardLevel = turretCardData.CardUpgradeLevel;

        turretAttack = TurretPartAttack.prefab.GetComponent<TurretPartAttack_Prefab>();
        
        bodyPart = Instantiate(turretPartBody.prefab, bodyHolder).GetComponent<TurretPartBody_Prefab>();
        bodyPart.Init(turretPartBody.bodyType, turretAttack.materialForTurret);

        basePart = Instantiate(turretPartBody.BasePartPrimitive.Prefab, baseHolder).GetComponent<TurretPartBase_Prefab>();
        basePart.Init(this, Stats.RadiusRange);
        UpdateRange();
        
        SetUpTriggerNotifier(basePart.baseCollider.triggerNotifier);

        Upgrader.InitTurret(this, _statsController, _statsController, CardLevel, currencyCounter,
                            hasBasePassive, turretPassiveBase.visualInformation.sprite, turretPassiveBase.visualInformation.color);
        Upgrader.OnUpgrade += PlayUpgradeAnimation;

        //PASSIVE
        basePassive = parts.Passive.passive;
        basePassive.ApplyEffects(this);

        DisableFunctionality();

        string dataTestingName = turretPartBody.name + "--" + turretPassiveBase.name + "--" + TurretPartAttack.name;
        _testingSnapshot = new TestingSnapshot(dataTestingName);
        
        _shootingController = new ProjectileShootingController(this, Stats, turretAttack, bodyPart, _testingSnapshot);
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
