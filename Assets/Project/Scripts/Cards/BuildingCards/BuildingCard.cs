using System;
using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections;
using UnityEngine.UI;
using static CardPart;
using Random = UnityEngine.Random;

public abstract class BuildingCard : MonoBehaviour
{
    public static float halfWidth = 0.7f;


    public enum CardBuildingType { NONE, TURRET, SUPPORT }
    public CardBuildingType cardBuildingType { get; protected set; }


    public enum CardLocation { NONE, DECK, HAND }
    [HideInInspector] public CardLocation cardLocation = CardLocation.NONE;
    private bool isRepositioning = false;
    public bool IsRepositioning => isRepositioning;

    public enum CardStates { STANDARD, HOVERED, SELECTED }
    [HideInInspector] public CardStates cardState = CardStates.STANDARD;

    [Header("MOTION")]
    [SerializeField] private CardMotionConfig _motionConfig;
    [SerializeField] private CardMotionEffectsController _motionEffectsController;
    public CardMotionEffectsController MotionEffectsController => _motionEffectsController;

    [Header("BUILDING PREFAB")]
    [SerializeField] public GameObject buildingPrefab;
    [HideInInspector] public GameObject copyBuildingPrefab;

    public bool AlreadySpawnedCopyBuildingPrefab => copyBuildingPrefab != null;


    [Header("CANVAS COMPONENTS")]
    [SerializeField] protected TextMeshProUGUI playCostText;
    [SerializeField] protected Image playCostCurrencyIcon;
    private static Color s_canNotPlayCardTextColor = new Color(202f/255f, 35f/255f, 54f/255f);

    [Header("OTHER COMPONENTS")]
    [SerializeField] private BoxCollider cardCollider;
    private Vector3 cardColliderOffset;
    [SerializeField] private Transform cardHolder;
    public Transform RootCardTransform => transform;
    public Transform CardTransform => cardHolder;

    public const float unhoverTime = 0.1f;
    public const float hoverTime = 0.05f; // This number needs to be VERY SMALL
    public const float toStandardTime = 0.1f;
    public const float selectedTime = 0.3f;

    private Vector3 initialPosition;
    private Vector3 standardPosition;
    private Vector3 hoveredPosition;
    private Vector3 selectedPosition;

    private Vector3 local_standardPosition;
    private Vector3 local_hoveredPosition;
    private Vector3 local_selectedPosition;
    private Vector3 hiddenDisplacement;
    private Vector3 startRotation_euler;
    private Vector3 local_standardRotation_euler;

    private Vector3 hiddenRootPosition;
    private Vector3 shownRootPosition;
    public Vector3 ShownRootPosition => shownRootPosition;
    public Vector3 HiddenRootPosition => hiddenRootPosition;

    public Vector3 hoverAdditionalOffset = Vector3.zero;

    private Vector3 HoveredTranslation => CardTransform.up * 0.2f + CardTransform.forward * -0.14f;
    public Vector3 HoveredTranslationWorld => _motionConfig.CurrentDisplacements.Hovered;
    public Vector3 SelectedPosition => CardTransform.position + (CardTransform.up * 1.3f) + (-CardTransform.right * 1.3f);



    [Header("DRAG & DROP")]
    [SerializeField] private LayerMask layerMaskMouseDragPlane;
    private bool isDraggingToSelect = false;
    public static Camera MouseDragCamera;
    public static Bounds DragStartBounds;


    [HideInInspector] public bool isShowingInfo = false;
    protected bool canInfoInteract = true;

    [Header("VISUALS")]
    [SerializeField] private BuildingCardView _view;
    [SerializeField] private MeshRenderer cardMeshRenderer;
    [SerializeField] private MeshRenderer discardIndicatorMesh;
    protected Material cardMaterial;
    protected Material discardIndicatorMaterial;

    [Header("CARD INFO")]
    [HideInInspector] public bool canDisplayInfoIfNotInteractable = false;
    [HideInInspector] public bool canDisplayInfoIfWhileInteractable = true;
    [HideInInspector] public bool hideInfoWhenSelected = true;

    // CARD DRAW ANIMATION
    protected bool isPlayingDrawAnimation = false;
    protected const float drawAnimLoopDuration = 0.4f;
    protected const float drawAnimWaitDurationBeforeBlink = 0.1f;
    protected const float drawAnimBlinkDuration = 0.2f;
    protected const float drawAnimNumBlinks = 2f;

