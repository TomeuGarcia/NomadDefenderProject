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


    [SerializeField]GameObject[] visualUpgrades;
    [Header("HOLDERS")]
    [SerializeField] protected Transform baseHolder;




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
    }



    public void Init(SupportBuildingStats stats, TurretPartBase turretPartBase, CurrencyCounter currencyCounter)
    {
        InitStats(stats);

        basePart = Instantiate(turretPartBase.prefab, baseHolder).GetComponent<TurretPartBase_Prefab>();
        basePart.InitAsSupportBuilding(this,stats.range);

        UpdateRange();
        SetUpTriggerNotifier(basePart.baseCollider.triggerNotifier);

        upgrader.InitSupport(currencyCounter); //TODO: change range for the actual level

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
        basePart.Upgrade(newStatLevel);

        if (visualUpgrades.Length == 0) return;
        if (visualUpgrades[newStatLevel - 1] != null) visualUpgrades[newStatLevel - 1].SetActive(true);
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
    }

}
