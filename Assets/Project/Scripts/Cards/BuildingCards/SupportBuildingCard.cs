using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupportBuildingCard : BuildingCard
{
    [System.Serializable]
    public class SupportCardParts
    {
        public SupportCardParts(TurretPartBase turretPartBase)
        {

            this.turretPartBase = turretPartBase;
        }

        public SupportCardParts(SupportCardParts other)
        {
            this.turretPartBase = other.turretPartBase;
        }

        public TurretPartBase turretPartBase; // TODO change

        public int GetCostCombinedParts()
        {
            return turretPartBase.cost;
        }
    }


    public SupportCardParts supportCardParts { get; private set; }





    public override void CreateCopyBuildingPrefab()
    {
        throw new System.NotImplementedException();
    }

    public override int GetCardPlayCost()
    {
        throw new System.NotImplementedException();
    }

    protected override void GetMaterialsRefs()
    {
        throw new System.NotImplementedException();
    }

    protected override void InitStatsFromTurretParts()
    {
        throw new System.NotImplementedException();
    }

    protected override void InitVisuals()
    {
        throw new System.NotImplementedException();
    }
}