    // CARD CAN NOT BE PLAYED ANIMATION
    private const float canNotBePlayedAnimDuration = 0.5f;

    public const float redrawHoldDuration = 0.5f;

    [Header("DESCRIPTION")] 
    [SerializeField] protected CardTooltipDisplayData.Positioning _descriptionTooltipPositioning;
    


    [HideInInspector] public bool isMissingDefaultCallbacks = false;
    public delegate void BuildingCardAction(BuildingCard buildingCard);
    public event BuildingCardAction OnCardHovered;
    public event BuildingCardAction OnCardUnhovered;
    public event BuildingCardAction OnCardSelected;
    public event BuildingCardAction OnCardInfoSelected;
    public event BuildingCardAction OnDragMouseUp;
    public static event BuildingCardAction OnDragOutsideDragBounds;


    public bool IsOnCardHoveredSubscrived => OnCardHovered != null;

    public event BuildingCardAction OnCardSelectedNotHovered;
    public event BuildingCardAction OnGetSaved;


    public delegate void BuildingCardAction2();
    public static event BuildingCardAction2 OnInfoShown;

    public static event BuildingCardAction2 OnMouseDragStart;
    public static event BuildingCardAction2 OnMouseDragEnd;


    public delegate void CardFunctionPtr();

    public bool AlreadyCanBeHovered => OnCardHovered != null;

    [HideInInspector] public bool isInteractable = true;
    [HideInInspector] public bool canBeHovered = true;


    /////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////
    
    //get the mesh prefab from the same place where you get the sprite
    //animation appear --> use the PreviewMat
    //assign the material throught the same functions used inbattle
    
    /////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////



    // MonoBehaviour methods
    private void OnEnable()
    {        
        CardPartReplaceManager.OnReplacementDone += InvokeGetSaved;
    }

    private void OnDisable()
    {
        CardPartReplaceManager.OnReplacementDone -= InvokeGetSaved;
    }

    private void Awake()
    {
        AwakeInit(CardBuildingType.NONE);
    }

    private void OnDestroy()
    {
        HideInfo();
        DoOnDestroy();
    }
    
    protected virtual void DoOnDestroy(){}

    private void OnMouseEnter()
    {
        if (canDisplayInfoIfNotInteractable)
        {
            StartShowInfoWithDelay();
            return;
        }

        if (!canBeHovered) return;
        if (isRepositioning) return;
        //if (isPlayingDrawAnimation) return;

        if (cardState != CardStates.STANDARD) return;

        if (OnCardHovered != null) OnCardHovered(this);

        if (cardState == CardStates.HOVERED && canDisplayInfoIfWhileInteractable)
        {
            StartShowInfoWithDelay();
        }
    }

    private void OnMouseExit()
    {
        if (canDisplayInfoIfNotInteractable)
        {
            DoHideInfo();
            return;
        }

        if (!canBeHovered) return;
        if (isRepositioning) return;
        //if (isPlayingDrawAnimation) return;

        if (cardState != CardStates.HOVERED) return;

        if (OnCardUnhovered != null) OnCardUnhovered(this);

        if (canDisplayInfoIfWhileInteractable)
        {
            DoHideInfo();
        }
    }

    private void OnMouseDown() // only called by Left Click
    {
        if (!canBeHovered) return;
        if (isRepositioning) return;
        if (!isInteractable) return;
        if (isPlayingDrawAnimation) return;

        if (cardState == CardStates.HOVERED)
        {
            if (OnCardSelected != null) OnCardSelected(this);
        }
        else
        {
            if (OnCardSelectedNotHovered != null) OnCardSelectedNotHovered(this);
        }

        if (hideInfoWhenSelected)
        {
            DoHideInfo();
        }
    }

    private void Update()
    {
        if (canBeHovered && canInfoInteract && Input.GetMouseButtonDown(1) && isInteractable)
        {
            if (cardState == CardStates.HOVERED)
            {
                if (OnCardInfoSelected != null) OnCardInfoSelected(this);
            }
        }
    }


    // ABSTRACT METHODS to implement
    protected abstract void InitStatsFromTurretParts();
    public abstract void CreateCopyBuildingPrefab(Transform spawnTransform, CurrencyCounter currencyCounter);
    public abstract int GetCardPlayCost();
    protected abstract void InitVisuals();



