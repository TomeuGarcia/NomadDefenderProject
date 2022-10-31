using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandBuildingCards : MonoBehaviour
{
    [SerializeField] private AnimationCurve cardsHeightCurve;
    [SerializeField] private AnimationCurve cardsRotationCurve;

    [SerializeField] private CurrencyCounter currencyCounter;
    [SerializeField] private BuildingPlacer buildingPlacer;

    [SerializeField] private List<BuildingCard> cards;


    private BuildingCard selectedCard;
    private Vector3 selectedPosition;

    private Vector3 defaultHandPosition;
    private Vector3 hiddenHandPosition;
    private bool isHidden;

    private bool AlreadyHasSelectedCard => selectedCard != null;

    private BuildingCard hoveredCard;
    private bool IsHoveringCard => hoveredCard != null;


    private void OnValidate()
    {
        ComputeSelectedPosition();
        ComputeHiddenPosition();
    }

    private void Awake()
    {
        ComputeSelectedPosition();
        ComputeHiddenPosition();
        InitCardsInHand();

        HideHand();
    }

    private void OnEnable()
    {
        BuildingCard.OnCardHovered += SetHoveredCard;
        BuildingCard.OnCardUnhovered += SetStandardCard;
        BuildingCard.OnCardSelected += CheckSelectCard;

        buildingPlacer.OnBuildingPlaced += SubtractCurrencyAndRemoveCard;
    }

    private void OnDisable()
    {
        BuildingCard.OnCardHovered -= SetHoveredCard;
        BuildingCard.OnCardUnhovered -= SetStandardCard;
        BuildingCard.OnCardSelected -= CheckSelectCard;

        buildingPlacer.OnBuildingPlaced -= SubtractCurrencyAndRemoveCard;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && AlreadyHasSelectedCard)
        {
            ResetAndSetStandardCard(selectedCard);
        }
    }

    
    private void InitCardsInHand()
    {
        float displacementStep = 0.8f;
        Vector3 widthDisplacement = transform.right * displacementStep;

        float halfCardCount = cards.Count / 2f;
        float extraWidthDisplacement = cards.Count % 2 > 0 ? BuildingCard.halfWidth * displacementStep : 0f;
        Vector3 startDisplacement = (extraWidthDisplacement - halfCardCount) * transform.right;
               

        for (int i = 0; i < cards.Count; ++i)
        {
            float iRatio = (float)i / (cards.Count - 1);
            Vector3 heightDisplacement = transform.up * cardsHeightCurve.Evaluate(iRatio);
            Vector3 depthDisplacement = transform.forward * (-0.1f * iRatio);
            Quaternion rotation = Quaternion.AngleAxis(cardsRotationCurve.Evaluate(iRatio), Vector3.forward);


            cards[i].transform.SetParent(transform);
            cards[i].transform.localPosition = Vector3.zero;
            cards[i].transform.position += startDisplacement + (widthDisplacement * i) + heightDisplacement + depthDisplacement;
            cards[i].transform.localRotation = rotation;

            cards[i].InitPositions(selectedPosition);
            cards[i].DoOnCardIsDrawn();
        }
    }

    private void SetHoveredCard(BuildingCard card)
    {
        if (isHidden) ShowHand();

        hoveredCard = card;
        card.HoveredState();
        BuildingCard.OnCardHovered -= SetHoveredCard;
    }

    private void SetStandardCard(BuildingCard card)
    {
        if (!isHidden)
        {
            StartCoroutine("WaitToHideHand");
        }

        hoveredCard = null;
        card.StandardState();
        BuildingCard.OnCardHovered += SetHoveredCard;
    }

    private void ResetAndSetStandardCard(BuildingCard card)
    {
        selectedCard = null;
        SetStandardCard(card);

        buildingPlacer.DisablePlacing();
    }


    private void CheckSelectCard(BuildingCard card)
    {
        int cardCost = card.turretStats.playCost;

        if (currencyCounter.HasEnoughCurrency(cardCost)) // Check card's playCost !!!!!!
        {
            SetSelectedCard(card);            
        }
    }

    private void SetSelectedCard(BuildingCard card)
    {
        if (AlreadyHasSelectedCard) return;

        selectedCard = card;
        selectedCard.SelectedState();

        buildingPlacer.EnablePlacing(card);
    }


    private void SubtractCurrencyAndRemoveCard() // TODO
    {
        int cardCost = selectedCard.turretStats.playCost;
        currencyCounter.SubtractCurrency(cardCost);

        // TODO
        // for now reset
        selectedCard.DoOnCardIsDrawn();
        ResetAndSetStandardCard(selectedCard); 
    }



    private void ComputeSelectedPosition()
    {
        selectedPosition = (transform.right * (-3f)) + (transform.up * (2f)) + transform.position;
    }
    
    private void ComputeHiddenPosition()
    {
        defaultHandPosition = transform.position;
        hiddenHandPosition = transform.position + (-0.9f * transform.up);
    }

    private void HideHand()
    {
        isHidden = true;
        transform.position = hiddenHandPosition;
    }

    private void ShowHand()
    {
        isHidden = false;
        transform.position = defaultHandPosition;
    }


    private IEnumerator WaitToHideHand()
    {
        yield return new WaitForSeconds(0.05f);

        if (!IsHoveringCard)
        {
            HideHand();
        }
    }

}
