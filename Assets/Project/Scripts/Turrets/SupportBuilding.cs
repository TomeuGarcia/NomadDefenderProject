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



    [SerializeField] private GameObject rangePlaneMeshObject;
    private Material rangePlaneMaterial;

    [Header("COMPONENTS")]
    [SerializeField] private CapsuleCollider rangeCollider;
    [SerializeField] private MouseOverNotifier meshMouseNotifier;
    //private TurretPartBody_Prefab bodyPart;
    private TurretPartBase_Prefab basePart;

    [HideInInspector] public SupportBuildingStats stats;

    private bool isFunctional = false;



    void Awake()
    {
        HideRangePlane();
    }

    void Update()
    {

    }

    public override void DisablePlayerInteraction()
    {
        throw new System.NotImplementedException();
    }

    public override void EnablePlayerInteraction()
    {
        throw new System.NotImplementedException();
    }

    public override void GotPlaced()
    {
        throw new System.NotImplementedException();
    }

    public override void HideRangePlane()
    {
        rangePlaneMeshObject.SetActive(false);
    }

    public override void ShowRangePlane()
    {
        throw new System.NotImplementedException();
    }

    protected override void DisableFunctionality()
    {
        throw new System.NotImplementedException();
    }

    protected override void EnableFunctionality()
    {
        throw new System.NotImplementedException();
    }
}
