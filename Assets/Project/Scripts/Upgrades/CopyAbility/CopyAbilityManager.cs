
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Upgrades.CopyAbility;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class CopyAbilityManager : MonoBehaviour
{
    [Header("CAMERA")]
    [SerializeField] private Camera _mouseDragCamera;
    
    [Header("SCENE MANAGEMENT")]
    [SerializeField] private MapSceneNotifier _mapSceneNotifier;
    
    [Header("CARDS")]
    [SerializeField, Min(1)] private int _numberOfCards = 5;
    [SerializeField] private UpgradeCardHolderMultiplePlaceSpots _upgradeCardHolder;
    [SerializeField] private TurretBuildingCard _previewTurretCard;
    [SerializeField] private CardMotionConfig _cardsMotionConfig;

    
    [Header("UPDATE CARD PLAY COST")]
    [SerializeField] private CardUpgradeTurretPlayCostConfig _playCostsConfig;
    
    [Header("PLACERS")] 
    [SerializeField] private CardPlaceSpot _copyFromPlaceSpot;
    [SerializeField] private CardPlaceSpot _copyToPlaceSpot;


    [Header("DECK DATA")]
    [SerializeField] private CardDeckInUseData _deckInUse;
    [SerializeField] private Transform _cardSpawnHolder;
    private BuildingCard[] _deckCards;

    [Header("BUTTONS")] 
    [SerializeField] private AbilityManagerConfirmButton _confirmButton;
    [SerializeField] private AbilityManagerCopyFromButton[] _copyFromButtons;
    private AbilityManagerCopyFromButton _selectedCopyFromButton;

    [Header("ANIMATIONS")] 
    [SerializeField] private CopyAbilityManagerTextsAnimator _textsAnimator;
    [SerializeField] private CopyAbilityManagerTutorizationAnimator _tutorizationAnimator;
    [SerializeField] private CopyAbilityManagerCardHandAnimator _cardHandAnimator;
    [SerializeField] private CopyAbilityManagerMachineAnimator _machineAnimator;
    

    private TurretBuildingCard _copyFromCard;
    private TurretBuildingCard _copyToCard;

    private CopyFromCardAllowPlaceCondition _copyFromCardAllowPlaceCondition = new ();
    
    
    private void OnEnable()
    {
        _copyFromPlaceSpot.OnCardPlaced += OnCopyFromCardPlaced;
        _copyFromPlaceSpot.OnCardRemoved += OnCopyFromCardRemoved;
        
        _copyToPlaceSpot.OnCardPlaced += OnCopyToCardPlaced;
        _copyToPlaceSpot.OnCardRemoved += OnCopyToCardRemoved;


        _confirmButton.OnClicked += OnConfirmButtonClicked;
        foreach (AbilityManagerCopyFromButton copyFromButton in _copyFromButtons)
        {
            copyFromButton.OnClicked += OnCopyFromButtonClicked;
        }

        _copyFromCardAllowPlaceCondition.OnTriedPlacingInvalidCard += OnNotValidCopyFromCard;
    }
    
    private void OnDisable()
    {
        _copyFromPlaceSpot.OnCardPlaced -= OnCopyFromCardPlaced;
        _copyFromPlaceSpot.OnCardRemoved -= OnCopyFromCardRemoved;
        
        _copyToPlaceSpot.OnCardPlaced -= OnCopyToCardPlaced;
        _copyToPlaceSpot.OnCardRemoved -= OnCopyToCardRemoved;
        
        
        _confirmButton.OnClicked -= OnConfirmButtonClicked;
        foreach (AbilityManagerCopyFromButton copyFromButton in _copyFromButtons)
        {
            copyFromButton.OnClicked -= OnCopyFromButtonClicked;
        }
        
        _copyFromCardAllowPlaceCondition.OnTriedPlacingInvalidCard -= OnNotValidCopyFromCard;
    }

    
    
    private void Awake()
    {
        InitCameras();
        

        _cardsMotionConfig.SetUpgradeSceneMode();
        _deckCards = _deckInUse.SpawnCurrentDeckBuildingCards(_cardSpawnHolder);
        _numberOfCards = Mathf.Min(_numberOfCards, _deckInUse.CurrentDeckContent.TurretCardsData.Length);
        
        _previewTurretCard.MotionEffectsController.DisableMotion();
        DisableCardPreview();


        BuildingCard[] randomCards = UpgradeRoomDeckCardsFilterer.GetRandomTurretCardsWithAtLeast1Ability(
                _deckCards, _numberOfCards, _upgradeCardHolder.CardsHolder).ToArray();
        DisableRemainingCardsFromDeckCards(randomCards);
        
        _upgradeCardHolder.Init(randomCards);
        _copyFromPlaceSpot.AllowPlaceCondition = _copyFromCardAllowPlaceCondition;

        StartCoroutine(PlayInitLogic(randomCards));
    }

    private void InitCameras()
    {
        ServiceLocator.GetInstance().CameraHelp.SetCardsCamera(_mouseDragCamera);
        BuildingCard.MouseDragCamera = _mouseDragCamera;
        CardPart.MouseDragCamera = _mouseDragCamera;

        CardTooltipDisplayManager.GetInstance().SetDisplayCamera(Camera.main);
    }

    private void DisableRemainingCardsFromDeckCards(BuildingCard[] chosenCards)
    {
        for (int i = 0; i < _deckCards.Length; ++i)
        {
            if (!chosenCards.Contains(_deckCards[i]))
            {
                _deckCards[i].DisableMouseInteraction();
            }            
        }
    }


    private IEnumerator PlayInitLogic(BuildingCard[] cards)
    {
        _cardHandAnimator.InitShowCards(cards);
        yield return StartCoroutine(_machineAnimator.PlayInitAppearAnimation_BeforeCardsAppearing());
        yield return StartCoroutine(_cardHandAnimator.PlayInitShowCards());
        StartCoroutine(_textsAnimator.PlayInitText());
        yield return StartCoroutine(_machineAnimator.PlayInitAppearAnimation_AfterCardsAppearing());
    }
    


    private void OnNotValidCopyFromCard()
    {
        GameAudioManager.GetInstance().PlayError();
        StartCoroutine(_tutorizationAnimator.PlayNotValidCopyFromCardWasPlaced());
    }
    
    

    private void OnCopyFromCardPlaced(BuildingCard card)
    {
        _copyFromCard = card as TurretBuildingCard;
        EnableCopyFromButtons();
    }
    private void OnCopyFromCardRemoved(BuildingCard card)
    {
        if (AbilityFromCopyIsSelected())
        {
            DisableCopyToButton();
        }
        
        DisableCopyFromButtons();
        DisableCardPreview();
        _copyFromCard = null;
    }
    
    private void OnCopyToCardPlaced(BuildingCard card)
    {
        _copyToCard = card as TurretBuildingCard;
        if (AllCardsArePlaced() && AbilityFromCopyIsSelected())
        {
            EnableCopyToButton();
            EnableCardPreview();
        }
    }
    private void OnCopyToCardRemoved(BuildingCard card)
    {
        if (_copyFromPlaceSpot.HasAnyPlacedCard())
        {
            DisableCopyToButton();
            DisableCardPreview();
        }
        
        _copyToCard = null;
    }


    
    private bool AllCardsArePlaced()
    {
        return _copyFromPlaceSpot.HasAnyPlacedCard() && _copyToPlaceSpot.HasAnyPlacedCard();
    }

    private bool AbilityFromCopyIsSelected()
    {
        return _selectedCopyFromButton != null;
    }
    
    
    private void EnableCopyFromButtons()
    {
        List<ATurretPassiveAbility> passiveAbilities =
            _copyFromCard.CardData.PassiveAbilitiesController.PassiveAbilities;

        for (int i = 0; i < passiveAbilities.Count; ++i)
        {
            _copyFromButtons[i].SetEnabled(passiveAbilities[i].OriginalModel);
        }
    }

    private void DisableCopyFromButtons()
    {
        List<ATurretPassiveAbility> passiveAbilities =
            _copyFromCard.CardData.PassiveAbilitiesController.PassiveAbilities;
        
        for (int i = 0; i < passiveAbilities.Count; ++i)
        {
            _copyFromButtons[i].SetDisabled();
        }

        _selectedCopyFromButton = null;
    }
    
    
    private void EnableCopyToButton()
    {
        _confirmButton.SetEnabled();
    }

    private void DisableCopyToButton()
    {
        _confirmButton.SetDisabled();
    }


    private void EnableCardPreview()
    {
        _previewTurretCard.RootCardTransform.gameObject.SetActive(true);
        _previewTurretCard.MotionEffectsController.DisableMotion();
        

        TurretPartProjectileDataModel turretPartAttack = null;
        TurretPartBody turretPartBody = null;
        ATurretPassiveAbilityDataModel turretPassive = _selectedCopyFromButton.AbilityDataModel;
        CardPartBonusStats cardPartBonusStats = null;
        
        _previewTurretCard.PreviewChangeVisuals(turretPartAttack, turretPartBody, turretPassive, cardPartBonusStats,
            _copyToCard, CardPartReplaceManager.PartType.BASE, _playCostsConfig);
    }
    
    private void DisableCardPreview()
    {
        _previewTurretCard.RootCardTransform.gameObject.SetActive(false);
    }







    private void OnCopyFromButtonClicked(AbilityManagerCopyFromButton selectedCopyFromButton)
    {
        _selectedCopyFromButton?.SetNotSelected();

        _selectedCopyFromButton = selectedCopyFromButton;
        _selectedCopyFromButton.SetSelected();
        
        if (AllCardsArePlaced())
        {
            EnableCardPreview();
            EnableCopyToButton();
        }
    }
    
    private void OnConfirmButtonClicked(AbilityManagerConfirmButton confirmButton)
    {
        FinalDisableInteractions();
        DisableCardPreview();
        StartCoroutine(PlayConfirmLogic());
    }

    private void FinalDisableInteractions()
    {
        foreach (BuildingCard card in _deckCards)
        {
            card.DisableMouseInteraction();
        }
        
        _confirmButton.SetFinalDisabled();
        foreach (AbilityManagerCopyFromButton copyFromButton in _copyFromButtons)
        {
            copyFromButton.SetFinalDisabled();
        }
    }

    private IEnumerator PlayConfirmLogic()
    {
        _textsAnimator.ClearInitText();
        yield return StartCoroutine(_machineAnimator.PlayConfirmAnimation_BeforeModifyingCard());
        StartCoroutine(_textsAnimator.PlayCompleteText());
        yield return StartCoroutine(ModifyCopyToCard());
        yield return StartCoroutine(_machineAnimator.PlayConfirmAnimation_AfterModifyingCard());
        yield return StartCoroutine(_cardHandAnimator.PlayFinishHideCards(
            _upgradeCardHolder.Cards,
            _upgradeCardHolder.CurrentlyPlacedCards().ToArray()));
        FinishScene();
    }

    private IEnumerator ModifyCopyToCard()
    {
        _copyToCard.AddNewPassive(_selectedCopyFromButton.AbilityDataModel);
        _copyToCard.IncrementCardLevel(1, false);

        bool replacedWithSamePart = _copyToCard.ReplacedWithSamePart;
        _copyToCard.PlayUpdatePlayCostAnimation(_playCostsConfig.ComputeCardPlayCostIncrement(!replacedWithSamePart, _copyToCard));
        _copyToCard.PlayLevelUpAnimation();

        yield return new WaitForSeconds(2.0f); // Extra wait for Level Up animation

        if (replacedWithSamePart)
        {
            AchievementDefinitions.NegativePlayCostCard.Check(_copyToCard.CardData.PlayCost);
            yield return new WaitForSeconds(1.5f); // Extra wait for Play Cost animation
        }
    }

    private void FinishScene()
    {
        _mapSceneNotifier.InvokeOnSceneFinished();
    }
}