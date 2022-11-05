using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Turret;

public class Building : MonoBehaviour
{
    [SerializeField] public Tile.TileType validTileType;
    
    [SerializeField] private GameObject meshObject;
    [SerializeField] private GameObject previewMeshObject;


    [System.Serializable]
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
        meshObject.SetActive(false);
        previewMeshObject.SetActive(true);
    }

    protected virtual void EnableFunctionality()
    {
        meshObject.SetActive(true);
        previewMeshObject.SetActive(false);
    }

    public virtual void GotPlaced()
    {
    }

    public virtual void Init(TurretStats turretStats)
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
