using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Project.Scripts.Upgrades.CopyAbility;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;

public class UpgradeCardHolderMultiplePlaceSpots : MonoBehaviour
{
    [SerializeField] private AnimationCurve cardsHeightCurve;
    [SerializeField] private AnimationCurve cardsRotationCurve;

    public Transform CardsHolder => transform;
    [SerializeField, Min(0f)] private float distanceBetweenCards = 0.8f;

    private BuildingCard[] _cards;
    public BuildingCard[] Cards => _cards;

    [Header("CARD DRAG & DROP")]
    [SerializeField] private BoxCollider cardDragBoundsCollider;
    [SerializeField] private CardPlaceSpot[] _cardPlaceSpots;
    

    
    private float startDelay, duration, delayBetweenCards; // Animation variables

    private bool cardWasSelected = false;

    [HideInInspector] public bool canSelectCard = true;

    private bool cardsInteractionEnabled = true;


    private BuildingCard _currentlyGrabbingCard;

    private void Awake()
    {
        BuildingCard.DragStartBounds = cardDragBoundsCollider.bounds;
        BuildingCard.DragStartBounds.extents *= 2f;
        
    }
    

    public void Init(BuildingCard[] cards)
    {
        _cards = cards;
        InitCardsInHand();

        foreach (BuildingCard itCard in cards)
        {
            itCard.OnCardHovered += SetHoveredCard;
        }
    }


    private void InitCardsInHand()
    {
        float cardCount = _cards.Length;
        float displacementStep = Mathf.Min(distanceBetweenCards / (cardCount * 0.2f), distanceBetweenCards);
        float halfCardCount = cardCount / 2f;
        Vector3 startDisplacement = (-halfCardCount * displacementStep) * transform.right;

        float ratio = 0f;
        if (_cards.Length > 0) ratio = 1f / _cards.Length;

        for (int i = 0; i < _cards.Length; ++i)
        {
            float iRatio = ratio * (i + 0.5f);
            Vector3 widthDisplacement = transform.right * displacementStep * i;
            Vector3 heightDisplacement = transform.up * cardsHeightCurve.Evaluate(iRatio);
            Quaternion rotation = Quaternion.AngleAxis(cardsRotationCurve.Evaluate(iRatio), Vector3.forward);


            _cards[i].transform.SetParent(transform);
            _cards[i].transform.localPosition = Vector3.zero;
            _cards[i].transform.position += startDisplacement + widthDisplacement + heightDisplacement;
            _cards[i].transform.localRotation = rotation;

            _cards[i].InitPositions(transform.position, Vector3.zero, _cards[i].transform.position);

            _cards[i].hideInfoWhenSelected = false;
        }
    }


    private void EnableCardsInteraction()
    {
        cardsInteractionEnabled = true;
        foreach (BuildingCard card in _cards)
        {
            card.EnableMouseInteraction();
        }
    }
    private void DisableCardsInteraction()
    {
        cardsInteractionEnabled = false;
        foreach (BuildingCard card in _cards)
        {
            card.DisableMouseInteraction();
        }
    }




    private void SetHoveredCard(BuildingCard card)
    {
        card.HoveredState();

        HashSet<BuildingCard> notPlacedCards = CurrentlyNotPlacedCards();

        foreach (BuildingCard itCard in notPlacedCards)
        {
            itCard.OnCardHovered -= SetHoveredCard;
            itCard.OnCardUnhovered += SetStandardCard;
            itCard.OnCardSelected += SetSelectedCard;
        }
        
        // Audio
        GameAudioManager.GetInstance().PlayCardHovered();
    }

    private void SetStandardCard(BuildingCard card)
    {
        card.StandardState(cardWasSelected, duration: BuildingCard.toStandardTime);
        cardWasSelected = false; // reset


        HashSet<BuildingCard> notPlacedCards = CurrentlyNotPlacedCards();


        card.canDisplayInfoIfNotInteractable = false;
        foreach (BuildingCard itCard in notPlacedCards)
        {
            itCard.OnCardHovered += SetHoveredCard;
            itCard.OnCardUnhovered -= SetStandardCard;
            itCard.OnCardSelected -= SetSelectedCard;
            itCard.canDisplayInfoIfNotInteractable = false;
        }
    }

    private void SetSelectedCard(BuildingCard card)
    {
        if (!canSelectCard) return;
        if (AllPlaceSpotsHaveCards()) return;

        cardDragBoundsCollider.gameObject.SetActive(true);

        _currentlyGrabbingCard = card;
        _currentlyGrabbingCard.MotionEffectsController.DisableMotion();
        _currentlyGrabbingCard.SelectedState(true, repositionColliderOnEnd: true, enableInteractionOnEnd: true);

        _currentlyGrabbingCard.OnDragMouseUp += CheckSnapCardAtSelectedPosition;

        cardWasSelected = true;

        card.HideInfo();
        //if (selectedCard.isShowingInfo)
        //{
        //    SetCardHideInfo(selectedCard);            
        //}

        HashSet<BuildingCard> notPlacedCards = CurrentlyNotPlacedCards();

        
        _currentlyGrabbingCard.canDisplayInfoIfNotInteractable = true;
        foreach (BuildingCard itCard in notPlacedCards)
        {
            itCard.OnCardHovered -= SetHoveredCard;
            itCard.OnCardSelected -= SetSelectedCard;
            itCard.canDisplayInfoIfNotInteractable = false;
        }
        _currentlyGrabbingCard.OnCardSelectedNotHovered += RetrieveCardWhenPlaced;

        // Audio
        GameAudioManager.GetInstance().PlayCardSelected();
    }