    protected virtual void AwakeInit(CardBuildingType cardBuildingType)
    {
        _motionEffectsController.Init(_motionConfig.IdleRotationEffect, _motionConfig.HoveredMouseRotationEffect);

        cardColliderOffset = cardCollider.center;

        this.cardBuildingType = cardBuildingType;

        cardMaterial = cardMeshRenderer.material;
        _view.Configure();

        if(discardIndicatorMesh != null)
        {
            discardIndicatorMaterial = discardIndicatorMesh.material;
        }
        SetCannotBePlayedAnimation(false);
        cardMaterial.SetFloat("_RandomTimeAdd", Random.Range(0f, Mathf.PI));

        cardMaterial.SetFloat("_BorderLoopEnabled", 0f);
        cardMaterial.SetFloat("_IsSmoothLooping", 0f);

        cardMaterial.SetFloat("_LoopDuration", drawAnimLoopDuration);
        cardMaterial.SetFloat("_WaitBeforeBlink", drawAnimWaitDurationBeforeBlink);
        cardMaterial.SetFloat("_BlinkDuration", drawAnimBlinkDuration);
        cardMaterial.SetFloat("_NumBlinks", drawAnimNumBlinks);

        cardMaterial.SetFloat("_CanNotBePlayedDuration", canNotBePlayedAnimDuration);
        
        
        SetBorderFillValue(0f);
        SetBorderFillEnabled(false);

        isShowingInfo = false;

        isInteractable = true;

        cardState = CardStates.HOVERED;
        SetCardState(CardStates.STANDARD);
    }


    protected void Init()
    {
        initialPosition = CardTransform.position;
        _view.Configure();

        InitStatsFromTurretParts();
        InitCostText();

        InitVisuals();
    }

    protected void InitCostText()
    {
        playCostText.text = GetCardPlayCost().ToString();
    }
    public abstract void UpdatePlayCost(int newPlayCost);


    // CARD MOVEMENT
    public void StartRepositioning(Vector3 finalPosition, float duration)
    {
        isRepositioning = true;

        ImmediateStandardState();/////

        RootCardTransform.DOMove(finalPosition, duration)
            .SetEase(_motionConfig.Repositioning_Move_Ease)
            .OnComplete(EndRepositioning);
    }
    private void EndRepositioning()
    {
        StartCoroutine(ScuffedreinableMouseInteraction()); // not working 
        isRepositioning = false;
    }
    public void ForceEndRepositioning()
    {
        RootCardTransform.DOComplete(true);
    }

    public void ResetCardPosition()
    {
        CardTransform.localPosition = Vector3.zero;
    }

    public void InitPositions(Vector3 selectedPosition, Vector3 hiddenDisplacement, Vector3 finalPosition)
    {
        InitPositions(CardTransform.localPosition, selectedPosition, hiddenDisplacement, finalPosition);
    }
    public void InitPositions(Vector3 standardLocalPosition, Vector3 selectedPosition, Vector3 hiddenDisplacement, Vector3 finalPosition)
    {
        //ResetCardPosition();

        local_standardPosition = standardLocalPosition;
        local_hoveredPosition = local_standardPosition + HoveredTranslation;
        //local_selectedPosition = CardTransform.InverseTransformPoint(selectedPosition);
        startRotation_euler = transform.rotation.eulerAngles;
        local_standardRotation_euler = transform.rotation.eulerAngles;
        this.hiddenDisplacement = hiddenDisplacement;

        shownRootPosition = finalPosition;
        //shownRootPosition = RootCardTransform.position;
        hiddenRootPosition = shownRootPosition + hiddenDisplacement;
                
        standardPosition = CardTransform.position;
        hoveredPosition = standardPosition + HoveredTranslation;

        this.selectedPosition = selectedPosition;
    }

