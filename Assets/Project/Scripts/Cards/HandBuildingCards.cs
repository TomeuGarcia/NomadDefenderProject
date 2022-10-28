using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandBuildingCards : MonoBehaviour
{
    [SerializeField] private BuildingPlacer buildingPlacer;

    [SerializeField] private List<BuildingCard> cards;


    private BuildingCard selectedCard;

    private bool AlreadyHasSelectedCard => selectedCard != null;


    private void Awake()
    {
        InitCardsInHand();
    }

    private void OnEnable()
    {
        BuildingCard.OnCardHovered += SetHoveredCard;
        BuildingCard.OnCardUnhovered += SetStandardCard;
        BuildingCard.OnCardSelected += CheckSelectCard;

        buildingPlacer.OnBuildingPlaced += RemoveCard;
    }

    private void OnDisable()
    {
        BuildingCard.OnCardHovered -= SetHoveredCard;
        BuildingCard.OnCardUnhovered -= SetStandardCard;
        BuildingCard.OnCardSelected -= CheckSelectCard;

        buildingPlacer.OnBuildingPlaced -= RemoveCard;
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
        for (int i = 0; i < cards.Count; ++i)
        {
            cards[i].transform.SetParent(transform);
            cards[i].transform.localPosition = Vector3.zero;
            cards[i].transform.position += transform.right * 1.1f * i;
            cards[i].transform.localRotation = Quaternion.identity;

            cards[i].InitPositions(cards[0].SelectedPosition);
        }
    }

    private void SetHoveredCard(BuildingCard card)
    {
        card.HoveredState();
        BuildingCard.OnCardHovered -= SetHoveredCard;
    }

    private void SetStandardCard(BuildingCard card)
    {
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
        if (true) // Check card's playCost !!!!!!
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


    private void RemoveCard() // TODO
    {
        // for now reset
        ResetAndSetStandardCard(selectedCard);
    }


}
