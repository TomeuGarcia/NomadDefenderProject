using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatherNewCardManager : MonoBehaviour
{
    [SerializeField] private PartsLibrary partsLibrary;
    [SerializeField] private DeckCreator deckCreator;

    [SerializeField, Min(1)] private int numCards;
    private BuildingCard[] cards;
    private BuildingCard selectedCard;

    [SerializeField, Min(0f)] private float distanceBetweenCards = 1.2f;
    [SerializeField] private Transform cardHolder;


    // Animations
    Vector3[] startPositions;


    public delegate void GatherNewCardManagerAction();
    public static event GatherNewCardManagerAction OnCardGatherDone;



    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        cards = new BuildingCard[numCards];

        TurretPartAttack[] attacks = partsLibrary.GetRandomTurretPartAttacks(numCards);
        TurretPartBody[] bodies = partsLibrary.GetRandomTurretPartBodies(numCards);
        TurretPartBase[] bases = partsLibrary.GetRandomTurretPartBases(numCards);

        for (int i = 0; i < numCards; i++)
        {
            cards[i] = deckCreator.GetUninitializedNewCard();
            cards[i].ResetParts(attacks[i], bodies[i], bases[i]);
        }
        InitCardsPlacement();

        StartCoroutine(InitCardsAnimation());
    }

    private void InitCardsPlacement()
    {
        float cardCount = cards.Length;
        float halfCardCount = (cardCount-1f) / 2f;
        Vector3 startDisplacement = (-halfCardCount * distanceBetweenCards) * cardHolder.right;

        for (int i = 0; i < cards.Length; ++i)
        {
            Vector3 widthDisplacement = transform.right * distanceBetweenCards * i;

            cards[i].transform.SetParent(cardHolder);
            cards[i].transform.localPosition = Vector3.zero;
            cards[i].transform.position += startDisplacement + widthDisplacement;
            cards[i].transform.localRotation = Quaternion.identity;

            cards[i].InitPositions(cards[i].transform.position);
        }
    }


    private void SetHoveredCard(BuildingCard card)
    {
        card.HoveredState();

        BuildingCard.OnCardHovered -= SetHoveredCard;
        BuildingCard.OnCardUnhovered += SetStandardCard;
        BuildingCard.OnCardSelected += SelectCard;

        // Audio
        GameAudioManager.GetInstance().PlayCardHovered();
    }

    private void SetStandardCard(BuildingCard card)
    {
        card.StandardState();

        BuildingCard.OnCardHovered += SetHoveredCard;
        BuildingCard.OnCardUnhovered -= SetStandardCard;
        BuildingCard.OnCardSelected -= SelectCard;
    }

    private void SelectCard(BuildingCard card)
    {
        BuildingCard.OnCardUnhovered -= SetStandardCard;
        BuildingCard.OnCardSelected -= SelectCard;

        selectedCard = card;

        deckCreator.AddNewCardToDeck(selectedCard);

        StartCoroutine(SelectCardAnimation());

        // Audio
        GameAudioManager.GetInstance().PlayCardSelected();
    }


    private IEnumerator InitCardsAnimation()
    {
        Vector3[] endPositions = new Vector3[numCards];
        startPositions = new Vector3[numCards];

        Quaternion upsideDown = Quaternion.AngleAxis(180f, cards[0].transform.forward);
        for (int i = 0; i < numCards; ++i)
        {
            endPositions[i] = cards[i].transform.localPosition;
            startPositions[i] = cards[i].transform.localPosition + (cards[i].transform.forward * -3f);
            cards[i].transform.localPosition = startPositions[i];
            cards[i].transform.localRotation = upsideDown;
        }

        yield return new WaitForSeconds(1f); // start delay

        float delayBetweenCards = 0.2f;
        float moveDuration = 1.5f;
        for (int i = 0; i < numCards; ++i)
        {
            cards[i].transform.DOLocalMove(endPositions[i], moveDuration);

            yield return new WaitForSeconds(delayBetweenCards);
        }
        yield return new WaitForSeconds(moveDuration);



        float rotationDuration = 0.5f;
        foreach (BuildingCard card in cards)
        {
            card.transform.DOLocalRotate(Vector3.zero, rotationDuration);

            yield return new WaitForSeconds(delayBetweenCards);
        }
        yield return new WaitForSeconds(rotationDuration);


        BuildingCard.OnCardHovered += SetHoveredCard;
    }

    private IEnumerator SelectCardAnimation()
    {
        float shakeDuration = 0.5f;
        selectedCard.transform.DOPunchScale(Vector3.one * 0.2f, shakeDuration, 6);

        yield return new WaitForSeconds(shakeDuration);

        float moveDuration = 1.0f;
        float delayBetweenCards = 0.2f;
        for (int i = 0; i < numCards; ++i)
        {
            if (cards[i] == selectedCard) continue;

            cards[i].transform.DOLocalMove(startPositions[i], moveDuration);

            yield return new WaitForSeconds(delayBetweenCards);
        }
        yield return new WaitForSeconds(moveDuration);


        float centerMoveDuration = 0.15f;
        Vector3 centerPos = cardHolder.transform.position;
        centerPos.y += 1f;
        centerPos += selectedCard.transform.up * 0.4f;

        selectedCard.transform.DOMove(centerPos, centerMoveDuration);
        yield return new WaitForSeconds(centerMoveDuration);

        // smooth move up
        centerMoveDuration = 1f - centerMoveDuration;
        centerPos += selectedCard.transform.up * 0.05f;
        selectedCard.transform.DOMove(centerPos, centerMoveDuration);
        yield return new WaitForSeconds(centerMoveDuration);


        Vector3 endPos = selectedCard.transform.localPosition + (selectedCard.transform.forward * 3f);
        selectedCard.transform.DOLocalMove(endPos, moveDuration);
        selectedCard.transform.DOLocalRotate(selectedCard.transform.up * 15f, 0.5f);
        yield return new WaitForSeconds(moveDuration);


        //Debug.Log("FINISH");
        yield return new WaitForSeconds(1f);
        if (OnCardGatherDone != null) OnCardGatherDone();
    }

}
