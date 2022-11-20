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


    public CardPart selectedCardPart { get; private set; }

    public bool AlreadyHasSelectedPart => selectedCardPart != null;

    private bool canInteract;


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
    }

    private void SetStandardCard(CardPart card)
    {
        card.StandardState();

        if (canInteract) CardPart.OnCardHovered += SetHoveredCard;
        CardPart.OnCardUnhovered -= SetStandardCard;
        CardPart.OnCardSelected -= SetSelectedCard;
    }

    private void SetSelectedCard(CardPart card)
    {
        if (AlreadyHasSelectedPart) return;

        selectedCardPart = card;
        selectedCardPart.SelectedState();

        CardPart.OnCardHovered -= SetHoveredCard;
        CardPart.OnCardSelected -= SetSelectedCard;
        selectedCardPart.OnCardSelectedNotHovered += RetrieveCard;

        if (OnPartSelected != null) OnPartSelected();
    }


    public void RetrieveCard(CardPart card)
    {
        selectedCardPart.OnCardSelectedNotHovered -= RetrieveCard;
        SetStandardCard(card);

        selectedCardPart = null;

        if (OnPartUnselected != null) OnPartUnselected();
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

}
