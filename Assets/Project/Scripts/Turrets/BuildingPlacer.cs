using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    private BuildingCard selectedBuildingCard = null;
    private Building selectedBuilding = null;
    private List<Building> placedBuildings = new List<Building>();


    public delegate void BuildingPlacerAction();
    public event BuildingPlacerAction OnBuildingPlaced;

    private bool SelectedBuildingIsBeingShown => selectedBuilding != null;



    public void EnablePlacing(BuildingCard selectedBuildingCard)
    {
        this.selectedBuildingCard = selectedBuildingCard;
        selectedBuilding = selectedBuildingCard.copyTurretPrefab.GetComponent<Building>();

        Tile.OnTileUnhovered += HideBuildingPreview;
        Tile.OnTileHovered += ShowBuildingOnTilePreview;
        Tile.OnTileSelected += TryPlaceBuilding;

        DisablePlacedBuildingsPlayerInteraction();
    }

    public void DisablePlacing()
    {
        Tile.OnTileUnhovered -= HideBuildingPreview;
        Tile.OnTileHovered -= ShowBuildingOnTilePreview;
        Tile.OnTileSelected -= TryPlaceBuilding;

        if (SelectedBuildingIsBeingShown)
        {
            HideBuildingPreview();
        }

        EnablePlacedBuildingsPlayerInteraction();
    }



    private void ShowBuildingOnTilePreview(Tile tile)
    {
        ShowAndPositionSelectedBuilding(tile);
    }

    private void HideBuildingPreview()
    {
        HideSelectedBuilding();
    }


    private void TryPlaceBuilding(Tile tile)
    {
        if (CanPlaceSelectedBuildingOnTile(tile))
        {
            PlaceSelectedBuilding(tile);
        }
    }

    private bool CanPlaceSelectedBuildingOnTile(Tile tile)
    {
        return selectedBuilding.validTileType == tile.tileType;
    }

    private void PlaceSelectedBuilding(Tile tile)
    {
        tile.isOccupied = true;

        ShowAndPositionSelectedBuilding(tile);
        selectedBuilding.GotPlaced();
        placedBuildings.Add(selectedBuilding);

        selectedBuildingCard = null;
        selectedBuilding = null;

        if (OnBuildingPlaced != null) OnBuildingPlaced();
    }

    private void ShowAndPositionSelectedBuilding(Tile tile)
    {
        selectedBuildingCard.copyTurretPrefab.SetActive(true);
        selectedBuildingCard.copyTurretPrefab.transform.position = tile.buildingPlacePosition;
        selectedBuilding.ShowRangePlane();
    }

    private void HideSelectedBuilding()
    {
        selectedBuildingCard.copyTurretPrefab.SetActive(false);
        selectedBuilding.HideRangePlane();

    }


    private void EnablePlacedBuildingsPlayerInteraction()
    {
        foreach (Building building in placedBuildings)
        {
            building.EnablePlayerInteraction();
        }
    }
    private void DisablePlacedBuildingsPlayerInteraction()
    {
        foreach (Building building in placedBuildings)
        {
            building.DisablePlayerInteraction();
        }
    }

}
