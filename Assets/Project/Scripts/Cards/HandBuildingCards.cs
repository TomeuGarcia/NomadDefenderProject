using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HandBuildingCards : MonoBehaviour
{
    [SerializeField] private AnimationCurve cardsHeightCurve;
    [SerializeField] private AnimationCurve cardsRotationCurve;

    [SerializeField] private CurrencyCounter currencyCounter;
    [SerializeField] private BuildingPlacer buildingPlacer;

    private List<BuildingCard> cards;

    private BuildingCard selectedCard;
    private Vector3 selectedPosition;

    private Vector3 initHandPosition;
    private Vector3 defaultHandPosition;
    private Vector3 hiddenHandPosition;
    private bool isHidden;

    private bool AlreadyHasSelectedCard => selectedCard != null;

    private BuildingCard hoveredCard;
    private bool IsHoveringCard => hoveredCard != null;

    public Transform HandTransform => transform;



    public delegate void HandAction();
    public static event HandAction OnQueryDrawCard;
    public static event HandAction OnCardPlayed;




    private void OnValidate()
    {
        SetInitHandPosition();
        ComputeSelectedPosition();
        ComputeHiddenPosition();
    }

    private void Awake()
    {
        cards = new List<BuildingCard>();
    }

    public void Init()
    {
        SetInitHandPosition();
        ComputeSelectedPosition();
        ComputeHiddenPosition();

        InitCardsInHand();

        for (int i = 0; i < cards.Count; ++i)
        {
            cards[i].CreateCopyBuildingPrefab();
        }

        HideHand(false);

        CheckCardsCost();
    }

    private void OnEnable()
    {
        foreach(BuildingCard itCard in cards)
        {
            itCard.OnCardHovered += SetHoveredCard;
            itCard.OnCardUnhovered += SetStandardCard;
            itCard.OnCardSelected += CheckSelectCard;
        }
        

        buildingPlacer.OnBuildingPlaced += OnSelectedCardPlayed;

        currencyCounter.OnCurrencyAdded += CheckCardsCost;
        currencyCounter.OnCurrencySpent += CheckCardsCost;
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

    
    private void InitCardsInHand()
    {
        Vector3 hiddenDisplacement = hiddenHandPosition - defaultHandPosition;

        float cardCount = cards.Count;
        float displacementStep = Mathf.Min(0.6f / (cardCount * 0.2f), 0.6f);
        float halfCardCount = cards.Count / 2f;
        Vector3 startDisplacement = (-halfCardCount * displacementStep) * transform.right;

        float ratio = 0f;
        if (cards.Count > 0)
            ratio = 1f / cards.Count;
        for (int i = 0; i < cards.Count; ++i)
        {
            float iRatio = ratio * (i + 0.5f);
            Vector3 widthDisplacement = transform.right * displacementStep * i;
            Vector3 heightDisplacement = transform.up * cardsHeightCurve.Evaluate(iRatio);
            Vector3 depthDisplacement = transform.forward * (-0.2f * iRatio);
            Quaternion rotation = Quaternion.AngleAxis(cardsRotationCurve.Evaluate(iRatio), Vector3.forward);


            cards[i].transform.SetParent(transform);
            cards[i].transform.localPosition = Vector3.zero;
            cards[i].transform.position = initHandPosition + startDisplacement + widthDisplacement + heightDisplacement + depthDisplacement;
            cards[i].transform.localRotation = rotation;

            cards[i].InitPositions(selectedPosition, hiddenDisplacement);
        }
    }



    public void AddCard(BuildingCard card)
    {
        cards.Add(card);

        if (isHidden) transform.position = defaultHandPosition;

        InitCardsInHand();

        if (AlreadyHasSelectedCard) ResetAndSetStandardCard(selectedCard);

        if (!card.AlreadySpawnedCopyBuildingPrefab)
        {
            card.CreateCopyBuildingPrefab();
        }

        CheckCardsCost();

        card.OnCardHovered += SetHoveredCard;
        card.OnCardUnhovered += SetStandardCard;
        card.OnCardSelected += CheckSelectCard;
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

        foreach (BuildingCard itCard in cards)
        {
            itCard.OnCardHovered += SetHoveredCard;            
        }
    }

    private void ResetAndSetStandardCard(BuildingCard card)
    {
        transform.position = defaultHandPosition;
        SetStandardCard(selectedCard);
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
        transform.position = defaultHandPosition;
        //ShowHand();
        ///////////
        
        InitCardsInHand();
    }

    void SetInitHandPosition()
    {
        initHandPosition = gameObject.transform.position;
    }

    private void ComputeSelectedPosition()
    {
        selectedPosition = (transform.right * (-2.5f)) + (transform.up * (2f)) + transform.position;
    }
    
    private void ComputeHiddenPosition()
    {
        defaultHandPosition = transform.position;
        hiddenHandPosition = transform.position + (-1.15f * transform.up);
    }

    private void HideHand(bool playSound)
    {
        isHidden = true;

        HandTransform.DOKill();
        HandTransform.DOMove(hiddenHandPosition, 0.1f);

        foreach(BuildingCard card in cards)
            card.normalCollider();

        // Audio
        if (playSound) GameAudioManager.GetInstance().PlayCardHoverExit();
    }

    private void ShowHand()
    {
        isHidden = false;

        HandTransform.DOKill();
        HandTransform.DOMove(defaultHandPosition, 0.1f);

        foreach (BuildingCard card in cards)
            card.bigCollider();
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

}
