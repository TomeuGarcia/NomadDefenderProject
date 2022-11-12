using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeCardHolder : MonoBehaviour
{
    [SerializeField] private AnimationCurve cardsHeightCurve;
    [SerializeField] private AnimationCurve cardsRotationCurve;

    [SerializeField] private Transform selectedTransform;
    [SerializeField, Min(0f)] private float distanceBetweenCards = 0.8f;

    // Serialize for now
    [SerializeField] private BuildingCard[] cards;

    public BuildingCard selectedCard { get; private set; }

    public bool AlreadyHasSelectedCard => selectedCard != null;

    private bool canInteract;


    private void OnValidate()
    {
        Init(cards);
    }
    private void Awake()
    {
        Init(cards);
        BuildingCard.OnCardHovered += SetHoveredCard;
        canInteract = true;
    }


    public void Init(BuildingCard[] cards)
    {
        selectedCard = null;
        this.cards = cards;
        InitCardsInHand();
    }


    private void InitCardsInHand()
    {
        float cardCount = cards.Length;
        float displacementStep = Mathf.Min(distanceBetweenCards / (cardCount * 0.2f), distanceBetweenCards);
        float halfCardCount = cardCount / 2f;
        Vector3 startDisplacement = (-halfCardCount * displacementStep) * transform.right;

        float ratio = 0f;
        if (cards.Length > 0) ratio = 1f / cards.Length;

        for (int i = 0; i < cards.Length; ++i)
        {
            float iRatio = ratio * (i + 0.5f);
            Vector3 widthDisplacement = transform.right * displacementStep * i;
            Vector3 heightDisplacement = transform.up * cardsHeightCurve.Evaluate(iRatio);
            Vector3 depthDisplacement = transform.forward * (-0.1f * iRatio);
            Quaternion rotation = Quaternion.AngleAxis(cardsRotationCurve.Evaluate(iRatio), Vector3.forward);


            cards[i].transform.SetParent(transform);
            cards[i].transform.localPosition = Vector3.zero;
            cards[i].transform.position += startDisplacement + widthDisplacement + heightDisplacement;
            cards[i].transform.localRotation = rotation;

            cards[i].InitPositions(selectedTransform.position);
        }
    }


    private void SetHoveredCard(BuildingCard card)
    {
        card.HoveredState();

        BuildingCard.OnCardHovered -= SetHoveredCard;
        BuildingCard.OnCardUnhovered += SetStandardCard;
        BuildingCard.OnCardSelected += SetSelectedCard;
    }

    private void SetStandardCard(BuildingCard card)
    {
        //if (!isHidden)
        //{
        //    StartCoroutine("WaitToHideHand");
        //}

        //hoveredCard = null;
        card.StandardState();
  
        if (canInteract) BuildingCard.OnCardHovered += SetHoveredCard;
        BuildingCard.OnCardUnhovered -= SetStandardCard;
        BuildingCard.OnCardSelected -= SetSelectedCard;
    }

    private void SetSelectedCard(BuildingCard card)
    {
        if (AlreadyHasSelectedCard) return;

        selectedCard = card;
        selectedCard.SelectedState();

        BuildingCard.OnCardHovered -= SetHoveredCard;
        BuildingCard.OnCardSelected -= SetSelectedCard;
        selectedCard.OnCardSelectedNotHovered += RetrieveCard;

    }


    public void RetrieveCard(BuildingCard card)
    {
        selectedCard.OnCardSelectedNotHovered -= RetrieveCard;
        SetStandardCard(card);

        selectedCard = null;
    }

    public void StopInteractions()
    {
        canInteract = false;
    }

}
