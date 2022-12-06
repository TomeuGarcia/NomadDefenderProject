using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Turret;

public class Building : MonoBehaviour
{
    [SerializeField] public Tile.TileType validTileType;

    [Header("HOLDERS")]
    [SerializeField] protected Transform bodyHolder;
    [SerializeField] protected Transform baseHolder;

    public struct TurretStats
    {
        public int playCost;
        public int damage;
        [SerializeField, Min(1)] public int range;
        public int targetAmount;
        public float cadence;
    }


    protected virtual void DisableFunctionality()
    {
    }

    protected virtual void EnableFunctionality()
    {
    }

    public virtual void GotPlaced()
    {
    }

    public virtual void Init(TurretStats turretStats, GameObject turretAttack, GameObject turretPartBody, GameObject turretPartBase, TurretPartBody.BodyType bodyType)
    {
    }

    public virtual void ShowRangePlane()
    {
    }

    public virtual void HideRangePlane()
    {
    }

    public virtual void EnablePlayerInteraction()
    {
    }

    public virtual void DisablePlayerInteraction()
    {
    }

}
