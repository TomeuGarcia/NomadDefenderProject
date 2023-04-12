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


    [Header("CARD DRAG & DROP")]
    [SerializeField] private BoxCollider cardDragBoundsCollider;
    [SerializeField] private Transform cardDragBoundsTargetTransform;
    private Bounds cardDragBoundsTarget;

    [Header("Materials")]
    [SerializeField] private MeshRenderer placerMeshRenderer;
    private Material placerMaterial;

    [HideInInspector] public bool appearAnimationCanStartMoving = true;


    public CardPart selectedCardPart { get; private set; }

    public bool AlreadyHasSelectedPart => selectedCardPart != null;

    private bool canInteract;

    private bool cardWasSelected = false;

    [HideInInspector] public bool canSelectCard = true;


    private bool cardsInteractionEnabled = true;


    public delegate void CardPartHolderAction();
    public event CardPartHolderAction OnPartSelected;
    public event CardPartHolderAction OnPartUnselected;




    private void OnValidate()
    {
        //Init(cardParts);
    }
    private void Awake()
    {
        CardPart.DragStartBounds = cardDragBoundsCollider.bounds;
        CardPart.DragStartBounds.extents *= 2f;

        Vector3 boundsSize = cardDragBoundsTargetTransform.lossyScale;
        boundsSize.z += 1f;
        cardDragBoundsTarget = new Bounds(cardDragBoundsTargetTransform.position, boundsSize);


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
        
        BuildingCard.OnMouseDragStart += DisableCardsInteraction;
        BuildingCard.OnMouseDragEnd += EnableCardsInteraction;
    }

    private void OnDisable()
    {
        CardPartReplaceManager.OnReplacementStart -= StopAnimationCompletely;

        BuildingCard.OnMouseDragStart -= DisableCardsInteraction;
        BuildingCard.OnMouseDragEnd -= EnableCardsInteraction;
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


    private void EnableCardsInteraction()
    {
        cardsInteractionEnabled = true;
        foreach (CardPart cardPart in cardParts)
        {
            cardPart.EnableMouseInteraction();
        }
    }
    private void DisableCardsInteraction()
    {
        cardsInteractionEnabled = false;
        foreach (CardPart cardPart in cardParts)
        {
            cardPart.DisableMouseInteraction();
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
        selectedCardPart.SelectedState(true, repositionColliderOnEnd: true);

        selectedCardPart.OnDragMouseUp += CheckSnapCardAtSelectedPosition;

        cardWasSelected = true;

        if (selectedCardPart.isShowingInfo)
        {
            SetCardPartHideInfo(selectedCardPart);
        }
        selectedCardPart.OnCardInfoSelected -= SetCardPartShowInfo;


        CardPart.OnCardHovered -= SetHoveredCard;
        CardPart.OnCardSelected -= SetSelectedCard;
        selectedCardPart.OnCardSelectedNotHovered += RetrieveCard;


        // Audio
        GameAudioManager.GetInstance().PlayCardSelected();
    }

    private void CheckSnapCardAtSelectedPosition(CardPart cardPart)
    {
        cardPart.OnDragMouseUp -= CheckSnapCardAtSelectedPosition;


        if (cardDragBoundsTarget.Contains(cardPart.CardTransform.position))
        {
            //Debug.Log("YEP drop here");
            cardPart.GoToSelectedPosition();
            if (OnPartSelected != null) OnPartSelected();
        }
        else
        {
            RetrieveCard(cardPart);
            cardPart.ReenableMouseInteraction();
        }
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
            if (cardsInteractionEnabled) cardPart.ReenableMouseInteraction();
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
