using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeCardHolder : MonoBehaviour
{
    [SerializeField] private AnimationCurve cardsHeightCurve;
    [SerializeField] private AnimationCurve cardsRotationCurve;

    [SerializeField] private Transform selectedTransform;
    [SerializeField, Min(0f)] private float distanceBetweenCards = 0.8f;

    // Serialize for now
    [SerializeField] private BuildingCard[] cards;

    public BuildingCard selectedCard { get; private set; }

    [Header("Materials")]
    [SerializeField] private MeshRenderer placerMeshRenderer;
    private Material placerMaterial;

    public bool AlreadyHasSelectedCard => selectedCard != null;

    private bool canInteract;

    private float startDelay, duration, delayBetweenCards; // Animation variables



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
        //Init(cards);
        BuildingCard.OnCardHovered += SetHoveredCard;
        canInteract = true;

        placerMaterial = placerMeshRenderer.material;
        placerMaterial.SetFloat("_IsOn", 1f);
    }

    private void OnEnable()
    {
        CardPartReplaceManager.OnReplacementDone += StopAnimationCompletely;
    }

    private void OnDisable()
    {
        CardPartReplaceManager.OnReplacementDone -= StopAnimationCompletely;
    }



    public void Init(BuildingCard[] cards)
    {
        selectedCard = null;
        this.cards = cards;
        InitCardsInHand();
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

            cards[i].InitPositions(selectedTransform.position, Vector3.zero);
        }
    }


    private void SetHoveredCard(BuildingCard card)
    {
        card.HoveredState();

        BuildingCard.OnCardHovered -= SetHoveredCard;
        BuildingCard.OnCardUnhovered += SetStandardCard;
        BuildingCard.OnCardSelected += SetSelectedCard;

        // Audio
        GameAudioManager.GetInstance().PlayCardHovered();
    }

    private void SetStandardCard(BuildingCard card)
    {
        card.StandardState();
  
        if (canInteract) BuildingCard.OnCardHovered += SetHoveredCard;
        BuildingCard.OnCardUnhovered -= SetStandardCard;
        BuildingCard.OnCardSelected -= SetSelectedCard;
    }

    private void SetSelectedCard(BuildingCard card)
    {
        if (AlreadyHasSelectedCard) return;

        selectedCard = card;
        selectedCard.SelectedState();

        BuildingCard.OnCardHovered -= SetHoveredCard;
        BuildingCard.OnCardSelected -= SetSelectedCard;
        selectedCard.OnCardSelectedNotHovered += RetrieveCard;

        if (OnCardSelected != null) OnCardSelected();

        // Audio
        GameAudioManager.GetInstance().PlayCardSelected();
    }


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
    }


    public void EnableFinalRetrieve(float startDelay, float duration, float delayBetweenCards)
    {
        selectedCard.OnCardSelectedNotHovered += FinalRetrieveCard;

        this.startDelay = startDelay;
        this.duration = duration;
        this.delayBetweenCards = delayBetweenCards;

        placerMaterial.SetFloat("_IsAlwaysOn", 0f);
        placerMaterial.SetFloat("_IsOn", 1f);
    }

    public void FinalRetrieveCard(BuildingCard card)
    {
        selectedCard.OnCardSelectedNotHovered -= RetrieveCard;
        SetStandardCard(card);

        selectedCard = null;

        StartCoroutine(DoFinalRetrieve(startDelay, duration, delayBetweenCards));

        // Audio
        GameAudioManager.GetInstance().PlayCardSelected();
    }

    private IEnumerator DoFinalRetrieve(float startDelay, float duration, float delayBetweenCards)
    {
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

            Transform cardPartTransform = cards[i].transform;
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

    public void StopAnimationCompletely()
    {
        placerMaterial.SetFloat("_IsAlwaysOn", 0f);
        placerMaterial.SetFloat("_IsOn", 0f);
    }


}
