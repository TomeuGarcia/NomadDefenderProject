using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    private BuildingCard selectedBuildingCard;
    private Building selectedBuilding;


    public delegate void BuildingPlacerAction();
    public event BuildingPlacerAction OnBuildingPlaced;



    public void EnablePlacing(BuildingCard selectedBuildingCard)
    {
        this.selectedBuildingCard = selectedBuildingCard;
        selectedBuilding = selectedBuildingCard.buildingPrefab.GetComponent<Building>();

        Tile.OnTileSelected += TryPlaceBuilding;
    }

    public void DisablePlacing()
    {
        Tile.OnTileSelected -= TryPlaceBuilding;
    }



    private void TryPlaceBuilding(Tile tile)
    {
        if (CanPlaceSelectedBuildingOnTile(tile))
        {
            PlaceSelectedBuilding(tile);
            DisablePlacing();
        }
    }

    public bool CanPlaceSelectedBuildingOnTile(Tile tile)
    {
        return selectedBuilding.validTileType == tile.tileType;
    }

    public void PlaceSelectedBuilding(Tile tile)
    {
        Building building = Instantiate(selectedBuildingCard.buildingPrefab, tile.buildingPlacePosition, Quaternion.identity).GetComponent<Building>();
        building.GotPlaced(selectedBuildingCard.turretStats);

        if (OnBuildingPlaced != null) OnBuildingPlaced();
    }



}
