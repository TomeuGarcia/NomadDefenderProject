using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Turret;

public class Building : MonoBehaviour
{
    [SerializeField] public Tile.TileType validTileType;


    [System.Serializable]
    public struct TurretStats
    {
        public int playCost;
        public int damage;
        public int range;
        public int targetAmount;
        public float cadence;
    }


    public virtual void GotPlaced(TurretStats turretStats)
    {

    }
}