    public void ImmediateStandardState()
    {
        SetCardState(CardStates.STANDARD);
        //CardTransform.DOComplete(true);
        CardTransform.localPosition = local_standardPosition;
        //CardTransform.localRotation = Quaternion.Euler(local_standardRotation_euler);
    }
    public void StandardState(bool repositionColliderOnEnd = false, float duration = BuildingCard.unhoverTime)
    {
        SetCardState(CardStates.STANDARD);

        DisableMouseInteraction();

        CardTransform.DOComplete(true);
        CardTransform.DOBlendableLocalMoveBy(local_standardPosition - CardTransform.localPosition, unhoverTime)
            .SetEase(_motionConfig.ToStandard_Move_Ease);
        CardTransform.DOBlendableLocalRotateBy(local_standardRotation_euler - CardTransform.rotation.eulerAngles, unhoverTime)
            .SetEase(_motionConfig.ToStandard_Rot_Ease)
            .OnComplete(() => {
                EnableMouseInteraction();
                if (repositionColliderOnEnd) RepositionColliderToCardTransform();
            });
    }

    public void HoveredStateRedraw(bool rotate = true, bool useAdditionalOffset = false)
    {
        HoveredState(rotate, useAdditionalOffset);
        RedrawHoverIndication(true);
    }

    public void HoveredState(bool rotate = true, bool useAdditionalOffset = false)
    {
        SetCardState(CardStates.HOVERED);

        Vector3 moveBy = (CardTransform.localRotation * HoveredTranslationWorld);
        if (useAdditionalOffset) moveBy += hoverAdditionalOffset;

        CardTransform.DOComplete(true);
        CardTransform.DOBlendableLocalMoveBy(moveBy, hoverTime)
            .SetEase(_motionConfig.Hovered_Move_Ease);

        if (rotate)
        {
            CardTransform.DOBlendableRotateBy(-RootCardTransform.localRotation.eulerAngles, hoverTime)
                .SetEase(_motionConfig.Hovered_Rot_Ease);
        }
    }

    private void SetCardState(CardStates newCardState)
    {
        if (cardState == CardStates.HOVERED)
        {
            _motionEffectsController.FinishHoverMotion();
        }

        if (newCardState == CardStates.HOVERED)
        {
            _motionEffectsController.StartHoverMotion();
        }

        cardState = newCardState;
    }


