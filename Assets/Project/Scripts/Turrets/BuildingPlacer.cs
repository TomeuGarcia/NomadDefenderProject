using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    private BuildingCard selectedBuildingCard = null;
    private Building selectedBuilding = null;
    private List<Building> placedBuildings = new List<Building>();

    private Tile currentHoveredTile = null;
    private bool placingEnabled = false;
    private Coroutine dragAndDropCardCoroutine = null;


    public delegate void BuildingPlacerAction();
    public event BuildingPlacerAction OnBuildingPlaced;

    private bool SelectedBuildingIsBeingShown => selectedBuilding != null;


    private void OnEnable()
    {
        TDGameManager.OnEndGameResetPools += RemoveInteractions;
    }

    private void OnDisable()
    {
        TDGameManager.OnEndGameResetPools -= RemoveInteractions;
    }

    private void RemoveInteractions()
    {
        DisablePlacing();
        selectedBuildingCard = null;
        selectedBuilding = null;
    }


    public void EnablePlacing(BuildingCard selectedBuildingCard)
    {
        this.selectedBuildingCard = selectedBuildingCard;
        selectedBuilding = selectedBuildingCard.copyBuildingPrefab.GetComponent<Building>();

        Tile.OnTileUnhovered += HideBuildingPreview;
        Tile.OnTileHovered += ShowBuildingOnTilePreview;
        Tile.OnTileSelected += TryPlaceBuilding;

        DisablePlacedBuildingsPlayerInteraction();

        placingEnabled = true;
        //dragAndDropCardCoroutine = StartCoroutine(ClickDropCoroutine());
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

        if (dragAndDropCardCoroutine != null) StopCoroutine(dragAndDropCardCoroutine);
        placingEnabled = false;
        currentHoveredTile = null;
    }


    private IEnumerator ClickDropCoroutine()
    {                
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0) || !placingEnabled); 

        if (placingEnabled && currentHoveredTile != null)
        {
            TryPlaceBuilding(currentHoveredTile);
        }

        dragAndDropCardCoroutine = null;
    }


    private void ShowBuildingOnTilePreview(Tile tile)
    {
        currentHoveredTile = tile;
        ShowAndPositionSelectedBuilding(selectedBuildingCard, selectedBuilding, tile);
    }

    private void HideBuildingPreview()
    {
        currentHoveredTile = null;
        HideSelectedBuilding();
    }


    private void TryPlaceBuilding(Tile tile)
    {
        if (CanPlaceSelectedBuildingOnTile(tile))
        {
            PlaceSelectedBuilding(tile);
        }
        else
        {
            GameAudioManager.GetInstance().PlayError();
            selectedBuilding.PlayCanNOTBePlacedColorPunch();
        }
    }

    private bool CanPlaceSelectedBuildingOnTile(Tile tile)
    {
        return selectedBuilding.validTileType == tile.tileType;
    }

    private void PlaceSelectedBuilding(Tile tile)
    {
        tile.isOccupied = true;

        ShowAndPositionSelectedBuilding(selectedBuildingCard, selectedBuilding, tile);
        selectedBuilding.GotPlaced();
        placedBuildings.Add(selectedBuilding);


        if (selectedBuildingCard.cardBuildingType == BuildingCard.CardBuildingType.TURRET)
        {
            GameAudioManager.GetInstance().PlayTurretCardPlaced(((TurretBuildingCard)selectedBuildingCard).turretCardParts.turretPartBody.bodyType);
        }
        else
        {
            GameAudioManager.GetInstance().PlayTurretCardPlaced(TurretPartBody.BodyType.SENTRY);
        }


        selectedBuildingCard = null;
        selectedBuilding = null;

        if (OnBuildingPlaced != null) OnBuildingPlaced();
    }

    public void PlaceTutorialBuilding(BuildingCard buildingCard, Building building, Tile tile)
    {
        tile.isOccupied = true;

        ShowAndPositionSelectedBuilding(buildingCard, building, tile);
        building.GotPlaced();
        placedBuildings.Add(building);


        if (buildingCard.cardBuildingType == BuildingCard.CardBuildingType.TURRET)
        {
            GameAudioManager.GetInstance().PlayTurretCardPlaced(((TurretBuildingCard)buildingCard).turretCardParts.turretPartBody.bodyType);
        }
        else
        {
            GameAudioManager.GetInstance().PlayTurretCardPlaced(TurretPartBody.BodyType.SENTRY);
        }

        building.EnablePlayerInteraction();
    }



    private void ShowAndPositionSelectedBuilding(BuildingCard buildingCard, Building building, Tile tile)
    {
        building.gameObject.SetActive(true);
        building.transform.position = tile.buildingPlacePosition;
        building.ShowRangePlane();

        if (CanPlaceSelectedBuildingOnTile(tile))
        {
            building.SetPreviewCanBePlacedColor();
        }
        else
        {
            building.SetPreviewCanNOTBePlacedColor();
        }
    }

    private void HideSelectedBuilding()
    {
        selectedBuildingCard.copyBuildingPrefab.SetActive(false);
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
