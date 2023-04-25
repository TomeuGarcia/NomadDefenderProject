using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class UpgradeCardHolder : MonoBehaviour
{
    [SerializeField] private AnimationCurve cardsHeightCurve;
    [SerializeField] private AnimationCurve cardsRotationCurve;

    public Transform CardsHolder => transform;
    [SerializeField] private Transform selectedTransform;
    [SerializeField, Min(0f)] private float distanceBetweenCards = 0.8f;

    // Serialize for now
    [SerializeField] private BuildingCard[] cards;

    [Header("CARD DRAG & DROP")]
    [SerializeField] private BoxCollider cardDragBoundsCollider;
    [SerializeField] private Transform cardDragBoundsTargetTransform;
    private Bounds cardDragBoundsTarget;

    public BuildingCard selectedCard { get; private set; }

    //[Header("Materials")]
    //[SerializeField] private MeshRenderer placerMeshRenderer;
    //private Material placerMaterial;


    [HideInInspector] public bool appearAnimationCanStartMoving = true;

    public bool AlreadyHasSelectedCard => selectedCard != null;

    private bool canInteract;

    private float startDelay, duration, delayBetweenCards; // Animation variables

    private bool cardWasSelected = false;

    [HideInInspector] public bool canSelectCard = true;

    private bool cardsInteractionEnabled = true;


    public delegate void CardPartHolderAction();
    public event CardPartHolderAction OnCardSelected;
    public event CardPartHolderAction OnCardUnselected;
    public event CardPartHolderAction OnFinalRetrieve;



    private void OnValidate()
    {
        //Init(cards);
    }
    private void Awake()
    {
        BuildingCard.DragStartBounds = cardDragBoundsCollider.bounds;
        BuildingCard.DragStartBounds.extents *= 2f;

        Vector3 boundsSize = cardDragBoundsTargetTransform.lossyScale;
        boundsSize.x += 1f;
        boundsSize.z += 3f;
        cardDragBoundsTarget = new Bounds(cardDragBoundsTargetTransform.position, boundsSize);


        //Init(cards);
        foreach (BuildingCard itCard in cards)
        {
            itCard.OnCardHovered += SetHoveredCard;
        }

        canInteract = true;

        //placerMaterial = placerMeshRenderer.material;
        //placerMaterial.SetFloat("_IsOn", 1f);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(cardDragBoundsTarget.center, cardDragBoundsTarget.size);
    }

    private void OnEnable()
    {
        CardPartReplaceManager.OnReplacementDone += StopAnimationCompletely;

        CardPart.OnMouseDragStart += DisableCardsInteraction;
        CardPart.OnMouseDragEnd += EnableCardsInteraction;
    }

    private void OnDisable()
    {
        CardPartReplaceManager.OnReplacementDone -= StopAnimationCompletely;

        CardPart.OnMouseDragStart -= DisableCardsInteraction;
        CardPart.OnMouseDragEnd -= EnableCardsInteraction;

    }



    public void Init(BuildingCard[] cards)
    {
        selectedCard = null;
        this.cards = cards;
        InitCardsInHand();

        foreach (BuildingCard itCard in cards)
        {
            itCard.OnCardHovered += SetHoveredCard;
        }
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
            Quaternion rotation = Quaternion.AngleAxis(cardsRotationCurve.Evaluate(iRatio), Vector3.forward);


            cards[i].transform.SetParent(transform);
            cards[i].transform.localPosition = Vector3.zero;
            cards[i].transform.position += startDisplacement + widthDisplacement + heightDisplacement;
            cards[i].transform.localRotation = rotation;

            cards[i].InitPositions(selectedTransform.position, Vector3.zero, cards[i].transform.position);
        }
    }


    private void EnableCardsInteraction()
    {
        cardsInteractionEnabled = true;
        foreach (BuildingCard card in cards)
        {
            card.EnableMouseInteraction();
        }
    }
    private void DisableCardsInteraction()
    {
        cardsInteractionEnabled = false;
        foreach (BuildingCard card in cards)
        {
            card.DisableMouseInteraction();
        }
    }




    private void SetHoveredCard(BuildingCard card)
    {
        card.HoveredState();

        //card.OnCardInfoSelected += SetCardShowInfo;


        foreach (BuildingCard itCard in cards)
        {
            itCard.OnCardHovered -= SetHoveredCard;
            itCard.OnCardUnhovered += SetStandardCard;
            itCard.OnCardSelected += SetSelectedCard;
        }

        // Audio
        GameAudioManager.GetInstance().PlayCardHovered();
    }

    private void SetStandardCard(BuildingCard card)
    {
        card.StandardState(cardWasSelected, duration: BuildingCard.toStandardTime);
        cardWasSelected = false; // reset

        //if (card.isShowingInfo)
        //{
        //    SetCardHideInfo(card);
        //}
        //card.OnCardInfoSelected -= SetCardShowInfo;

        if (canInteract)
        {
            foreach (BuildingCard itCard in cards)
            {
                itCard.OnCardHovered += SetHoveredCard;
            }
        }

        card.canDisplayInfoIfNotInteractable = false;
        foreach (BuildingCard itCard in cards)
        {
            itCard.OnCardUnhovered -= SetStandardCard;
            itCard.OnCardSelected -= SetSelectedCard;
            itCard.canDisplayInfoIfNotInteractable = false;
        }
    }

    private void SetSelectedCard(BuildingCard card)
    {
        if (!canSelectCard) return;
        if (AlreadyHasSelectedCard) return;

        cardDragBoundsCollider.gameObject.SetActive(true);

        selectedCard = card;
        selectedCard.SelectedState(true, repositionColliderOnEnd: true, enableInteractionOnEnd: true);

        selectedCard.OnDragMouseUp += CheckSnapCardAtSelectedPosition;

        cardWasSelected = true;

        //if (selectedCard.isShowingInfo)
        //{
        //    SetCardHideInfo(selectedCard);            
        //}

        selectedCard.canDisplayInfoIfNotInteractable = true;
        foreach (BuildingCard itCard in cards)
        {
            itCard.OnCardHovered -= SetHoveredCard;
            itCard.OnCardSelected -= SetSelectedCard;
            itCard.canDisplayInfoIfNotInteractable = false;
        }
        selectedCard.OnCardSelectedNotHovered += RetrieveCard;

        // Audio
        GameAudioManager.GetInstance().PlayCardSelected();
    }

    private void CheckSnapCardAtSelectedPosition(BuildingCard card)
    {
        card.OnDragMouseUp -= CheckSnapCardAtSelectedPosition;

        
        if (cardDragBoundsTarget.Contains(card.CardTransform.position))
        {
            //Debug.Log("YEP drop here");
            card.GoToSelectedPosition();
            if (OnCardSelected != null) OnCardSelected();

            foreach (BuildingCard itCard in cards)
            {
                itCard.canDisplayInfoIfNotInteractable = true;
            }
        }
        else
        {
            RetrieveCard(card);
            card.ReenableMouseInteraction();
        }


        cardDragBoundsCollider.gameObject.SetActive(false);
    }


    //private void SetCardShowInfo(BuildingCard card)
    //{
    //    card.ShowInfo();


    //    card.OnCardInfoSelected -= SetCardShowInfo;
    //    card.OnCardInfoSelected += SetCardHideInfo;
    //}
    //private void SetCardHideInfo(BuildingCard card)
    //{
    //    card.HideInfo();


    //    card.OnCardInfoSelected += SetCardShowInfo;
    //    card.OnCardInfoSelected -= SetCardHideInfo;
    //}


    public void RetrieveCard(BuildingCard card)
    {
        selectedCard.OnCardSelectedNotHovered -= RetrieveCard;
        SetStandardCard(card);

        selectedCard = null;

        if (OnCardUnselected != null) OnCardUnselected();

        // Audio
        GameAudioManager.GetInstance().PlayCardHoverExit();
    }

    public void StopInteractions()
    {
        canInteract = false;

        selectedCard.OnCardSelectedNotHovered -= RetrieveCard;

        foreach (BuildingCard card in cards)
        {
            card.canDisplayInfoIfNotInteractable = false;
        }
    }


    public void StartFinalRetrieve(float startDelay, float duration, float delayBetweenCards)
    {
        this.startDelay = startDelay;
        this.duration = duration;
        this.delayBetweenCards = delayBetweenCards;

        //placerMaterial.SetFloat("_IsAlwaysOn", 0f);
        //placerMaterial.SetFloat("_IsOn", 1f);       

        FinalRetrieveCard(selectedCard);
    }

    public void EnableFinalRetrieve(float startDelay, float duration, float delayBetweenCards)
    {
        selectedCard.OnCardSelectedNotHovered += FinalRetrieveCard;

        this.startDelay = startDelay;
        this.duration = duration;
        this.delayBetweenCards = delayBetweenCards;

        //placerMaterial.SetFloat("_IsAlwaysOn", 0f);
        //placerMaterial.SetFloat("_IsOn", 1f);
    }

    public void FinalRetrieveCard(BuildingCard card)
    {
        selectedCard.OnCardSelectedNotHovered -= RetrieveCard;

        selectedCard.RootCardTransform.SetParent(transform);
        SetStandardCard(card);   


        StartCoroutine(DoFinalRetrieve(startDelay, duration, delayBetweenCards, selectedCard));
        selectedCard = null;

        // Audio
        GameAudioManager.GetInstance().PlayCardSelected();
    }

    private IEnumerator DoFinalRetrieve(float startDelay, float duration, float delayBetweenCards, BuildingCard selectedCard)
    {
        selectedCard.RootCardTransform.DOBlendableMoveBy(selectedCard.RootCardTransform.forward * -3.0f, 0.75f);
        yield return new WaitForSeconds(startDelay);

        Hide(duration, delayBetweenCards);

        yield return new WaitForSeconds(duration * cards.Length + delayBetweenCards * cards.Length);

        if (OnFinalRetrieve != null) OnFinalRetrieve();
    }


    public void Hide(float duration, float delayBetweenCards)
    {
        StartCoroutine(DoHide(duration, delayBetweenCards));
    }

    private IEnumerator DoHide(float duration, float delayBetweenCards)
    {
        for (int i = 0; i < cards.Length; ++i)
        {
            if (cards[i] == selectedCard) continue;

            Transform cardPartTransform = cards[i].RootCardTransform;
            Vector3 moveBy = cardPartTransform.up * -2.2f;
            //cardPartTransform.DOMove(cardPartTransform.position + moveBy, duration);
            cardPartTransform.DOBlendableMoveBy(moveBy, duration);

            yield return new WaitForSeconds(delayBetweenCards);
        }
    }


    public void StartAnimation()
    {
        //placerMaterial.SetFloat("_IsAlwaysOn", 1f);        
    }

    public void FinishAnimation()
    {
        //placerMaterial.SetFloat("_IsAlwaysOn", 0f);        
    }

    public void StopAnimationCompletely()
    {
        //placerMaterial.SetFloat("_IsAlwaysOn", 0f);
        //placerMaterial.SetFloat("_IsOn", 0f);
    }


    public IEnumerator AppearAnimation(float delayBeforeStartMoving)
    {
        Vector3 moveOffset = Vector3.forward * -3f;

        foreach (BuildingCard card in cards)
        {
            card.RootCardTransform.position = card.RootCardTransform.position + moveOffset;
            card.canBeHovered = false;
        }

        yield return new WaitForSeconds(delayBeforeStartMoving);
        yield return new WaitUntil(() => appearAnimationCanStartMoving);


        float moveDuration = 0.7f;
        float delayBetweenCards = 0.3f;
        foreach (BuildingCard card in cards)
        {
            card.RootCardTransform.DOMove(card.RootCardTransform.position - moveOffset, moveDuration);
            yield return new WaitForSeconds(delayBetweenCards);
        }

        
        foreach (BuildingCard card in cards)
        {
            yield return new WaitForSeconds(moveDuration - delayBetweenCards);
            card.canBeHovered = true;
            if (cardsInteractionEnabled) card.ReenableMouseInteraction();
        }
    }


}