    private void CheckSnapCardAtSelectedPosition(BuildingCard card)
    {
        card.OnDragMouseUp -= CheckSnapCardAtSelectedPosition;

        bool couldPlace = false;
        foreach (CardPlaceSpot cardPlaceSpot in _cardPlaceSpots)
        {
            if (cardPlaceSpot.CheckSnapCardAtSelectedPosition(card))
            {
                if (!cardPlaceSpot.AllowsPlacingCard(card))
                {
                    break;
                }   
               
                //Debug.Log("YEP drop here");
                cardPlaceSpot.SetPlacedCard(card);
                
                card.InitSelectedPosition(cardPlaceSpot.PlacePosition);
                card.GoToSelectedPosition();

                foreach (BuildingCard itCard in _cards)
                {
                    itCard.canDisplayInfoIfNotInteractable = true;
                }

                GameAudioManager.GetInstance().PlayCardPlacedOnUpgradeHolder();
                couldPlace = true;
                break;
            }
            
        }

        if (couldPlace)
        {
            _currentlyGrabbingCard = null;
            ReenableCardInteractionsAfterPlacingCard();
        }
        else
        {
            RetrieveCard(card);
            card.ReenableMouseInteraction();
        }

        cardDragBoundsCollider.gameObject.SetActive(false);
    }




    public void RetrieveCardWhenPlaced(BuildingCard card)
    {
        HashSet<BuildingCard> previouslyNotPlacedCards = CurrentlyNotPlacedCards();

        RetrieveCard(card);
        GameAudioManager.GetInstance().PlayCardRetreivedFromUpgradeHolder();

        
        foreach (BuildingCard itCard in previouslyNotPlacedCards)
        {
            itCard.OnCardHovered -= SetHoveredCard; // Undo extra subscription
        }
    }
    
    public void RetrieveCard(BuildingCard card)
    {
        RemovePlacedCard(card);
        
        card.OnCardSelectedNotHovered -= RetrieveCardWhenPlaced;
        card.MotionEffectsController.EnableMotion();
        SetStandardCard(card);
        card.HideInfo();

        
        _currentlyGrabbingCard = null;

        StartCoroutine(WaitToRepositionCollider(card));


        // Audio
        GameAudioManager.GetInstance().PlayCardHoverExit();
    }

    private IEnumerator WaitToRepositionCollider(BuildingCard card)
    {
        yield return new WaitForSeconds(BuildingCard.toStandardTime);
        card.RepositionColliderToCardTransform();
    }


    private bool AllPlaceSpotsHaveCards()
    {
        foreach (CardPlaceSpot cardPlaceSpot in _cardPlaceSpots)
        {
            if (!cardPlaceSpot.HasAnyPlacedCard())
            {
                return false;
            }
        }

        return true;
    }
    
    
    
    private void ReenableCardInteractionsAfterPlacingCard()
    {
        HashSet<BuildingCard> notPlacedCards = CurrentlyNotPlacedCards();

        foreach (BuildingCard itCard in notPlacedCards)
        {
            itCard.canDisplayInfoIfNotInteractable = false;
            itCard.OnCardHovered += SetHoveredCard;
            itCard.OnCardUnhovered -= SetStandardCard;
        }
    }

    
    
    public HashSet<BuildingCard> CurrentlyPlacedCards()
    {
        HashSet<BuildingCard> placedCards = new HashSet<BuildingCard>(_cardPlaceSpots.Length);
        
        foreach (CardPlaceSpot cardPlaceSpot in _cardPlaceSpots)
        {
            if (cardPlaceSpot.HasAnyPlacedCard())
            {
                placedCards.Add(cardPlaceSpot.PlacedCard);
            }
        }

        return placedCards;
    }
    
    
    public HashSet<BuildingCard> CurrentlyNotPlacedCards()
    {
        HashSet<BuildingCard> placedCards = CurrentlyPlacedCards();
        HashSet<BuildingCard> notPlacedCards = new HashSet<BuildingCard>();
        
        foreach (BuildingCard card in _cards)
        {
            if (!placedCards.Contains(card))
            {
                notPlacedCards.Add(card);
            }
        }

        return notPlacedCards;
    }


    private void RemovePlacedCard(BuildingCard card)
    {
        foreach (CardPlaceSpot cardPlaceSpot in _cardPlaceSpots)
        {
            if (cardPlaceSpot.HasPlacedCard(card))
            {
                cardPlaceSpot.RemovePlacedCard();
            }
        }
    }
}
