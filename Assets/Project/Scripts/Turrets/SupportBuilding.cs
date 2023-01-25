using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupportBuilding : RangeBuilding
{
    public struct SupportBuildingStats
    {
        public int playCost;
        [SerializeField, Min(1)] public float range;
    }

    [HideInInspector] public SupportBuildingStats stats;

        

    [Header("HOLDERS")]
    [SerializeField] protected Transform baseHolder;




    void Awake()
    {
        AwakeInit();
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
    }

}
