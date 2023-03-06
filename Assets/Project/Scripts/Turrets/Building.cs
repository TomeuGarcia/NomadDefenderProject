using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Building : MonoBehaviour
{
    [SerializeField] public Tile.TileType validTileType;


    protected bool isFunctional = false;


    public delegate void BuildingAction();
    public static event BuildingAction OnBuildingPlaced;
    protected void InvokeOnBuildingPlaced() { if (OnBuildingPlaced != null) OnBuildingPlaced(); }


    protected virtual void DisableFunctionality()
    {
        isFunctional = false;
    }

    protected virtual void EnableFunctionality()
    {
        isFunctional = true;
    }

    protected abstract void AwakeInit();
    public abstract void GotPlaced();

    //public virtual void Init(TurretStats turretStats, BuildingCard.BuildingCardParts buildingCardParts)
    //{
    //}

    public abstract void ShowRangePlane();

    public abstract void HideRangePlane();

    public abstract void EnablePlayerInteraction();

    public abstract void DisablePlayerInteraction();

}
