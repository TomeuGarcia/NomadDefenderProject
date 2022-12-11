using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static TurretBuilding;

public abstract class Building : MonoBehaviour
{
    [SerializeField] public Tile.TileType validTileType;

    [Header("HOLDERS")]
    [SerializeField] protected Transform bodyHolder;
    [SerializeField] protected Transform baseHolder;

    protected abstract void DisableFunctionality();

    protected abstract void EnableFunctionality();

    public abstract void GotPlaced();

    //public virtual void Init(TurretStats turretStats, BuildingCard.BuildingCardParts buildingCardParts)
    //{
    //}

    public abstract void ShowRangePlane();

    public abstract void HideRangePlane();

    public abstract void EnablePlayerInteraction();

    public abstract void DisablePlayerInteraction();

}
