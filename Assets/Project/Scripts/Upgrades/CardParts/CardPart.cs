using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using static Building;
using DG.Tweening;
using static BuildingCard;

public abstract class CardPart : MonoBehaviour
{
    public static float halfWidth = 0.7f;

    public enum CardPartStates { STANDARD, HOVERED, SELECTED }
    [HideInInspector] public CardPartStates cardState = CardPartStates.STANDARD;

    [Header("OTHER COMPONENTS")]
    [SerializeField] private BoxCollider cardCollider;
    private Vector3 cardColliderOffset;
    [SerializeField] private Transform cardHolder;

    [Header("VISUALS")]
    [SerializeField] protected CanvasGroup interfaceCanvasGroup;

    [Header("CARD INFO")]
    [SerializeField] protected GameObject infoInterface;
    protected Coroutine showInfoCoroutine = null;
    protected bool isShowInfoAnimationPlaying = false;
    protected bool isHideInfoAnimationPlaying = false;


    private Vector3 standardPosition;
    private Vector3 hoveredPosition;
    private Vector3 selectedPosition;

    public Transform RootCardTransform => transform;
    public Transform CardTransform => cardHolder;

    private Vector3 HoveredTranslation => CardTransform.up * 0.2f + CardTransform.forward * -0.04f;
    public Vector3 SelectedPosition => CardTransform.position + (CardTransform.up * 1.3f) + (-CardTransform.right * 1.3f);


    [HideInInspector] public bool isShowingInfo;
    protected bool canInfoInteract = true;


    public delegate void BuildingCardPartAction(CardPart cardPart);
    public static event BuildingCardPartAction OnCardHovered;
    public static event BuildingCardPartAction OnCardUnhovered;
    public static event BuildingCardPartAction OnCardSelected;
    public event BuildingCardPartAction OnCardInfoSelected;

    public event BuildingCardPartAction OnCardSelectedNotHovered;

    private void Awake()
    {
        AwakeInit();
    }
    protected virtual void AwakeInit()
    {
        cardColliderOffset = cardCollider.center;
    }

    private void OnMouseEnter()
    {
        if (cardState != CardPartStates.STANDARD) return;

        if (OnCardHovered != null) OnCardHovered(this);
    }

    private void OnMouseExit()
    {
        if (cardState != CardPartStates.HOVERED) return;

        if (OnCardUnhovered != null) OnCardUnhovered(this);
    }

    private void OnMouseDown()
    {
        if (cardState == CardPartStates.HOVERED)
        {
            if (OnCardSelected != null) OnCardSelected(this);
        }
        else 
        {
            if (OnCardSelectedNotHovered != null) OnCardSelectedNotHovered(this);
        }
        
    }

    private void Update()
    {
        if (canInfoInteract && Input.GetMouseButtonDown(1))
        {
            if (cardState == CardPartStates.HOVERED)
            {
                if (OnCardInfoSelected != null) OnCardInfoSelected(this);
            }
        }
    }

    public virtual void Init()
    {

    }

    public void InitPositions(Vector3 selectedPosition)
    {
        standardPosition = transform.position;
        hoveredPosition = transform.position + HoveredTranslation;
        this.selectedPosition = selectedPosition;
    }

    public void StandardState(bool repositionColliderOnEnd = false)
    {
        cardState = CardPartStates.STANDARD;

        CardTransform.DOMove(standardPosition, BuildingCard.unhoverTime)
            .OnComplete( () => { if (repositionColliderOnEnd) RepositionColliderToCardTransform(); });
    }

    public void HoveredState()
    {
        cardState = CardPartStates.HOVERED;

        CardTransform.DOMove(hoveredPosition, BuildingCard.hoverTime);
    }

    public void SelectedState(bool repositionColliderOnEnd = false)
    {
        cardState = CardPartStates.SELECTED;

        CardTransform.DOMove(selectedPosition, BuildingCard.selectedTime)
            .OnComplete(() => { if (repositionColliderOnEnd) RepositionColliderToCardTransform(); });
    }

    public void RepositionColliderToCardTransform()
    {
        cardCollider.center = cardColliderOffset + CardTransform.localPosition;
    }


    protected abstract void InitInfoVisuals();
    protected abstract void SetupCardInfo();
    public virtual void ShowInfo()
    {
        isShowingInfo = true;
        Debug.Log("ShowInfo");
    }
    public virtual void HideInfo()
    {
        isShowingInfo = false;
        Debug.Log("HideInfo");
    }
}
