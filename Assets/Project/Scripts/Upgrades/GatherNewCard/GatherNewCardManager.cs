using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatherNewCardManager : MonoBehaviour
{
    [Header("SCENE MANAGEMENT")]
    [SerializeField] private MapSceneNotifier mapSceneNotifier;

    [SerializeField] private CardsLibrary cardsLibrary;
    [SerializeField] private PartsLibrary partsLibrary;
    [SerializeField] private DeckCreator deckCreator;

    [SerializeField, Min(1)] private int numCards;
    private BuildingCard[] cards;
    private BuildingCard selectedCard;

    [SerializeField, Min(0f)] private float distanceBetweenCards = 1.2f;
    [SerializeField] private Transform cardHolder;

    // Animations
    Vector3[] startPositions;


    // Card Generation Constants and Variables
    private static int turretCardsLevel = 1;


    public delegate void GatherNewCardManagerAction();
    public static event GatherNewCardManagerAction OnCardGatherDone;



    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        cardsLibrary.InitSetup();

        // NodeEnums.HealthState.GREATLY_DAMAGED        --> 2 Turret Cards
        // NodeEnums.HealthState.SLIGHTLY_DAMAGED       --> 2-3 Turret Cards + 50% 0-1 Support Cards
        // NodeEnums.HealthState.UNDAMAGED              --> 2 Turret Cards + 1 Support Cards
        // NodeEnums.HealthState.UNDAMAGED + perfect    --> 2 Turret Cards + 1 Support Cards + Lvl3 Turrets + Reroll

        //// TODO read from data instead
        NodeEnums.ProgressionState progressionState = NodeEnums.ProgressionState.EARLY;
        NodeEnums.HealthState currentNodeHealthState = NodeEnums.HealthState.GREATLY_DAMAGED;
        bool lastBattleWasDefendedPerfectly = false;
        ////

        int numSupportCards = 0;

        if (currentNodeHealthState == NodeEnums.HealthState.GREATLY_DAMAGED)
        {
            numCards = 2;
            numSupportCards = 0;
        }
        else if (currentNodeHealthState == NodeEnums.HealthState.SLIGHTLY_DAMAGED)
        {
            numCards = 3;
            numSupportCards = Random.Range(0,2); // 50%
        }
        else if (currentNodeHealthState == NodeEnums.HealthState.UNDAMAGED)
        {
            numCards = 3;
            numSupportCards = 1;

            if (lastBattleWasDefendedPerfectly)
            {
                turretCardsLevel = TurretCardParts.MAX_CARD_LEVEL;
            }
        }



        cards = new BuildingCard[numCards];
        int numTurretCards = numCards - numSupportCards;


        CreateNTurretCards(numTurretCards, 1, lastBattleWasDefendedPerfectly, progressionState); // Get Random Turrets

        if (numSupportCards > 0)
        {
            CreateNSupportCards(numTurretCards, numSupportCards, GroupedCardParts.MIN_CARD_POWERFULNESS, GroupedCardParts.MAX_CARD_POWERFULNESS); // Get Random Supports
        }


        InitCardsPlacement();

        StartCoroutine(InitCardsAnimation());
    }


    /*
    private void CreateNTurretCards(int numTurretCards)
    {
        // Get Random Turrets
        TurretPartAttack[] attacks = partsLibrary.GetRandomTurretPartAttacks(numTurretCards);
        TurretPartBody[] bodies = partsLibrary.GetRandomTurretPartBodies(numTurretCards);
        TurretPartBase[] bases = partsLibrary.GetRandomTurretPartBases(numTurretCards);
        TurretPassiveBase[] passives = partsLibrary.GetRandomTurretPassiveBases(numTurretCards);


        for (int i = 0; i < numTurretCards; i++)
        {
            TurretBuildingCard turretCard = deckCreator.GetUninitializedNewTurretCard();

            TurretCardParts cardParts = ScriptableObject.CreateInstance("TurretCardParts") as TurretCardParts;
            cardParts.Init(1, attacks[i], bodies[i], bases[i], passives[i]);

            turretCard.ResetParts(cardParts);

            cards[i] = turretCard;
        }
    }
    */

    private void CreateNTurretCards(int totalAmount, int perfectAmount, bool perfect, NodeEnums.ProgressionState progressionState)
    {
        TurretCardParts[] turretCardPartsSet = cardsLibrary.GetRandomByProgressionTurretCardPartsSet(totalAmount, perfectAmount, perfect, progressionState); // Generate 
        turretCardPartsSet = cardsLibrary.GetConsumableTurretCardPartsSet(turretCardPartsSet); // Setup to create new cards

        for (int i = 0; i < totalAmount; i++)
        {
            TurretBuildingCard turretCard = deckCreator.GetUninitializedNewTurretCard();

            turretCardPartsSet[i].cardLevel = turretCardsLevel;

            turretCard.ResetParts(turretCardPartsSet[i]);

            cards[i] = turretCard;
        }
    }

    private void CreateNSupportCards(int startIndex, int numSupportCards, int minPowerfulness, int maxPowerfulness)
    {
        SupportCardParts[] supportCardPartsSet = cardsLibrary.GetRandomSupportCardPartsSet(numSupportCards, minPowerfulness, maxPowerfulness); // Generate 
        supportCardPartsSet = cardsLibrary.GetConsumableSupportCardPartsSet(supportCardPartsSet); // Setup to create new cards

        for (int i = 0; i < numSupportCards; i++)
        {
            SupportBuildingCard turretCard = deckCreator.GetUninitializedNewSupportCard();
            turretCard.ResetParts(supportCardPartsSet[i]);

            cards[i + startIndex] = turretCard;
        }
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

            cards[i].InitPositions(cards[i].transform.position, Vector3.zero);
        }
    }


    private void SetHoveredCard(BuildingCard card)
    {
        card.HoveredState();

        card.OnCardInfoSelected += SetCardShowInfo;

        foreach (BuildingCard itCard in cards)
        {
            itCard.OnCardHovered -= SetHoveredCard;
            itCard.OnCardUnhovered += SetStandardCard;
            itCard.OnCardSelected += SelectCard;
        }

        // Audio
        GameAudioManager.GetInstance().PlayCardHovered();
    }

    private void SetStandardCard(BuildingCard card)
    {
        card.StandardState();

        if (card.isShowingInfo)
        {
            SetCardHideInfo(card);
        }


        card.OnCardInfoSelected -= SetCardShowInfo;

        foreach (BuildingCard itCard in cards)
        {
            itCard.OnCardHovered += SetHoveredCard;
            itCard.OnCardUnhovered -= SetStandardCard;
            itCard.OnCardSelected -= SelectCard;
        }
    }

    private void SelectCard(BuildingCard card)
    {
        foreach (BuildingCard itCard in cards)
        {
            itCard.OnCardUnhovered -= SetStandardCard;
            itCard.OnCardSelected -= SelectCard;
        }

        selectedCard = card;

        if (selectedCard.isShowingInfo)
        {
            SetCardHideInfo(selectedCard);
        }
        selectedCard.OnCardInfoSelected -= SetCardShowInfo;


        if (selectedCard.cardBuildingType == BuildingCard.CardBuildingType.TURRET) 
        {
            deckCreator.AddNewTurretCardToDeck(selectedCard as TurretBuildingCard);
        }
        else if (selectedCard.cardBuildingType == BuildingCard.CardBuildingType.SUPPORT)
        {            
            deckCreator.AddNewSupportCardToDeck(selectedCard as SupportBuildingCard);
        }
        

        StartCoroutine(SelectCardAnimation());

        // Audio
        GameAudioManager.GetInstance().PlayCardSelected();
    }


    private void SetCardShowInfo(BuildingCard card)
    {
        card.ShowInfo();


        card.OnCardInfoSelected -= SetCardShowInfo;
        card.OnCardInfoSelected += SetCardHideInfo;
    }

    private void SetCardHideInfo(BuildingCard card)
    {
        card.HideInfo();


        card.OnCardInfoSelected += SetCardShowInfo;
        card.OnCardInfoSelected -= SetCardHideInfo;
    }



    private IEnumerator InitCardsAnimation()
    {
        Vector3[] endPositions = new Vector3[numCards];
        startPositions = new Vector3[numCards];

        Quaternion upsideDown = Quaternion.AngleAxis(180f, cards[0].RootCardTransform.forward);
        for (int i = 0; i < numCards; ++i)
        {
            endPositions[i] = cards[i].RootCardTransform.localPosition;
            startPositions[i] = cards[i].RootCardTransform.localPosition + (cards[i].RootCardTransform.forward * -3f);
            cards[i].RootCardTransform.localPosition = startPositions[i];
            cards[i].RootCardTransform.localRotation = upsideDown;
        }

        yield return new WaitForSeconds(1f); // start delay

        float delayBetweenCards = 0.2f;
        float moveDuration = 1.5f;
        for (int i = 0; i < numCards; ++i)
        {
            cards[i].RootCardTransform.DOLocalMove(endPositions[i], moveDuration);

            yield return new WaitForSeconds(delayBetweenCards);
        }
        yield return new WaitForSeconds(moveDuration);



        float rotationDuration = 0.5f;
        foreach (BuildingCard card in cards)
        {
            card.RootCardTransform.DOLocalRotate(Vector3.zero, rotationDuration);

            yield return new WaitForSeconds(delayBetweenCards);
        }
        yield return new WaitForSeconds(delayBetweenCards); // extra wait


        foreach (BuildingCard itCard in cards)
        {
            itCard.OnCardHovered += SetHoveredCard;
        }

        // scuffed fix, allow mouse interaction if already hovering the cards
        foreach (BuildingCard itCard in cards)
        {
            itCard.DisableMouseInteraction();
            yield return null;
            itCard.EnableMouseInteraction();
        }

    }

    private IEnumerator SelectCardAnimation()
    {
        float shakeDuration = 0.5f;
        selectedCard.RootCardTransform.DOPunchScale(Vector3.one * 0.2f, shakeDuration, 6);

        yield return new WaitForSeconds(shakeDuration);

        float moveDuration = 1.0f;
        float delayBetweenCards = 0.2f;
        for (int i = 0; i < numCards; ++i)
        {
            if (cards[i] == selectedCard) continue;

            cards[i].RootCardTransform.DOLocalMove(startPositions[i], moveDuration);

            yield return new WaitForSeconds(delayBetweenCards);
        }
        yield return new WaitForSeconds(moveDuration);


        float centerMoveDuration = 0.15f;
        Vector3 centerPos = cardHolder.transform.position;
        centerPos.y += 0.5f;
        centerPos += selectedCard.RootCardTransform.up * 0.4f;

        selectedCard.RootCardTransform.DOMove(centerPos, centerMoveDuration);
        yield return new WaitForSeconds(centerMoveDuration);

        // smooth move up
        centerMoveDuration = 1f - centerMoveDuration;
        centerPos += selectedCard.RootCardTransform.up * 0.05f;
        selectedCard.RootCardTransform.DOMove(centerPos, centerMoveDuration);
        yield return new WaitForSeconds(centerMoveDuration);


        Vector3 endPos = selectedCard.RootCardTransform.localPosition + (selectedCard.RootCardTransform.forward * 3f);
        selectedCard.RootCardTransform.DOLocalMove(endPos, moveDuration);
        selectedCard.RootCardTransform.DOLocalRotate(selectedCard.RootCardTransform.up * 15f, 1.5f);
        yield return new WaitForSeconds(moveDuration);


        //Debug.Log("FINISH");
        yield return new WaitForSeconds(1f);
        //if (OnCardGatherDone != null) OnCardGatherDone();

        mapSceneNotifier.InvokeOnSceneFinished();
    }



}