    bool repositionColliderOnEnd, enableInteractionOnEnd = false;
    public void SelectedState(bool useDragAndDrop, bool repositionColliderOnEnd = false, bool enableInteractionOnEnd = false)
    {
        SetCardState(CardStates.SELECTED);

        this.repositionColliderOnEnd = repositionColliderOnEnd;
        this.enableInteractionOnEnd = enableInteractionOnEnd;

        if (useDragAndDrop)
        {
            isDraggingToSelect = true;
            //Debug.Log("isDraggingToSelect");
            if (OnMouseDragStart != null) OnMouseDragStart();
        }
        else
        {
            GoToSelectedPosition();
        }
    }
    public void GoToSelectedPosition()
    {
        DisableMouseInteraction();

        CardTransform.DOComplete(true);
        CardTransform.DOBlendableMoveBy(selectedPosition - CardTransform.position, selectedTime)
            .SetEase(_motionConfig.Selected_Move_Ease)
            .OnComplete(() => {
            if (enableInteractionOnEnd) EnableMouseInteraction();
            if (repositionColliderOnEnd) RepositionColliderToCardTransform();
        });
        //CardTransform.DOBlendableLocalRotateBy(startRotation_euler - CardTransform.rotation.eulerAngles, selectedTime);
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

    // CARD MOVEMENT end


    private void InvokeGetSaved()
    {
        if (OnGetSaved != null) OnGetSaved(this);
    }


    public void SetCanBePlayedAnimation()
    {
        _view.SetCanBePlayed(true);
        playCostText.DOColor(Color.white, 0.2f);
        playCostCurrencyIcon.DOColor(Color.white, 0.2f);
    }

    public void SetCannotBePlayedAnimation(bool updatePlayCostText)
    {
        _view.SetCanBePlayed(false);

        if (updatePlayCostText)
        {
            playCostText.DOColor(s_canNotPlayCardTextColor, 0.2f);
            playCostCurrencyIcon.DOColor(s_canNotPlayCardTextColor, 0.2f);
        }        
    }


    public float borderFillValue01 = 0f;
    private Coroutine decreaseBorderFillCoroutine;
    public void RedrawHoverIndication(bool isEnabled)
    {
        float value = isEnabled ? 1f : 0f;
        if (discardIndicatorMaterial != null)
        {
            discardIndicatorMaterial.SetFloat("_Appear", value);
        }
    }
    public void SetBorderFillEnabled(bool isEnabled)
    {
        float value = isEnabled ? 1f : 0f;
        cardMaterial.SetFloat("_IsBorderFillEnabled", value);
        if (discardIndicatorMaterial != null)
        {
            discardIndicatorMaterial.SetFloat("_Fill", value);
            discardIndicatorMaterial.SetFloat("_Appear", value);
        }
    }
    public void SetBorderFillValue(float fillValue01)
    {
        cardMaterial.SetFloat("_BorderFillValue", fillValue01);
        if(discardIndicatorMaterial != null)
        {
            discardIndicatorMaterial.SetFloat("_Fill", fillValue01);
        }
    }
    public void ResetBorderFill()
    {
        if (decreaseBorderFillCoroutine != null)
        {
            StopCoroutine(decreaseBorderFillCoroutine);            
        }
        else
        {
            borderFillValue01 = 0f;
            if (discardIndicatorMaterial != null)
            {
                discardIndicatorMaterial.SetFloat("_Fill", 0);
            }
        }
    }
    public void StartDecreaseBorderFill()
    {
        decreaseBorderFillCoroutine = StartCoroutine(DecreaseBorderFill());
    }
    private IEnumerator DecreaseBorderFill()
    {
        while (borderFillValue01 > 0f)
        {
            borderFillValue01 -= Time.deltaTime * (2.0f / redrawHoldDuration );
            SetBorderFillValue(borderFillValue01);

            yield return null;
        }

        borderFillValue01 = 0f;

        SetBorderFillValue(borderFillValue01);
        SetBorderFillEnabled(false);

        decreaseBorderFillCoroutine = null;
    }


    public void EnableMouseInteraction()
    {
        cardCollider.enabled = true;
        //Debug.Log("Interaction ON");
    }
    public void DisableMouseInteraction()
    {
        cardCollider.enabled = false;
        //Debug.Log("Interaction OFF");
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

        if (cardState == CardStates.HOVERED) 
            if (OnCardUnhovered != null) OnCardUnhovered(this);
    }

    public virtual int GetCardLevel()
    { 
        return 0;
    }

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


    private Coroutine showInfoDelayCoroutine = null;
    private void StartShowInfoWithDelay()
    {
        showInfoDelayCoroutine = StartCoroutine(ShowInfoWithDelay());
    }
    private IEnumerator ShowInfoWithDelay()
    {
        yield return new WaitForSeconds(0.1f);

        if (cardState != CardStates.HOVERED && !canDisplayInfoIfNotInteractable) yield break;

        ShowInfo();
        showInfoDelayCoroutine = null;
    }
    private void DoHideInfo()
    {
        if (showInfoDelayCoroutine != null) StopCoroutine(showInfoDelayCoroutine);
        HideInfo();
    }


    public void PlayDrawAnimation(CardFunctionPtr animationEndCallback)
    {
        StartCoroutine(InterfaceDrawAnimation(animationEndCallback));
    }

    protected virtual IEnumerator InterfaceDrawAnimation(CardFunctionPtr animationEndCallback)
    {
        canInfoInteract = false;
        isPlayingDrawAnimation = true;
        
        yield return StartCoroutine(_view.DrawAnimationPlayer.PlayDrawAnimation());
        
        canInfoInteract = true;
        isPlayingDrawAnimation = false;

        animationEndCallback();
    }

    public void PlayCanNotBePlayedAnimation()
    {
        SetCannotBePlayedAnimation(true);
        _view.PlayCanNotBePlayedAnimation();

        CardTransform.DOComplete(true);
        CardTransform.DOPunchRotation(CardTransform.forward * 10f, canNotBePlayedAnimDuration, 8, 0.8f);
    }


    public void StartDisableInfoDisplayForDuration(float duration)
    {
        StartCoroutine(DisableInfoDisplayForDuration(duration));
    }
    private IEnumerator DisableInfoDisplayForDuration(float duration)
    {
        canDisplayInfoIfWhileInteractable = false;

        yield return new WaitForSeconds(duration);

        canDisplayInfoIfWhileInteractable = true;

        if (cardState == CardStates.HOVERED)
        {
            StartShowInfoWithDelay();
        }
    }

    protected void UpdateCardLevelView()
    {
        _view.UpdateCardLevelView(GetCardLevel());
    }
}
