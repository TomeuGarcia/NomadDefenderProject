using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper;
using UnityEditor;
using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    [SerializeField] private CurrencyCounter _currencyCounter;
    
    private BuildingCard selectedBuildingCard = null;
    private Building selectedBuilding = null;
    private List<Building> placedBuildings = new List<Building>();
    public static int TotalPlacedBuildingsThisBattle { get; private set; } = 0;

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
    private HandBuildingCards _handBuildingCards;


    public delegate void BuildingPlacerAction();
    public event BuildingPlacerAction OnBuildingPlaced;
    public static Action<RangeBuilding> OnRangedBuildingPlaced;
    public static Action<TurretBuilding> OnTurretBuildingPlaced;
    public static Action<Building> OnBuildingUnplaced;
    public static event BuildingPlacerAction OnBuildingPlacedGlobal;

    public static event BuildingPlacerAction OnPlacingBuildingsDisabled;


    public delegate void BuildingPlacerAction2(BuildingCard buildingCard); 
    public event BuildingPlacerAction2 OnBuildingCantBePlaced;

    public delegate void BuildingPlacerAction3(Building building); 
    public static event BuildingPlacerAction3 OnPreviewTurretBuildingHoversTile;



    private bool SelectedBuildingIsBeingShown => selectedBuilding != null;

    
    private void OnEnable()
    {
        s_currentBuildingPlacer = this;
        TotalPlacedBuildingsThisBattle = 0;

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

    public void SetHand(HandBuildingCards handBuildingCards)
    {
        _handBuildingCards = handBuildingCards;
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


        InitEnoughCurrencyToPlaceBuilding();
        _currencyCounter.OnCurrencyAdded += OnCurrencyGainedWhilePlacingBuilding;
    }

    public void DisablePlacing()
    {
        if (selectedBuilding != null)
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

        _currencyCounter.OnCurrencyAdded -= OnCurrencyGainedWhilePlacingBuilding;

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
        int cardCost = selectedBuilding.BuildingCard.GetCardPlayCost();
        
        if (!_currencyCounter.HasEnoughCurrency(cardCost))
        {
            _currencyCounter.PlayNotEnoughCurrencyAnimation();
            selectedBuilding.BuildingCard.PlayCanNotBePlayedAnimation();
            selectedBuilding.PlayCanNOTBePlacedColorPunch();
        }
        else if (!CanPlaceBuildingOnTile(selectedBuilding, tile))
        {
            selectedBuilding.PlayCanNOTBePlacedColorPunch();
        }
        else
        {
            PlaceSelectedBuilding(tile, cardCost);
            return;
        }


        GameAudioManager.GetInstance().PlayError();
        StartCoroutine(DelayedDisablePlacing(0.4f));
        if (OnBuildingCantBePlaced != null) OnBuildingCantBePlaced(selectedBuildingCard);
    }

    private bool CanPlaceBuildingOnTile(Building building, Tile tile)
    {
        return building.validTileType == tile.tileType;
    }

    private void PlaceSelectedBuilding(Tile tile, int cardCost)
    {
        tile.isOccupied = true;

        ShowAndPositionSelectedBuilding(selectedBuildingCard, selectedBuilding, tile);
        selectedBuilding.GotPlaced(tile);
        AddPlacedBuilding(selectedBuilding);


        if (selectedBuildingCard.cardBuildingType == BuildingCard.CardBuildingType.TURRET)
        {
            GameAudioManager.GetInstance().PlayTurretCardPlaced(((TurretBuildingCard)selectedBuildingCard).CardParts.Body.bodyType);
        }
        else
        {
            GameAudioManager.GetInstance().PlayTurretCardPlaced(TurretPartBody.BodyType.SENTRY);
        }

        if (selectedBuilding is RangeBuilding placedRangedBuilding)
        {
            OnRangedBuildingPlaced?.Invoke(placedRangedBuilding); 
        }

        if (selectedBuilding is TurretBuilding placedTurretBuilding)
        {
            OnTurretBuildingPlaced?.Invoke(placedTurretBuilding);
        }
        
        selectedBuildingCard = null;
        selectedBuilding = null;

        _handBuildingCards.OnSelectedCardPlayed(cardCost);
        if (OnBuildingPlaced != null) OnBuildingPlaced();

    }

    public void PlaceTutorialBuilding(BuildingCard buildingCard, Building building, Tile tile)
    {
        tile.isOccupied = true;

        ShowAndPositionSelectedBuilding(buildingCard, building, tile);
        building.GotPlaced(tile);
        AddPlacedBuilding(building);


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

    private void AddPlacedBuilding(Building building)
    {
        placedBuildings.Add(building);
        ++TotalPlacedBuildingsThisBattle;
        
        AchievementDefinitions.HaveAmountOfBuildingsSimultaneously.Check(PlacedBuildingsCount);
        
        if (OnBuildingPlacedGlobal != null) OnBuildingPlacedGlobal();
    }


    private void ShowAndPositionSelectedBuilding(BuildingCard buildingCard, Building building, Tile tile)
    {
        building.gameObject.SetActive(true);
        building.transform.position = tile.buildingPlacePosition;
        building.ShowRangePlane();

        bool hasEnoughCurrencyToPlace =
            _currencyCounter.HasEnoughCurrency(selectedBuilding.BuildingCard.GetCardPlayCost());
        
        if (CanPlaceBuildingOnTile(building, tile))
        {
            if (hasEnoughCurrencyToPlace)
            {
                building.SetPreviewCanBePlacedColor();
            }
            else
            {
                building.SetPreviewCanNOTBePlacedColor(true);
            }
        }
        else
        {
            building.SetPreviewCanNOTBePlacedColor(false);
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
        
        OnBuildingUnplaced?.Invoke(building);
    }


    
    private bool _hadEnoughCurrencyToPlaceBuilding;

    private void InitEnoughCurrencyToPlaceBuilding()
    {
        _hadEnoughCurrencyToPlaceBuilding =
            _currencyCounter.HasEnoughCurrency(selectedBuilding.BuildingCard.GetCardPlayCost());

        if (!_hadEnoughCurrencyToPlaceBuilding)
        {
            selectedBuilding.ShowMissingCurrencyToPlace();
            UpdateSelectedBuildingMissingCurrency();
        }
        else
        {
            selectedBuilding.HideMissingCurrencyToPlace();
        }
    }
    
    private void OnCurrencyGainedWhilePlacingBuilding()
    {
        if (_hadEnoughCurrencyToPlaceBuilding) return;

        int playCost = selectedBuilding.BuildingCard.GetCardPlayCost();
        bool startedToHaveEnoughCurrency = !_hadEnoughCurrencyToPlaceBuilding &&
                                           _currencyCounter.HasEnoughCurrency(playCost);
        
        if (startedToHaveEnoughCurrency)
        {
            selectedBuilding.HideMissingCurrencyToPlace();
            selectedBuilding.BuildingCard.SetCanNotBePlayedPermanent(false);

            if (currentHoveredTile != null)
            {
                ShowAndPositionSelectedBuilding(selectedBuildingCard, selectedBuilding, currentHoveredTile);
            }
        }
        else
        {
            UpdateSelectedBuildingMissingCurrency();
        }
    }

    private void UpdateSelectedBuildingMissingCurrency()
    {
        int playCost = selectedBuilding.BuildingCard.GetCardPlayCost();
        int missingCurrency = playCost - _currencyCounter.CurrencyCount;
        selectedBuilding.UpdateMissingCurrencyToPlace(playCost);
    }
}
