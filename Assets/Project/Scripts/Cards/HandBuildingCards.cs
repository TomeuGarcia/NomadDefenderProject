using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandBuildingCards : MonoBehaviour
{
    [SerializeField] private BuildingPlacer buildingPlacer;


    private BuildingCard selectedCard;

    private bool AlreadyHasSelectedCard => selectedCard != null;



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



    private void SetHoveredCard(BuildingCard card)
    {
        card.HoveredState();
    }

    private void SetStandardCard(BuildingCard card)
    {
        card.StandardState();
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
