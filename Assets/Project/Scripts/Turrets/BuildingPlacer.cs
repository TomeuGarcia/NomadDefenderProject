using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    private BuildingCard selectedBuildingCard = null;
    private Building selectedBuilding = null;
    private List<Building> placedBuildings = new List<Building>();

    private static BuildingPlacer s_currentBuildingPlacer;
    public static Building[] GetCurrentPlacedBuildings()
    {
        return s_currentBuildingPlacer.placedBuildings.ToArray();
    }

    private Tile currentHoveredTile = null;
    private bool placingEnabled = false;
    private Coroutine dragAndDropCardCoroutine = null;

    private bool isDisablePlacingDelayed = false;

    public int PlacedBuildingsCount => placedBuildings.Count;


    public delegate void BuildingPlacerAction();
    public event BuildingPlacerAction OnBuildingPlaced;

    public static event BuildingPlacerAction OnPlacingBuildingsDisabled;


    public delegate void BuildingPlacerAction2(BuildingCard buildingCard); 
    public event BuildingPlacerAction2 OnBuildingCantBePlaced;

    public delegate void BuildingPlacerAction3(Building building); 
    public static event BuildingPlacerAction3 OnPreviewTurretBuildingHoversTile;



    private bool SelectedBuildingIsBeingShown => selectedBuilding != null;

    
    private void OnEnable()
    {
        s_currentBuildingPlacer = this;

        TDGameManager.OnEndGameResetPools += RemoveInteractions;
    }

    private void OnDisable()
    {
        s_currentBuildingPlacer = null;

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

        selectedBuilding.GotEnabledPlacing();

        Tile.OnTileUnhovered += HideBuildingPreview;
        Tile.OnTileHovered += ShowBuildingOnTilePreview;
        //Tile.OnTileSelected += TryPlaceBuilding;

        DisablePlacedBuildingsPlayerInteraction();

        placingEnabled = true;
        dragAndDropCardCoroutine = StartCoroutine(ClickDropCoroutine());
    }

    public void DisablePlacing()
    {
        if ( selectedBuilding != null)
        {
            selectedBuilding.GotDisabledPlacing();
        }

        Tile.OnTileUnhovered -= HideBuildingPreview;
        Tile.OnTileHovered -= ShowBuildingOnTilePreview;
        //Tile.OnTileSelected -= TryPlaceBuilding;

        if (SelectedBuildingIsBeingShown)
        {
            HideBuildingPreview();
        }

        EnablePlacedBuildingsPlayerInteraction();

        if (dragAndDropCardCoroutine != null) StopCoroutine(dragAndDropCardCoroutine);
        placingEnabled = false;
        currentHoveredTile = null;

        if (OnPlacingBuildingsDisabled != null) OnPlacingBuildingsDisabled();
    }
    private IEnumerator DelayedDisablePlacing(float duration)
    {
        isDisablePlacingDelayed = true;
        yield return new WaitForSeconds(duration);
        isDisablePlacingDelayed = false;
        DisablePlacing();
    }

    private IEnumerator ClickDropCoroutine()
    {                
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0) || !placingEnabled); 

        if (placingEnabled && currentHoveredTile != null)
        {
            TryPlaceBuilding(currentHoveredTile);
        }
        else
        {
            DisablePlacing();
            if (OnBuildingCantBePlaced != null) OnBuildingCantBePlaced(selectedBuildingCard);
        }

        dragAndDropCardCoroutine = null;
    }


    private void ShowBuildingOnTilePreview(Tile tile)
    {
        if (isDisablePlacingDelayed) return;

        currentHoveredTile = tile;
        ShowAndPositionSelectedBuilding(selectedBuildingCard, selectedBuilding, tile);

        selectedBuilding.GotMovedWhenPlacing();


        if (selectedBuildingCard.cardBuildingType == BuildingCard.CardBuildingType.TURRET)
        {
            if (OnPreviewTurretBuildingHoversTile != null) OnPreviewTurretBuildingHoversTile(selectedBuilding);
        }
    }

    private void HideBuildingPreview()
    {
        if (isDisablePlacingDelayed) return;

        currentHoveredTile = null;
        HideSelectedBuilding();
    }


    private void TryPlaceBuilding(Tile tile)
    {
        if (CanPlaceBuildingOnTile(selectedBuilding, tile))
        {
            PlaceSelectedBuilding(tile);
        }
        else
        {
            GameAudioManager.GetInstance().PlayError();
            selectedBuilding.PlayCanNOTBePlacedColorPunch();
            
            StartCoroutine(DelayedDisablePlacing(0.4f));
            if (OnBuildingCantBePlaced != null) OnBuildingCantBePlaced(selectedBuildingCard);
        }
    }

    private bool CanPlaceBuildingOnTile(Building building, Tile tile)
    {
        return building.validTileType == tile.tileType;
    }

    private void PlaceSelectedBuilding(Tile tile)
    {
        tile.isOccupied = true;

        ShowAndPositionSelectedBuilding(selectedBuildingCard, selectedBuilding, tile);
        selectedBuilding.GotPlaced(tile);
        placedBuildings.Add(selectedBuilding);


        if (selectedBuildingCard.cardBuildingType == BuildingCard.CardBuildingType.TURRET)
        {
            GameAudioManager.GetInstance().PlayTurretCardPlaced(((TurretBuildingCard)selectedBuildingCard).CardParts.Body.bodyType);
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
        building.GotPlaced(tile);
        placedBuildings.Add(building);


        if (buildingCard.cardBuildingType == BuildingCard.CardBuildingType.TURRET)
        {
            GameAudioManager.GetInstance().PlayTurretCardPlaced(((TurretBuildingCard)buildingCard).CardParts.Body.bodyType);
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

        if (CanPlaceBuildingOnTile(building, tile))
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

    

    public void UnplaceBuilding(Building building)
    {
        building.PlacedTile.isOccupied = false;
        building.GotUnplaced();

        placedBuildings.Remove(building);

        BuildingCard buildingCard = building.BuildingCard;

        if (buildingCard.cardBuildingType == BuildingCard.CardBuildingType.TURRET)
        {
            GameAudioManager.GetInstance().PlayTurretCardUnplaced(((TurretBuildingCard)buildingCard).CardParts.Body.bodyType);
        }
        else
        {
            GameAudioManager.GetInstance().PlayTurretCardUnplaced(TurretPartBody.BodyType.SENTRY);
        }
    }
}
