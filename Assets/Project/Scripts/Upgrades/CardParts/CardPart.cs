using UnityEngine;
using DG.Tweening;
using System.Collections;


public abstract class CardPart : MonoBehaviour
{
    public static float halfWidth = 0.7f;

    public enum CardPartStates { STANDARD, HOVERED, SELECTED }
    [HideInInspector] public CardPartStates cardState = CardPartStates.STANDARD;

    [Header("DRAG & DROP")]
    [SerializeField] private LayerMask layerMaskMouseDragPlane;
    private bool isDraggingToSelect = false;
    public static Camera MouseDragCamera;
    public static Bounds DragStartBounds;

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
    [HideInInspector] public bool isInteractable = true;


    public delegate void BuildingCardPartAction(CardPart cardPart);
    public static event BuildingCardPartAction OnCardHovered;
    public static event BuildingCardPartAction OnCardUnhovered;
    public static event BuildingCardPartAction OnCardSelected;
    public event BuildingCardPartAction OnCardInfoSelected;

    public event BuildingCardPartAction OnCardSelectedNotHovered;

    public event BuildingCardPartAction OnDragOutsideDragBounds;
    public event BuildingCardPartAction OnDragMouseUp;


    public delegate void BuildingCardPartAction2();
    public static event BuildingCardPartAction2 OnInfoShown;

    public static event BuildingCardPartAction2 OnMouseDragStart;
    public static event BuildingCardPartAction2 OnMouseDragEnd;



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
        if (!isInteractable) return;

        if (cardState != CardPartStates.STANDARD) return;

        if (OnCardHovered != null) OnCardHovered(this);
    }

    private void OnMouseExit()
    {
        if (!isInteractable) return;

        if (cardState != CardPartStates.HOVERED) return;

        if (OnCardUnhovered != null) OnCardUnhovered(this);
    }

    private void OnMouseDown()
    {
        if (!isInteractable) return;

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
        if (canInfoInteract && Input.GetMouseButtonDown(1) && isInteractable)
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

    public void StandardState(bool repositionColliderOnEnd = false, float duration = BuildingCard.toStandardTime)
    {
        cardState = CardPartStates.STANDARD;

        CardTransform.DOComplete(true);
        CardTransform.DOBlendableMoveBy(standardPosition - CardTransform.position, duration)        
            .OnComplete(() => { if (repositionColliderOnEnd) RepositionColliderToCardTransform(); });
    }

    public void HoveredState()
    {
        cardState = CardPartStates.HOVERED;

        CardTransform.DOBlendableLocalMoveBy(CardTransform.localRotation * BuildingCard.HoveredTranslationWorld, BuildingCard.hoverTime);
    }

    bool repositionColliderOnEnd = false;
    public void SelectedState(bool useDragAndDrop, bool repositionColliderOnEnd = false)
    {
        cardState = CardPartStates.SELECTED;

        this.repositionColliderOnEnd = repositionColliderOnEnd;
        
        if (useDragAndDrop)
        {
            isDraggingToSelect = true;
            if (OnMouseDragStart != null) OnMouseDragStart();
        }
        else
        {
            GoToSelectedPosition();
        }
    }

    public void GoToSelectedPosition()
    {
        CardTransform.DOBlendableMoveBy(selectedPosition - CardTransform.position, BuildingCard.selectedTime)
            .OnComplete(() => { if (repositionColliderOnEnd) RepositionColliderToCardTransform(); });
    }

    private void OnMouseDrag()
    {
        if (isDraggingToSelect)
        {
            Ray ray = MouseDragCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, layerMaskMouseDragPlane))
            {
                Vector3 goalPosition = hit.point + (hit.normal * 0.1f);
                float distance = Vector3.Distance(CardTransform.position, goalPosition);
                if (distance > 0.05f)
                {
                    float speed = 10f * Mathf.Clamp(distance * 2f, 0.1f, 10f);
                    Vector3 dir = (goalPosition - CardTransform.position).normalized;
                    CardTransform.position = CardTransform.position + (dir * Time.deltaTime * speed);
                }


                if (DragStartBounds.Contains(goalPosition))
                {
                    //Debug.Log("inside bounds");
                    //if (Vector3.Distance())
                }
                else
                {
                    //Debug.Log("outside bounds");
                    isDraggingToSelect = false;
                    GoToSelectedPosition();
                    if (OnDragOutsideDragBounds != null) OnDragOutsideDragBounds(this);
                    if (OnMouseDragEnd != null) OnMouseDragEnd();
                }
            }
        }

    }
    private void OnMouseUp()
    {
        if (isDraggingToSelect)
        {
            //Debug.Log("STOPPED Dragging");
            isDraggingToSelect = false;
            if (OnDragMouseUp != null) OnDragMouseUp(this);
            if (OnMouseDragEnd != null) OnMouseDragEnd();
        }
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
        //Debug.Log("ShowInfo");
        if (OnInfoShown != null) OnInfoShown();
    }
    public virtual void HideInfo()
    {
        isShowingInfo = false;
        //Debug.Log("HideInfo");
    }


    public void EnableMouseInteraction()
    {
        cardCollider.enabled = true;
    }
    public void DisableMouseInteraction()
    {
        cardCollider.enabled = false;
    }

    public void ReenableMouseInteraction()
    {
        StartCoroutine(ScuffedreinableMouseInteraction());
    }
    private IEnumerator ScuffedreinableMouseInteraction()
    {
        DisableMouseInteraction();
        yield return null;
        EnableMouseInteraction();

        if (cardState == CardPartStates.HOVERED)
            if (OnCardUnhovered != null) OnCardUnhovered(this);
    }

}
