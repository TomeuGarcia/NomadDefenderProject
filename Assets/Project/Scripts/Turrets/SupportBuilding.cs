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

    
    //private TurretPartBody_Prefab bodyPart;
    private TurretPartBase_Prefab basePart;





    void Awake()
    {
        AwakeInit();
    }
    protected override void AwakeInit()
    {
        base.AwakeInit();
    }


    void Update()
    {

    }


    protected override void DisableFunctionality()
    {
        throw new System.NotImplementedException();
    }

    protected override void EnableFunctionality()
    {
        throw new System.NotImplementedException();
    }

    public override void GotPlaced()
    {
        throw new System.NotImplementedException();
    }

}
