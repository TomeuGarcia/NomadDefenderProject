using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public enum TileType { PATH, GRASS }


    [SerializeField] public TileType tileType;
    [SerializeField] private Vector3 buildingPlaceOffset = Vector3.up * 0.2f;

    public Vector3 buildingPlacePosition => transform.position + buildingPlaceOffset;


    public delegate void TileAction(Tile tile);
    public static event TileAction OnTileSelected;
    public static event TileAction OnTileHovered;


    private void OnMouseDown()
    {
        if (OnTileSelected != null) OnTileSelected(this);
    }

    private void OnMouseEnter()
    {
        if (OnTileHovered != null) OnTileHovered(this);
    }

}
