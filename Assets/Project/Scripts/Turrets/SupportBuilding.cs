using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SupportBuilding : RangeBuilding
{
    private SupportCardStatsController _statsController;
    public SupportStatsSnapshot Stats => _statsController.CurrentStats;


    private TurretPartBase turretPartBase;


    [SerializeField]GameObject[] visualUpgrades;
    [Header("HOLDERS")]
    [SerializeField] protected Transform baseHolder;


    [Header("PARTICLES")]
    [SerializeField] private ParticleSystem upgradeParticles;
    



    void Awake()
    {
        AwakeInit();

        foreach(GameObject go in visualUpgrades)
        {
            go.SetActive(false);
        }
    }
    protected override void AwakeInit()
    {
        base.AwakeInit();
        CardBuildingType = BuildingCard.CardBuildingType.SUPPORT;
    }

    private void OnDestroy()
    {
        if (_statsController != null)
        {
            _statsController.OnStatsUpdated -= OnControllerUpdatedStats;
        }
    }


    public void Init(SupportCardStatsController statsController, TurretPartBase turretPartBase, 
        CurrencyCounter currencyCounter, Sprite abilitySprite, Color abilityColor)
    {
        _statsController = statsController;
        _statsController.OnStatsUpdated += OnControllerUpdatedStats;

        this.turretPartBase = turretPartBase;

        basePart = Instantiate(turretPartBase.prefab, baseHolder).GetComponent<TurretPartBase_Prefab>();
        basePart.InitAsSupportBuilding(this, Stats.RadiusRange);

        UpdateRange();
        SetUpTriggerNotifier(basePart.baseCollider.triggerNotifier);

        Upgrader.InitSupport(this, _statsController, currencyCounter, abilitySprite, abilityColor, turretPartBase);

        DisableFunctionality();
        basePart.PlacedParticleSystem.gameObject.SetActive(false);
    }

    protected override void UpdateRange()
    {
        basePart.baseCollider.UpdateRange(Stats.RadiusRange);
    }
    private void OnControllerUpdatedStats()
    {
        UpdateRange();
    }

    public void ApplyUpgrade(int newStatsLevel)
    {
        basePart.Upgrade(this, newStatsLevel);
        InvokeOnBuildingUpgraded();
    }

    public void UpgradeRangeIncrementingLevel()
    {
        _statsController.IncrementUpgradeLevel();
    }

    protected override void DisableFunctionality()
    {
        base.DisableFunctionality();

        basePart.baseCollider.DisableCollisions();

        basePart.SetPreviewMaterial();
    }

    protected override void EnableFunctionality()
    {
        base.EnableFunctionality();

        basePart.baseCollider.EnableCollisions();

        basePart.SetDefaultMaterial();
    }

    public override void GotPlaced()
    {
        HideRangePlane();
        EnableFunctionality();
        basePart.OnGetPlaced();

        basePart.MeshTransform.DOPunchScale(Vector3.up * -0.3f, 0.7f, 7);
        
        basePart.PlacedParticleSystem.gameObject.SetActive(true);
        basePart.PlacedParticleSystem.Play();

        Upgrader.OnBuildingOwnerPlaced();
        Upgrader.OnUpgrade += PlayUpgradeAnimation;

        InvokeOnPlaced();
    }

    public override void GotEnabledPlacing()
    {
        basePart.GotEnabledPlacing();
    }
    public override void GotDisabledPlacing()
    {
        basePart.GotDisabledPlacing();
    }
    public override void GotMovedWhenPlacing()
    {
        basePart.GotMovedWhenPlacing();
    }

    public override void ShowQuickLevelUI() 
    {
        Upgrader.ShowQuickLevelDisplay();
        basePart.GotHoveredWhenPlaced();
    }

    public override void HideQuickLevelUI() 
    {
        Upgrader.HideQuickLevelDisplay();
        basePart.GotUnoveredWhenPlaced();
    }


    private void PlayUpgradeAnimation(TurretUpgradeType upgradeType, int upgradeLevel)
    {
        StartCoroutine(UpgradeAnimation(upgradeType, upgradeLevel));
    }

    private IEnumerator UpgradeAnimation(TurretUpgradeType upgradeType, int upgradeLevel)
    {
        basePart.DOComplete();
        basePart.MeshTransform.DOPunchScale(Vector3.up * 0.5f, 0.7f, 5);


        ParticleSystemRenderer particleRenderer = upgradeParticles.GetComponentInChildren<ParticleSystemRenderer>();
        particleRenderer.sharedMaterial = buildingsUtils.SupportUpgradeParticleMat;

        GameAudioManager.GetInstance().PlayInBattleBuildingUpgrade();
        yield return new WaitForSeconds(0.25f);
        upgradeParticles.Play();

        if (visualUpgrades.Length > 0)
        {
            if (visualUpgrades[upgradeLevel - 1] != null) visualUpgrades[upgradeLevel - 1].SetActive(true);
        }        
    }


    public override void SetBuildingPartsColor(Color color)
    {
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
