using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandBuildingCards : MonoBehaviour
{
    [SerializeField] private AnimationCurve cardsHeightCurve;
    [SerializeField] private AnimationCurve cardsRotationCurve;

    [SerializeField] private CurrencyCounter currencyCounter;
    [SerializeField] private BuildingPlacer buildingPlacer;
    [SerializeField] private Lerp lerp;

    private List<BuildingCard> cards;

    [SerializeField] private float lerpSpeed;

    private BuildingCard selectedCard;
    private Vector3 selectedPosition;

    private Vector3 initHandPosition;
    private Vector3 defaultHandPosition;
    private Vector3 hiddenHandPosition;
    private bool isHidden;

    private bool AlreadyHasSelectedCard => selectedCard != null;

    private BuildingCard hoveredCard;
    private bool IsHoveringCard => hoveredCard != null;


    public delegate void HandAction();
    public static event HandAction OnQueryDrawCard;




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
        ComputeSelectedPosition();
        ComputeHiddenPosition();

        InitCardsInHand();

        for (int i = 0; i < cards.Count; ++i)
        {
            cards[i].CreateCopyBuildingPrefab();
        }

        HideHand();
    }

    private void OnEnable()
    {
        BuildingCard.OnCardHovered += SetHoveredCard;
        BuildingCard.OnCardUnhovered += SetStandardCard;
        BuildingCard.OnCardSelected += CheckSelectCard;

        buildingPlacer.OnBuildingPlaced += SubtractCurrencyAndRemoveCard;
    }

    private void OnDisable()
    {
        BuildingCard.OnCardHovered -= SetHoveredCard;
        BuildingCard.OnCardUnhovered -= SetStandardCard;
        BuildingCard.OnCardSelected -= CheckSelectCard;

        buildingPlacer.OnBuildingPlaced -= SubtractCurrencyAndRemoveCard;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && AlreadyHasSelectedCard)
        {
            ResetAndSetStandardCard(selectedCard);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShowHand();

            //// please make this better in the future
            if (isHidden)
                StartCoroutine(InvokeDrawCardAfterDelay(lerpSpeed));
            else
                if (OnQueryDrawCard != null) OnQueryDrawCard();
            ////
        }
    }

    
    private void InitCardsInHand()
    {
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

            cards[i].InitPositions(selectedPosition);
        }
    }



    public void AddCard(BuildingCard card)
    {
        cards.Add(card);
        InitCardsInHand();

        if (!card.AlreadySpawnedCopyBuildingPrefab)
        {
            card.CreateCopyBuildingPrefab();
        }

        StartCoroutine(WaitToHideHand());
    }


    private void SetHoveredCard(BuildingCard card)
    {
        if (isHidden) ShowHand();

        hoveredCard = card;
        card.HoveredState();
        BuildingCard.OnCardHovered -= SetHoveredCard;
    }

    private void SetStandardCard(BuildingCard card)
    {
        if (!isHidden)
        {
            StartCoroutine("WaitToHideHand");
        }

        hoveredCard = null;
        card.StandardState();
        BuildingCard.OnCardHovered += SetHoveredCard;
    }



    private void ResetAndSetStandardCard(BuildingCard card)
    {
        selectedCard = null;
        SetStandardCard(card);

        buildingPlacer.DisablePlacing();

        ShowHand();
    }


    private void CheckSelectCard(BuildingCard card)
    {
        int cardCost = card.turretStats.playCost;

        if (currencyCounter.HasEnoughCurrency(cardCost)) // Check card's playCost !!!!!!
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
        HideHand();
    }


    private void SubtractCurrencyAndRemoveCard() // TODO
    {
        int cardCost = selectedCard.turretStats.playCost;
        currencyCounter.SubtractCurrency(cardCost);

        // TODO
        // for now reset
        //selectedCard.DoOnCardIsDrawn();
        selectedCard.gameObject.SetActive(false);
        cards.Remove(selectedCard);
        ResetAndSetStandardCard(selectedCard);

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

    private void HideHand()
    {
        isHidden = true;
        lerp.SpeedLerpPosition(hiddenHandPosition, lerpSpeed);
    }

    private void ShowHand()
    {
        isHidden = false;
        lerp.SpeedLerpPosition(defaultHandPosition, lerpSpeed);
    }


    private IEnumerator WaitToHideHand()
    {
        yield return new WaitForSeconds(0.05f);

        if (!IsHoveringCard)
        {
            HideHand();
        }
    }

    private IEnumerator InvokeDrawCardAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        yield return null;

        if (OnQueryDrawCard != null) OnQueryDrawCard();
    }

}
