using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SupportBuilding : RangeBuilding
{
    public struct SupportBuildingStats
    {
        public int playCost;
        [SerializeField, Min(1)] public float range;
    }

    [HideInInspector] public SupportBuildingStats stats;
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



    public void Init(SupportBuildingStats stats, TurretPartBase turretPartBase, CurrencyCounter currencyCounter, Sprite abilitySprite, Color abilityColor)
    {
        InitStats(stats);

        this.turretPartBase = turretPartBase;

        basePart = Instantiate(turretPartBase.prefab, baseHolder).GetComponent<TurretPartBase_Prefab>();
        basePart.InitAsSupportBuilding(this, stats.range);

        UpdateRange();
        SetUpTriggerNotifier(basePart.baseCollider.triggerNotifier);

        Upgrader.InitSupport(currencyCounter, abilitySprite, abilityColor, turretPartBase);

        DisableFunctionality();
        basePart.PlacedParticleSystem.gameObject.SetActive(false);
    }

    protected override void UpdateRange()
    {
        basePart.baseCollider.UpdateRange(stats.range);
    }


    public void InitStats(SupportBuildingStats stats)
    {
        this.stats = stats;
    }

    public override void Upgrade(TurretUpgradeType upgradeType, int newStatLevel)
    {
        basePart.Upgrade(this, newStatLevel);

        if (visualUpgrades.Length == 0) return;
        if (visualUpgrades[newStatLevel - 1] != null) visualUpgrades[newStatLevel - 1].SetActive(true);

        InvokeOnBuildingUpgraded();
    }
    public void UpgradeRangeIncrementingLevel()
    {
        int newStatLevel = 1; // IDK - Tomeu
        stats.range = turretPartBase.GetRangeByLevel(newStatLevel);
        UpdateRange();
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
        StartCoroutine(UpgradeAnimation(upgradeType));
    }

    private IEnumerator UpgradeAnimation(TurretUpgradeType upgradeType)
    {
        basePart.DOComplete();
        basePart.MeshTransform.DOPunchScale(Vector3.up * 0.5f, 0.7f, 5);


        ParticleSystemRenderer particleRenderer = upgradeParticles.GetComponentInChildren<ParticleSystemRenderer>();
        particleRenderer.sharedMaterial = buildingsUtils.SupportUpgradeParticleMat;

        GameAudioManager.GetInstance().PlayInBattleBuildingUpgrade();
        yield return new WaitForSeconds(0.25f);
        upgradeParticles.Play();
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
