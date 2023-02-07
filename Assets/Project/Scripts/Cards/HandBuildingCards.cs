using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class HandBuildingCards : MonoBehaviour
{
    [SerializeField] private Transform handCameraTransform;

    [SerializeField] private Transform cardHolder;
    [SerializeField] private Transform buildingsHolder;

    [SerializeField] private AnimationCurve cardsHeightCurve;
    [SerializeField] private AnimationCurve cardsRotationCurve;

    [SerializeField] private Transform handSideBlockerLeft;
    [SerializeField] private Transform handSideBlockerRight;

    [SerializeField] private CurrencyCounter currencyCounter;
    [SerializeField] private BuildingPlacer buildingPlacer;
    private int initialRedraws = 3;
    private int redrawsLeft;
    private bool isInRedrawPhase = false;


    private List<BuildingCard> cards;

    private BuildingCard selectedCard;
    private Vector3 selectedPosition;

    private Vector3 initHandPosition;
    private Vector3 defaultHandPosition;
    private Vector3 hiddenHandPosition;
    private Vector3 hiddenDisplacement;
    private bool isHidden;
    private bool isBeingHidden = false;
    private bool isBeingShown = false;

    private bool isPlayerHoveringTheCards = false;

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
        isPlayerHoveringTheCards = false;
    }

    public void Init()
    {
        SetInitHandPosition();
        ComputeSelectedPosition();
        ComputeHiddenPosition();

        isInRedrawPhase = true;
        InitCardsInHandForRedraw();        

        for (int i = 0; i < cards.Count; ++i)
        {
            cards[i].CreateCopyBuildingPrefab(buildingsHolder, currencyCounter);
        }

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
        //Debug.Log("InitCardsInHand");

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
            if (!isBeingHidden && !isInRedrawPhase)
            {
                HideHand(true);
            }
        }

        UpdateHandSideBlockers();
    }


    public void InitCardsInHandAfterPlayingCard()
    {
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

            Vector3 finalPosition = hiddenHandPosition + startDisplacement + widthDisplacement + heightDisplacement + depthDisplacement;

            BuildingCard card = cards[i];

            float repositionDuration = 0.3f;
            card.StartRepositioning(finalPosition, repositionDuration);
            card.RootCardTransform.DOLocalRotate(rotation.eulerAngles, repositionDuration)
                .OnComplete(
                () => {
                    card.InitPositions(selectedPosition, hiddenDisplacement);
                    UpdateHandSideBlockers();
                });
        }

    }


    public void InitCardsInHandForRedraw()
    {
        //Debug.Log("InitCardsInHandFirstDraw");
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
            if (!card.AlreadyCanBeHovered)
            {
                card.OnCardHovered += SetHoveredCard;
            }
            card.OnCardUnhovered += SetStandardCard;
            card.OnCardSelected += Redraw;

            card.OnCardInfoSelected += SetCardShowInfo;


            card.cardLocation = BuildingCard.CardLocation.HAND;
        }

        UpdateHandSideBlockers();
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

        isInRedrawPhase = false;
    }

    private void Redraw(BuildingCard card)
    {
        foreach (BuildingCard itCard in cards)
        {
            itCard.OnCardHovered += SetHoveredCard;
        }

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


    public void CorrectCardsBeforeAddingCard()
    {
        bool corrected = false;

        for (int i = 0; i < cards.Count; ++i)
        {
            if (cards[i].IsRepositioning)
            {
                cards[i].ForceEndRepositioning();
            }
            else if (cards[i].cardState != BuildingCard.CardStates.STANDARD)
            {
                cards[i].ImmediateStandardState();
                corrected = true;

                if (cards[i].isShowingInfo)
                {
                    SetCardHideInfo(cards[i]);
                }
            }
        }

        if (corrected)
        {
            //// bellow is copied from SetStandardCard
            hoveredCard = null;

            foreach (BuildingCard itCard in cards)
            {
                itCard.OnCardHovered += SetHoveredCard;               
            }
            ////
        }

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

        card.PlayDrawAnimation(
            () => { if (!isInRedrawPhase) { 
                    StartCoroutine(DelayedTryHideHandAfterDraw()); } 
            } );

        CheckCardsCost();
    }


    private void SetHoveredCard(BuildingCard card)
    {
        isPlayerHoveringTheCards = true;

        if (isHidden && !isBeingShown) ShowHand();

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
        isPlayerHoveringTheCards = false;

        if (!isHidden && !isInRedrawPhase)
        {
            StartCoroutine("DelayedHideHand");
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
        //HandTransform.position = defaultHandPosition;
        SetStandardCard(selectedCard);
        selectedCard.OnCardInfoSelected += SetCardShowInfo;
        selectedCard = null;

        buildingPlacer.DisablePlacing();

        //if (!isBeingShown) ShowHand();

        UpdateHandSideBlockers();
    }

    private void CheckSelectCard(BuildingCard card)
    {
        int cardCost = card.GetCardPlayCost();

        if (currencyCounter.HasEnoughCurrency(cardCost))
        {
            SetSelectedCard(card);            
        }
        else
        {
            currencyCounter.PlayNotEnoughCurrencyAnimation();
            card.PlayCanNotBePlayedAnimation();
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
        if (!isBeingHidden && !isInRedrawPhase) HideHand(false);

        DisableHandSideBlockers();

        // Audio
        GameAudioManager.GetInstance().PlayCardSelected();
    }

    private void OnSelectedCardPlayed()
    {
        SubtractCurrencyAndRemoveCard();

        if (OnCardPlayed != null) OnCardPlayed();

        isPlayerHoveringTheCards = false;
        StartCoroutine(DelayedTryHideHandAfterDraw());

        // Audio
        GameAudioManager.GetInstance().PlayCurrencySpent();
    }

    private void SubtractCurrencyAndRemoveCard()
    {
        int cardCost = selectedCard.GetCardPlayCost();
        currencyCounter.SubtractCurrency(cardCost);

        selectedCard.gameObject.SetActive(false);
        cards.Remove(selectedCard);


        ///////////
        foreach (BuildingCard itCard in cards)
        {
            itCard.OnCardHovered += SetHoveredCard;
        }

        selectedCard = null;
        buildingPlacer.DisablePlacing();
        //HandTransform.position = defaultHandPosition;
        ///////////

        InitCardsInHandAfterPlayingCard();
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

        isBeingHidden = true;

        HandTransform.DOComplete(true);
        HandTransform.DOMove(hiddenHandPosition, 0.1f)
            .OnComplete(() => isBeingHidden = false);


        // Audio
        if (playSound) GameAudioManager.GetInstance().PlayCardHoverExit();
    }

    private void ShowHand()
    {
        isHidden = false;

        isBeingShown = true;

        HandTransform.DOComplete(true);
        HandTransform.DOMove(defaultHandPosition, 0.1f)
            .OnComplete( () => isBeingShown = false );
    }


    private IEnumerator DelayedHideHand()
    {
        yield return new WaitForSeconds(0.05f);

        if (!IsHoveringCard && !isBeingHidden)
        {
            HideHand(true);
        }
    }

    private IEnumerator DelayedTryHideHandAfterDraw()
    {
        yield return new WaitForSeconds(0.5f);

        if (!isPlayerHoveringTheCards && !isBeingHidden)
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
    

    private void EnableHandSideBlockers()
    {
        handSideBlockerLeft.gameObject.SetActive(true);
        handSideBlockerRight.gameObject.SetActive(true);
    }
    private void DisableHandSideBlockers()
    {
        handSideBlockerLeft.gameObject.SetActive(false);
        handSideBlockerRight.gameObject.SetActive(false);
    }
    private void UpdateHandSideBlockers()
    {
        if (cards.Count < 2)
        {
            DisableHandSideBlockers();
            return;
        }



        EnableHandSideBlockers();

        float rightmostX = 0f;
        float leftmostX = 0f;
        foreach (BuildingCard card in cards)
        {
            float currentX = card.RootCardTransform.position.x;
            if (currentX < leftmostX)
            {
                leftmostX = currentX;
            }
            else if(currentX > rightmostX)
            {
                rightmostX = currentX;
            }
            
        }

        Vector3 halfCardWidthOffset = Vector3.right * 0.5f;
        handSideBlockerLeft.position = new Vector3(leftmostX, handSideBlockerLeft.position.y, handSideBlockerLeft.position.z) - halfCardWidthOffset;
        handSideBlockerRight.position = new Vector3(rightmostX, handSideBlockerRight.position.y, handSideBlockerRight.position.z) + halfCardWidthOffset;

        handSideBlockerLeft.rotation = Quaternion.LookRotation((handCameraTransform.position - handSideBlockerLeft.position).normalized, handCameraTransform.up);
        handSideBlockerRight.rotation = Quaternion.LookRotation((handCameraTransform.position - handSideBlockerRight.position).normalized, handCameraTransform.up);
    }

}
