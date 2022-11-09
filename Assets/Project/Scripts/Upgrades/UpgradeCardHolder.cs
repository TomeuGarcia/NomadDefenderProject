using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeCardHolder : MonoBehaviour
{
    [SerializeField] private AnimationCurve cardsHeightCurve;
    [SerializeField] private AnimationCurve cardsRotationCurve;

    [SerializeField] private Transform selectedTransform;

    // Serialize for now
    [SerializeField] private BuildingCard[] cards;

    private BuildingCard selectedCard = null;

    private bool AlreadyHasSelectedCard => selectedCard != null;


    private void Awake()
    {
        Init(cards);
    }

    private void OnEnable()
    {
        BuildingCard.OnCardHovered += SetHoveredCard;
        BuildingCard.OnCardSelected += SetSelectedCard;
    }

    private void OnDisable()
    {
        BuildingCard.OnCardHovered -= SetHoveredCard;
        BuildingCard.OnCardSelected -= SetSelectedCard;
    }


    public void Init(BuildingCard[] cards)
    {
        this.cards = cards;
        InitCardsInHand();
    }


    private void InitCardsInHand()
    {
        float cardCount = cards.Length;
        float displacementStep = Mathf.Min(0.6f / (cardCount * 0.2f), 0.6f);
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
            cards[i].transform.position += startDisplacement + widthDisplacement + heightDisplacement + depthDisplacement;
            cards[i].transform.localRotation = rotation;

            cards[i].InitPositions(selectedTransform.position);
        }
    }


    private void SetHoveredCard(BuildingCard card)
    {
        card.HoveredState();
        BuildingCard.OnCardHovered -= SetHoveredCard;
    }

    private void SetStandardCard(BuildingCard card)
    {
        //if (!isHidden)
        //{
        //    StartCoroutine("WaitToHideHand");
        //}

        //hoveredCard = null;
        card.StandardState();
        BuildingCard.OnCardHovered += SetHoveredCard;
    }

    private void SetSelectedCard(BuildingCard card)
    {
        if (AlreadyHasSelectedCard) return;

        selectedCard = card;
        selectedCard.SelectedState();
    }

}
