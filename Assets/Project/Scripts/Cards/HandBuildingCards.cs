using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class HandBuildingCards : MonoBehaviour
{
    [SerializeField] private Transform cardHolder;
    [SerializeField] private Transform buildingsHolder;

    [SerializeField] private AnimationCurve cardsHeightCurve;
    [SerializeField] private AnimationCurve cardsRotationCurve;

    [SerializeField] private CurrencyCounter currencyCounter;
    [SerializeField] private BuildingPlacer buildingPlacer;
    private int initialRedraws = 3;
    int redrawsLeft;
    private List<BuildingCard> cards;

    private BuildingCard selectedCard;
    private Vector3 selectedPosition;

    private Vector3 initHandPosition;
    private Vector3 defaultHandPosition;
    private Vector3 hiddenHandPosition;
    private Vector3 hiddenDisplacement;
    private bool isHidden;

    private bool AlreadyHasSelectedCard => selectedCard != null;

    private BuildingCard hoveredCard;
    private bool IsHoveringCard => hoveredCard != null;

    public Transform HandTransform => cardHolder;



    public delegate void HandAction();
    public static event HandAction OnQueryDrawCard;
    public static event HandAction OnQueryRedrawCard;
    public static event HandAction OnFinishRedrawing;
    public static event HandAction OnCardPlayed;
    public delegate void CardAction( BuildingCard card);
    public static event CardAction ReturnCardToDeck;
    public event HandAction OnCanAddCard;




    private void OnValidate()
    {
        SetInitHandPosition();
        ComputeSelectedPosition();
        ComputeHiddenPosition();
    }

    private void Awake()
    {
        cards = new List<BuildingCard>();
        redrawsLeft = initialRedraws;
    }

    public void Init()
    {
        SetInitHandPosition();
        ComputeSelectedPosition();
        ComputeHiddenPosition();

        InitCardsInHandForRedraw();

        for (int i = 0; i < cards.Count; ++i)
        {
            cards[i].CreateCopyBuildingPrefab(buildingsHolder, currencyCounter);
        }

        //HideHand(false);

        CheckCardsCost();
    }

    private void OnEnable()
    {
        //redrawsLeft = initialRedraws;
        //foreach (BuildingCard itCard in cards)
        //{
        //    itCard.OnCardHovered += SetHoveredCard;
        //    itCard.OnCardUnhovered += SetStandardCard;
        //    itCard.OnCardSelected += Redraw;
        //}
        //FinishedRedrawing();
    }
    private void OnDisable()
    {
        foreach (BuildingCard itCard in cards)
        {
            itCard.OnCardHovered -= SetHoveredCard;
            itCard.OnCardUnhovered -= SetStandardCard;
            itCard.OnCardSelected -= CheckSelectCard;
        }

        buildingPlacer.OnBuildingPlaced -= OnSelectedCardPlayed;
        currencyCounter.OnCurrencyAdded -= CheckCardsCost;
        currencyCounter.OnCurrencySpent -= CheckCardsCost;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && AlreadyHasSelectedCard)
        {
            ResetAndSetStandardCard(selectedCard);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (OnQueryDrawCard != null) OnQueryDrawCard();
        }
    }

    
    public void InitCardsInHand()
    {
        Debug.Log("InitCardsInHand");

        float cardCount = cards.Count;
        float displacementStep = Mathf.Min(0.65f / (cardCount * 0.2f), 0.65f);
        float halfCardCount = cards.Count / 2f;
        Vector3 startDisplacement = (-halfCardCount * displacementStep) * HandTransform.right;

        float ratio = cards.Count == 0 ? 0f : 1f / cards.Count;

        for (int i = 0; i < cards.Count; ++i)
        {
            float iRatio = ratio * (i + 0.5f);
            Vector3 widthDisplacement = HandTransform.right * displacementStep * i;
            Vector3 heightDisplacement = HandTransform.up * cardsHeightCurve.Evaluate(iRatio);
            Vector3 depthDisplacement = HandTransform.forward * (-0.2f * iRatio);
            Quaternion rotation = Quaternion.AngleAxis(cardsRotationCurve.Evaluate(iRatio), Vector3.forward);

            Vector3 finalPosition = initHandPosition + startDisplacement + widthDisplacement + heightDisplacement + depthDisplacement;

            BuildingCard card = cards[i];


            card.RootCardTransform.SetParent(HandTransform);

            //card.CardTransform.localPosition = Vector3.zero;
            //card.CardTransform.position = finalPosition;
            //card.CardTransform.localRotation = rotation;
            //card.InitPositions(selectedPosition, hiddenDisplacement);

            float repositionDuration = 0.3f;
            card.StartRepositioning(finalPosition, repositionDuration);
            card.RootCardTransform.DOLocalRotate(rotation.eulerAngles, repositionDuration)
                .OnComplete(() => SetupCard(card, selectedPosition, hiddenDisplacement, i));
        }
        
    }
    private void SetupCard(BuildingCard card, Vector3 selectedPosition, Vector3 hiddenDisplacement, int index)
    {
        card.InitPositions(selectedPosition, hiddenDisplacement);

        if (card.cardLocation != BuildingCard.CardLocation.HAND)
        {
            card.OnCardHovered += SetHoveredCard;
            card.OnCardUnhovered += SetStandardCard;
            card.OnCardSelected += CheckSelectCard;

            card.OnCardInfoSelected += SetCardShowInfo;


            card.cardLocation = BuildingCard.CardLocation.HAND;
        }

        if (index == cards.Count - 1)
        {
            HideHand(true);
        }
    }

    public void InitCardsInHandForRedraw()
    {
        Debug.Log("InitCardsInHandFirstDraw");
        float cardCount = cards.Count;
        float displacementStep = Mathf.Min(0.65f / (cardCount * 0.2f), 0.65f);
        float halfCardCount = cards.Count / 2f;
        Vector3 startDisplacement = (-halfCardCount * displacementStep) * HandTransform.right;

        float ratio = cards.Count == 0 ? 0f : 1f / cards.Count;

        for (int i = 0; i < cards.Count; ++i)
        {
            float iRatio = ratio * (i + 0.5f);
            Vector3 widthDisplacement = HandTransform.right * displacementStep * i;
            Vector3 heightDisplacement = HandTransform.up * cardsHeightCurve.Evaluate(iRatio);
            Vector3 depthDisplacement = HandTransform.forward * (-0.2f * iRatio);
            Quaternion rotation = Quaternion.AngleAxis(cardsRotationCurve.Evaluate(iRatio), Vector3.forward);

            Vector3 finalPosition = initHandPosition + startDisplacement + widthDisplacement + heightDisplacement + depthDisplacement;

            BuildingCard card = cards[i];


            card.RootCardTransform.SetParent(HandTransform);

            //card.CardTransform.localPosition = Vector3.zero;
            //card.CardTransform.position = finalPosition;
            //card.CardTransform.localRotation = rotation;
            //card.InitPositions(selectedPosition, hiddenDisplacement);

            float repositionDuration = 0.3f;
            card.StartRepositioning(finalPosition, repositionDuration);
            card.RootCardTransform.DOLocalRotate(rotation.eulerAngles, repositionDuration)
                .OnComplete(() => SetupCardForRedraw(card, selectedPosition, hiddenDisplacement, i));
        }

    }
    private void SetupCardForRedraw(BuildingCard card, Vector3 selectedPosition, Vector3 hiddenDisplacement, int index)
    {
        card.InitPositions(selectedPosition, hiddenDisplacement);

        if (card.cardLocation != BuildingCard.CardLocation.HAND)
        {
            card.OnCardHovered += SetHoveredCard;
            card.OnCardUnhovered += SetStandardCard;
            card.OnCardSelected += Redraw;

            card.OnCardInfoSelected += SetCardShowInfo;


            card.cardLocation = BuildingCard.CardLocation.HAND;
        }

        if (index == cards.Count - 1)
        {
            HideHand(true);
        }
    }
    public void FinishedRedrawing()
    {
        foreach (BuildingCard itCard in cards)
        {
            itCard.OnCardSelected += CheckSelectCard;
            itCard.OnCardSelected -= Redraw;
        }

        buildingPlacer.OnBuildingPlaced += OnSelectedCardPlayed;
        currencyCounter.OnCurrencyAdded += CheckCardsCost;
        currencyCounter.OnCurrencySpent += CheckCardsCost;

        if (OnFinishRedrawing != null) OnFinishRedrawing();
    }

    private void Redraw(BuildingCard card)
    {
        redrawsLeft--;
        card.OnCardHovered -= SetHoveredCard;
        card.OnCardUnhovered -= SetStandardCard;
        card.OnCardSelected -= Redraw;

        if (card.isShowingInfo)
        {
            SetCardHideInfo(card);
        }
        card.OnCardInfoSelected -= SetCardShowInfo;


        card.cardLocation = BuildingCard.CardLocation.DECK;

        if (ReturnCardToDeck != null) ReturnCardToDeck(card);
        if (OnQueryRedrawCard != null) OnQueryRedrawCard();

        if (!HasRedrawsLeft())
        {
            FinishedRedrawing();
        }
    }
    public bool HasRedrawsLeft()
    {
        return redrawsLeft > 0;
    }
    public int GetRedrawsLeft()
    {
        return redrawsLeft;
    }


    public void HintedCardWillBeAdded()
    {
        if (AlreadyHasSelectedCard)
        {
            selectedCard.ImmediateStandardState();
            ResetAndSetStandardCard(selectedCard);
        }
    }
    public void AddCard(BuildingCard card)
    {
        cards.Add(card);

        if (isHidden) HandTransform.position = defaultHandPosition;

        if (!card.AlreadySpawnedCopyBuildingPrefab)
        {
            card.CreateCopyBuildingPrefab(buildingsHolder, currencyCounter);
        }

        CheckCardsCost();
    }


    private void SetHoveredCard(BuildingCard card)
    {
        if (isHidden) ShowHand();

        hoveredCard = card;
        card.HoveredState();

        foreach (BuildingCard itCard in cards)
        {
            itCard.OnCardHovered -= SetHoveredCard;
        }


        // Audio
        GameAudioManager.GetInstance().PlayCardHovered();
    }


    private void SetStandardCard(BuildingCard card)
    {
        if (!isHidden)
        {
            StartCoroutine("WaitToHideHand");
        }

        hoveredCard = null;
        card.StandardState();


        if (card.isShowingInfo)
        {
            SetCardHideInfo(card);            
        }


        foreach (BuildingCard itCard in cards)
        {
            itCard.OnCardHovered += SetHoveredCard;            
        }
    }

    private void ResetAndSetStandardCard(BuildingCard card)
    {
        HandTransform.position = defaultHandPosition;
        SetStandardCard(selectedCard);
        selectedCard.OnCardInfoSelected += SetCardShowInfo;
        selectedCard = null;

        buildingPlacer.DisablePlacing();

        ShowHand();
    }

    private void CheckSelectCard(BuildingCard card)
    {
        int cardCost = card.GetCardPlayCost();

        if (currencyCounter.HasEnoughCurrency(cardCost))
        {
            SetSelectedCard(card);            
        }
    }

    private void SetSelectedCard(BuildingCard card)
    {
        if (AlreadyHasSelectedCard) return;

        selectedCard = card;
        selectedCard.SelectedState();

        if (selectedCard.isShowingInfo)
        {
            SetCardHideInfo(selectedCard);
        }
        selectedCard.OnCardInfoSelected -= SetCardShowInfo;


        buildingPlacer.EnablePlacing(card);

        //hide hand
        HideHand(false);

        // Audio
        GameAudioManager.GetInstance().PlayCardSelected();
    }

    private void OnSelectedCardPlayed()
    {
        SubtractCurrencyAndRemoveCard();

        if (OnCardPlayed != null) OnCardPlayed();

        // Audio
        GameAudioManager.GetInstance().PlayCurrencySpent();
    }

    private void SubtractCurrencyAndRemoveCard()
    {
        int cardCost = selectedCard.GetCardPlayCost();
        currencyCounter.SubtractCurrency(cardCost);

        selectedCard.gameObject.SetActive(false);
        cards.Remove(selectedCard);


        //ResetAndSetStandardCard(selectedCard);
        ///////////
        //SetStandardCard(selectedCard);
        foreach (BuildingCard itCard in cards)
        {
            itCard.OnCardHovered += SetHoveredCard;
        }

        selectedCard = null;
        buildingPlacer.DisablePlacing();
        HandTransform.position = defaultHandPosition;
        //ShowHand();
        ///////////
        
        InitCardsInHand();
    }

    void SetInitHandPosition()
    {
        initHandPosition = cardHolder.position;
    }

    private void ComputeSelectedPosition()
    {
        selectedPosition = (HandTransform.right * (-2.5f)) + (HandTransform.up * (2f)) + HandTransform.position;
    }
    
    private void ComputeHiddenPosition()
    {
        defaultHandPosition = HandTransform.position;
        hiddenHandPosition = HandTransform.position + (-1.15f * HandTransform.up);

        hiddenDisplacement = hiddenHandPosition - defaultHandPosition;
    }

    private void HideHand(bool playSound)
    {
        isHidden = true;

        HandTransform.DOKill();
        HandTransform.DOMove(hiddenHandPosition, 0.1f);


        // Audio
        if (playSound) GameAudioManager.GetInstance().PlayCardHoverExit();
    }

    private void ShowHand()
    {
        isHidden = false;

        HandTransform.DOKill();
        HandTransform.DOMove(defaultHandPosition, 0.1f);
    }


    private IEnumerator WaitToHideHand()
    {
        yield return new WaitForSeconds(0.05f);

        if (!IsHoveringCard)
        {
            HideHand(true);
        }
    }

    private IEnumerator InvokeDrawCardAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        yield return null;

        if (OnQueryDrawCard != null) OnQueryDrawCard();
    }

    public void RemoveCard(BuildingCard card) {
        cards.Remove(card);
    }
    private void CheckCardsCost()
    {
        for (int i = 0; i < cards.Count; ++i)
        {
            int cardCost = cards[i].GetCardPlayCost();

            if (currencyCounter.HasEnoughCurrency(cardCost))
            {
                cards[i].SetCanBePlayedAnimation();
            }
            else
            {
                cards[i].SetCannotBePlayedAnimation();
            }
        }
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
    
}
