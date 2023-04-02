using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPartHolder : MonoBehaviour
{
    [HideInInspector] public Transform cardsHolderTransform;
    [SerializeField] private Transform selectedTransform;
    [SerializeField, Min(0f)] private float distanceBetweenCards = 0.8f;

    // Serialize for now
    [SerializeField] private CardPart[] cardParts;

    [Header("Materials")]
    [SerializeField] private MeshRenderer placerMeshRenderer;
    private Material placerMaterial;

    [HideInInspector] public bool appearAnimationCanStartMoving = true;


    public CardPart selectedCardPart { get; private set; }

    public bool AlreadyHasSelectedPart => selectedCardPart != null;

    private bool canInteract;

    private bool cardWasSelected = false;

    [HideInInspector] public bool canSelectCard = true;


    public delegate void CardPartHolderAction();
    public event CardPartHolderAction OnPartSelected;
    public event CardPartHolderAction OnPartUnselected;




    private void OnValidate()
    {
        //Init(cardParts);
    }
    private void Awake()
    {
        cardsHolderTransform = transform;
        //Init(cardParts);
        CardPart.OnCardHovered += SetHoveredCard;

        canInteract = true;

        placerMaterial = placerMeshRenderer.material;
        placerMaterial.SetFloat("_IsOn", 1f);
    }

    private void OnEnable()
    {
        CardPartReplaceManager.OnReplacementStart += StopAnimationCompletely;
    }

    private void OnDisable()
    {
        CardPartReplaceManager.OnReplacementStart -= StopAnimationCompletely;
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

            cardParts[i].Init();
        }

    }


    private void SetHoveredCard(CardPart card)
    {
        card.HoveredState();

        CardPart.OnCardHovered -= SetHoveredCard;
        CardPart.OnCardUnhovered += SetStandardCard;
        CardPart.OnCardSelected += SetSelectedCard;

        card.OnCardInfoSelected += SetCardPartShowInfo;

        // Audio
        GameAudioManager.GetInstance().PlayCardHovered();
    }

    private void SetStandardCard(CardPart card)
    {
        card.StandardState(cardWasSelected);
        cardWasSelected = false; // reset

        if (card.isShowingInfo)
        {
            SetCardPartHideInfo(card);
        }
        card.OnCardInfoSelected -= SetCardPartShowInfo;


        if (canInteract) CardPart.OnCardHovered += SetHoveredCard;
        CardPart.OnCardUnhovered -= SetStandardCard;
        CardPart.OnCardSelected -= SetSelectedCard;
    }

    private void SetSelectedCard(CardPart card)
    {
        if (!canSelectCard) return;
        if (AlreadyHasSelectedPart) return;

        selectedCardPart = card;
        selectedCardPart.SelectedState(true);
        cardWasSelected = true;

        if (selectedCardPart.isShowingInfo)
        {
            SetCardPartHideInfo(selectedCardPart);
        }
        selectedCardPart.OnCardInfoSelected -= SetCardPartShowInfo;


        CardPart.OnCardHovered -= SetHoveredCard;
        CardPart.OnCardSelected -= SetSelectedCard;
        selectedCardPart.OnCardSelectedNotHovered += RetrieveCard;

        if (OnPartSelected != null) OnPartSelected();

        // Audio
        GameAudioManager.GetInstance().PlayCardSelected();
    }

    private void SetCardPartShowInfo(CardPart cardPart)
    {
        cardPart.ShowInfo();


        cardPart.OnCardInfoSelected -= SetCardPartShowInfo;
        cardPart.OnCardInfoSelected += SetCardPartHideInfo;
    }
    private void SetCardPartHideInfo(CardPart cardPart)
    {
        cardPart.HideInfo();


        cardPart.OnCardInfoSelected += SetCardPartShowInfo;
        cardPart.OnCardInfoSelected -= SetCardPartHideInfo;
    }


    public void RetrieveCard(CardPart card)
    {
        selectedCardPart.OnCardSelectedNotHovered -= RetrieveCard;
        SetStandardCard(card);

        selectedCardPart = null;

        if (OnPartUnselected != null) OnPartUnselected();

        // Audio
        GameAudioManager.GetInstance().PlayCardHoverExit();
    }

    public void StopInteractions()
    {
        canInteract = false;

        selectedCardPart.OnCardSelectedNotHovered -= RetrieveCard;
    }



    public void Hide(float duration, float delayBetweenCards)
    {
        StartCoroutine(DoHide(duration, delayBetweenCards));
    }

    private IEnumerator DoHide(float duration, float delayBetweenCards)
    {
        for (int i = 0; i < cardParts.Length; ++i)
        {
            if (cardParts[i] == selectedCardPart) continue;

            Transform cardPartTransform = cardParts[i].transform;
            cardPartTransform.DOMove(cardPartTransform.position + (cardPartTransform.up * -1.5f), duration);

            yield return new WaitForSeconds(delayBetweenCards);
        }
    }


    public void StartAnimation()
    {
        placerMaterial.SetFloat("_IsAlwaysOn", 1f);
    }

    public void FinishAnimation()
    {
        placerMaterial.SetFloat("_IsAlwaysOn", 0f);
    }

    private void StopAnimationCompletely()
    {
        placerMaterial.SetFloat("_IsAlwaysOn", 0f);
        placerMaterial.SetFloat("_IsOn", 0f);
    }



    public IEnumerator AppearAnimation(float delayBeforeStartMoving)
    {
        Vector3 moveOffset = Vector3.forward * -2f;

        foreach (CardPart cardPart in cardParts)
        {
            cardPart.transform.position = cardPart.transform.position + moveOffset;
            cardPart.isInteractable = false;
        }

        yield return new WaitForSeconds(delayBeforeStartMoving);
        yield return new WaitUntil(() => appearAnimationCanStartMoving);


        float moveDuration = 0.5f;
        float delayBetweenCards = 0.3f;
        foreach (CardPart cardPart in cardParts)
        {
            cardPart.transform.DOMove(cardPart.transform.position - moveOffset, moveDuration);
            yield return new WaitForSeconds(delayBetweenCards);
        }

        foreach (CardPart cardPart in cardParts)
        {
            yield return new WaitForSeconds(moveDuration - delayBetweenCards);
            cardPart.isInteractable = true;
            cardPart.ReenableMouseInteraction();
        }
    }



    public void PlayBodyTutorialBlinkAnimation()
    {
        foreach (CardPart cardPart in cardParts)
        {
            CardPartBody cardPartBody = cardPart as CardPartBody;
            cardPartBody.PlayTutorialBlinkAnimation();
        }
    }

    public void PlayBaseTutorialBlinkAnimation(float delayBeforeAbility)
    {
        foreach (CardPart cardPart in cardParts)
        {
            CardPartBase cardPartBase = cardPart as CardPartBase;
            cardPartBase.PlayTutorialBlinkAnimation(delayBeforeAbility);
        }
    }

}
