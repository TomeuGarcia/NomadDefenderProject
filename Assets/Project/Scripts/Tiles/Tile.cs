using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public enum TileType { PATH, GRASS, OBSTACLE }


    [SerializeField] public TileType tileType;
    [SerializeField] private Vector3 buildingPlaceOffset = Vector3.up * 0.2f;
    [HideInInspector] public bool isOccupied = false;

    public Vector3 buildingPlacePosition => transform.position + buildingPlaceOffset;


    public delegate void TileAction(Tile tile);
    public static event TileAction OnTileSelected;
    public static event TileAction OnTileHovered;
    public delegate void TileAction2();
    public static event TileAction2 OnTileUnhovered;


    private void OnMouseDown()
    {
        if (isOccupied) return;

        if (OnTileSelected != null) OnTileSelected(this);
    }

    private void OnMouseEnter()
    {
        if (isOccupied) return;

        if (OnTileHovered != null) OnTileHovered(this);
    }

    private void OnMouseExit()
    {
        if (isOccupied) return;

        if (OnTileUnhovered != null) OnTileUnhovered();
    }
}
