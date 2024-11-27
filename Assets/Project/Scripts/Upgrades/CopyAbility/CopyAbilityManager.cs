
using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Upgrades.CopyAbility;
using UnityEngine;
using Random = UnityEngine.Random;

public class CopyAbilityManager : MonoBehaviour
{
    [Header("DEBUG")]
    [SerializeField] protected DecksLibrary _decksLibraryDEBUG;
    
    [Header("CAMERA")]
    [SerializeField] private Camera _mouseDragCamera;
    
    [Header("CARDS")]
    [SerializeField] private UpgradeCardHolderMultiplePlaceSpots _upgradeCardHolder;
    [SerializeField, Min(1)] private int _numberOfCards = 5;
    [SerializeField] private TurretBuildingCard _previewTurretCard;

    [Header("PLACERS")] 
    [SerializeField] private CardPlaceSpot _copyFromPlaceSpot;
    [SerializeField] private CardPlaceSpot _copyToPlaceSpot;


    [Header("DECK DATA")]
    [SerializeField] private CardDeckInUseData _deckInUse;
    [SerializeField] private Transform _cardSpawnHolder;
    private BuildingCard[] _deckCards;


    private TurretBuildingCard _copyFromCard;
    private TurretBuildingCard _copyToCard;
    
    
    private void OnEnable()
    {
        _copyFromPlaceSpot.OnCardPlaced += OnCopyFromCardPlaced;
        _copyFromPlaceSpot.OnCardRemoved += OnCopyFromCardRemoved;
        
        _copyToPlaceSpot.OnCardPlaced += OnCopyToCardPlaced;
        _copyToPlaceSpot.OnCardRemoved += OnCopyToCardRemoved;
    }
    private void OnDisable()
    {
        _copyFromPlaceSpot.OnCardPlaced -= OnCopyFromCardPlaced;
        _copyFromPlaceSpot.OnCardRemoved -= OnCopyFromCardRemoved;
        
        _copyToPlaceSpot.OnCardPlaced -= OnCopyToCardPlaced;
        _copyToPlaceSpot.OnCardRemoved -= OnCopyToCardRemoved;
    }

    
    
    private void Awake()
    {
        InitCameras();
        

        _decksLibraryDEBUG.InitGameDeck();
        _deckCards = _deckInUse.SpawnCurrentDeckBuildingCards(_cardSpawnHolder);
        _numberOfCards = Mathf.Min(_numberOfCards, _deckInUse.CurrentDeckContent.TurretCardsData.Length);
        
        _previewTurretCard.MotionEffectsController.DisableMotion();
        UpdatePreviewCard_MissingCards(_previewTurretCard, true, true);


        List<BuildingCard> randomCards =
            UpgradeRoomDeckCardsFilterer.GetRandomTurretCards(_deckCards, _numberOfCards, _upgradeCardHolder.CardsHolder);
        DisableRemainingcardsFromDeckCards(randomCards);
        
        _upgradeCardHolder.Init(randomCards.ToArray());
    }

    private void InitCameras()
    {
        ServiceLocator.GetInstance().CameraHelp.SetCardsCamera(_mouseDragCamera);
        BuildingCard.MouseDragCamera = _mouseDragCamera;
        CardPart.MouseDragCamera = _mouseDragCamera;

        CardTooltipDisplayManager.GetInstance().SetDisplayCamera(Camera.main);
    }

    private void DisableRemainingcardsFromDeckCards(List<BuildingCard> chosenCards)
    {
        for (int i = 0; i < _deckCards.Length; ++i)
        {
            if (!chosenCards.Contains(_deckCards[i]))
            {
                _deckCards[i].DisableMouseInteraction();
            }            
        }
    }

    private void UpdatePreviewCard_CardAndCardPart(TurretBuildingCard previewCard, TurretBuildingCard copyFromCard, TurretBuildingCard copyToCard)
    {
        previewCard.RootCardTransform.gameObject.SetActive(true);

        //...
    }

    private void UpdatePreviewCard_MissingCards(TurretBuildingCard previewCard, bool copyFromCardIsMissing, bool copyToCardIsMissing)
    {
        previewCard.RootCardTransform.gameObject.SetActive(false);

        //...
    }






    private void OnCopyFromCardPlaced(BuildingCard card)
    {
        _copyFromCard = card as TurretBuildingCard;
        EnableCopyFromButtons();
        if (AllCardsArePlaced())
        {
            EnableCardPreview();
        }
    }
    private void OnCopyFromCardRemoved(BuildingCard card)
    {
        DisableCopyFromButtons();
        DisableCardPreview();
    }
    
    private void OnCopyToCardPlaced(BuildingCard card)
    {
        _copyToCard = card as TurretBuildingCard;
        EnableCopyToButton();
        if (AllCardsArePlaced())
        {
            EnableCardPreview();
        }
    }
    private void OnCopyToCardRemoved(BuildingCard card)
    {
        DisableCopyToButton();
        DisableCardPreview();
    }



    private bool AllCardsArePlaced()
    {
        return _copyFromPlaceSpot.HasAnyPlacedCard() && _copyToPlaceSpot.HasAnyPlacedCard();
    }
    
    
    
    private void EnableCopyFromButtons()
    {
        //_copyFromCard
    }

    private void DisableCopyFromButtons()
    {
        
    }
    
    
    private void EnableCopyToButton()
    {
        
    }

    private void DisableCopyToButton()
    {
        
    }


    private void EnableCardPreview()
    {
        
    }
    
    private void DisableCardPreview()
    {
        
    }
    
}