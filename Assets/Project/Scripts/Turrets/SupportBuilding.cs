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


    [Header("COLLIDER")]
    [SerializeField] private CapsuleCollider rangeCollider;

    
    private TurretPartBase_Prefab basePart;

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



    public void Init(SupportBuildingStats stats, TurretPartBase turretPartBase)
    {
        InitStats(stats);

        //area effect visual feedback
        float planeRange = stats.range * 2 + 1; //only for square
        float range = stats.range;

        rangeCollider.radius = range + 0.5f;
        rangePlaneMeshObject.transform.localScale = Vector3.one * (planeRange / 10f);
        rangePlaneMaterial = rangePlaneMeshObject.GetComponent<MeshRenderer>().materials[0];
        rangePlaneMaterial.SetFloat("_TileNum", planeRange);

        basePart = Instantiate(turretPartBase.prefab, baseHolder).GetComponent<TurretPartBase_Prefab>();
        basePart.InitAsSupportBuilding(this,stats.range);

        DisableFunctionality();
    }

    public void InitStats(SupportBuildingStats stats)
    {
        this.stats = stats;
    }

    protected override void DisableFunctionality()
    {
        base.DisableFunctionality();
        rangeCollider.enabled = false;
        basePart.SetPreviewMaterial();
    }

    protected override void EnableFunctionality()
    {
        base.EnableFunctionality();
        rangeCollider.enabled = true;
        basePart.SetDefaultMaterial();
    }

    public override void GotPlaced()
    {
        HideRangePlane();
        EnableFunctionality();
    }

}
