using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPartHolder : MonoBehaviour
{
    [SerializeField] private Transform selectedTransform;
    [SerializeField, Min(0f)] private float distanceBetweenCards = 0.8f;

    // Serialize for now
    [SerializeField] private CardPart[] cardParts;

    public CardPart selectedCardPart { get; private set; }

    public bool AlreadyHasSelectedPart => selectedCardPart != null;


    private void OnValidate()
    {
        Init(cardParts);
    }
    private void Awake()
    {
        Init(cardParts);
        CardPart.OnCardHovered += SetHoveredCard;
    }


    public void Init(CardPart[] cardsParts)
    {
        selectedCardPart = null;
        this.cardParts = cardsParts;
        InitCardsInHand();
    }


    private void InitCardsInHand()
    {
        float cardCount = cardParts.Length;
        float displacementStep = Mathf.Min(distanceBetweenCards / (cardCount * 0.2f), distanceBetweenCards);
        float halfCardCount = cardCount / 2f;
        Vector3 startDisplacement = (-halfCardCount * displacementStep) * transform.right;

        float ratio = 0f;
        if (cardParts.Length > 0) ratio = 1f / cardParts.Length;

        for (int i = 0; i < cardParts.Length; ++i)
        {
            float iRatio = ratio * (i + 0.5f);
            Vector3 widthDisplacement = transform.right * displacementStep * i;


            cardParts[i].transform.SetParent(transform);
            cardParts[i].transform.localPosition = Vector3.zero;
            cardParts[i].transform.position += startDisplacement + widthDisplacement;

            cardParts[i].InitPositions(selectedTransform.position);
        }
    }


    private void SetHoveredCard(CardPart card)
    {
        card.HoveredState();

        CardPart.OnCardHovered -= SetHoveredCard;
        CardPart.OnCardUnhovered += SetStandardCard;
        CardPart.OnCardSelected += SetSelectedCard;
    }

    private void SetStandardCard(CardPart card)
    {
        card.StandardState();

        CardPart.OnCardHovered += SetHoveredCard;
        CardPart.OnCardUnhovered -= SetStandardCard;
        CardPart.OnCardSelected -= SetSelectedCard;
    }

    private void SetSelectedCard(CardPart card)
    {
        if (AlreadyHasSelectedPart) return;

        selectedCardPart = card;
        selectedCardPart.SelectedState();

        CardPart.OnCardHovered -= SetHoveredCard;
        CardPart.OnCardSelected -= SetSelectedCard;
        selectedCardPart.OnCardSelectedNotHovered += RetrieveCard;
    }


    private void RetrieveCard(CardPart card)
    {
        selectedCardPart.OnCardSelectedNotHovered -= RetrieveCard;
        SetStandardCard(card);

        selectedCardPart = null;
    }
}
