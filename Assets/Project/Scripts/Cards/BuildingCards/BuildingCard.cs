using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections;
using UnityEngine.UI;

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


    [Header("BUILDING PREFAB")]
    [SerializeField] public GameObject buildingPrefab;
    [HideInInspector] public GameObject copyBuildingPrefab;
    public bool AlreadySpawnedCopyBuildingPrefab => copyBuildingPrefab != null;


    [Header("CANVAS COMPONENTS")]
    [SerializeField] protected TextMeshProUGUI playCostText;

    [Header("OTHER COMPONENTS")]
    [SerializeField] private BoxCollider cardCollider;
    private Vector3 cardColliderOffset;
    [SerializeField] private Transform cardHolder;
    public Transform RootCardTransform => transform;
    public Transform CardTransform => cardHolder;

    public const float unhoverTime = 0.05f;
    public const float hoverTime = 0.02f; // This numebr needs to be VERY SMALL
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

    private Vector3 HoveredTranslation => CardTransform.up * 0.2f + CardTransform.forward * -0.14f;
    private Vector3 HoveredTranslationWorld => Vector3.up * 0.2f + Vector3.forward * -0.14f;
    public Vector3 SelectedPosition => CardTransform.position + (CardTransform.up * 1.3f) + (-CardTransform.right * 1.3f);


    [HideInInspector] public bool isShowingInfo = false;
    protected bool canInfoInteract = true;

    [Header("VISUALS")]
    [SerializeField] private MeshRenderer cardMeshRenderer;
    [SerializeField] protected CanvasGroup interfaceCanvasGroup;
    private Material cardMaterial;

    [Header("CARD INFO")]
    [SerializeField] protected CanvasGroup[] cgsInfoHide;
    protected Coroutine showInfoCoroutine = null;
    protected bool isShowInfoAnimationPlaying = false;
    protected bool isHideInfoAnimationPlaying = false;

    // CARD DRAW ANIMATION
    [SerializeField] protected CanvasGroup[] otherCfDrawAnimation;    
    private bool isPlayingDrawAnimation = false;
    private const float drawAnimLoopDuration = 0.75f;
    private const float drawAnimWaitDurationBeforeBlink = 0.2f;
    private const float drawAnimBlinkDuration = 0.3f;
    private const float drawAnimNumBlinks = 3f;

    // CARD CAN NOT BE PLAYED ANIMATION
    private const float canNotBePlayedAnimDuration = 0.5f;


    public delegate void BuildingCardAction(BuildingCard buildingCard);
    public event BuildingCardAction OnCardHovered;
    public event BuildingCardAction OnCardUnhovered;
    public event BuildingCardAction OnCardSelected;
    public event BuildingCardAction OnCardInfoSelected;

    public event BuildingCardAction OnCardSelectedNotHovered;
    public event BuildingCardAction OnGetSaved;

    public bool AlreadyCanBeHovered => OnCardHovered != null;


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

    private void OnMouseEnter()
    {
        if (isRepositioning) return;
        if (isPlayingDrawAnimation) return;

        if (cardState != CardStates.STANDARD) return;

        if (OnCardHovered != null) OnCardHovered(this);
    }

    private void OnMouseExit()
    {
        if (isRepositioning) return;
        if (isPlayingDrawAnimation) return;

        if (cardState != CardStates.HOVERED) return;

        if (OnCardUnhovered != null) OnCardUnhovered(this);
    }

    private void OnMouseDown() // only called by Left Click
    {
        if (isRepositioning) return;
        if (isPlayingDrawAnimation) return;
        
        if (cardState == CardStates.HOVERED)
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
    protected abstract void GetMaterialsRefs();
    protected abstract void InitVisuals();



    protected virtual void AwakeInit(CardBuildingType cardBuildingType)
    {
        cardColliderOffset = cardCollider.center;

        this.cardBuildingType = cardBuildingType;
        GetMaterialsRefs();

        cardMaterial = cardMeshRenderer.material;
        SetCannotBePlayedAnimation();
        cardMaterial.SetFloat("_RandomTimeAdd", Random.Range(0f, Mathf.PI));

        cardMaterial.SetFloat("_BorderLoopEnabled", 0f);
        cardMaterial.SetFloat("_IsSmoothLooping", 0f);

        cardMaterial.SetFloat("_LoopDuration", drawAnimLoopDuration);
        cardMaterial.SetFloat("_WaitBeforeBlink", drawAnimWaitDurationBeforeBlink);
        cardMaterial.SetFloat("_BlinkDuration", drawAnimBlinkDuration);
        cardMaterial.SetFloat("_NumBlinks", drawAnimNumBlinks);

        cardMaterial.SetFloat("_CanNotBePlayedDuration", canNotBePlayedAnimDuration);

        isShowingInfo = false;
    }


    protected void Init()
    {
        initialPosition = CardTransform.position;

        InitStatsFromTurretParts();
        InitCostText();

        InitVisuals();
    }

    protected void InitCostText()
    {
        playCostText.text = GetCardPlayCost().ToString();
    }


    // CARD MOVEMENT
    public void StartRepositioning(Vector3 finalPosition, float duration)
    {
        isRepositioning = true;

        ImmediateStandardState();/////

        RootCardTransform.DOMove(finalPosition, duration)
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

    public void InitPositions(Vector3 selectedPosition, Vector3 hiddenDisplacement)
    {
        //ResetCardPosition();

        local_standardPosition = CardTransform.localPosition;
        local_hoveredPosition = CardTransform.localPosition + HoveredTranslation;
        //local_selectedPosition = CardTransform.InverseTransformPoint(selectedPosition);
        startRotation_euler = transform.rotation.eulerAngles;
        local_standardRotation_euler = transform.rotation.eulerAngles;
        this.hiddenDisplacement = hiddenDisplacement;

        standardPosition = CardTransform.position;
        hoveredPosition = CardTransform.position + HoveredTranslation;
        this.selectedPosition = selectedPosition;
    }

    public void ImmediateStandardState()
    {
        cardState = CardStates.STANDARD;
        //CardTransform.DOComplete(true);
        CardTransform.localPosition = local_standardPosition;
        //CardTransform.localRotation = Quaternion.Euler(local_standardRotation_euler);
    }
    public void StandardState(bool repositionColliderOnEnd = false)
    {
        cardState = CardStates.STANDARD;

        DisableMouseInteraction();

        CardTransform.DOComplete(true);
        CardTransform.DOBlendableLocalMoveBy(local_standardPosition - CardTransform.localPosition, unhoverTime);
        CardTransform.DOBlendableLocalRotateBy(local_standardRotation_euler - CardTransform.rotation.eulerAngles, unhoverTime)
            .OnComplete(() => {
                EnableMouseInteraction();
                if (repositionColliderOnEnd) RepositionColliderToCardTransform();
            });
    }

    public void HoveredState()
    {
        cardState = CardStates.HOVERED;

        CardTransform.DOComplete(true);
        CardTransform.DOBlendableLocalMoveBy(CardTransform.localRotation * HoveredTranslationWorld, hoverTime);
    }

    public void SelectedState(bool repositionColliderOnEnd = false)
    {
        cardState = CardStates.SELECTED;

        DisableMouseInteraction();

        CardTransform.DOComplete(true);
        CardTransform.DOBlendableMoveBy(selectedPosition - CardTransform.position, selectedTime);
        CardTransform.DOBlendableLocalRotateBy(startRotation_euler - CardTransform.rotation.eulerAngles, selectedTime)
            .OnComplete(() => { EnableMouseInteraction(); 
                                if (repositionColliderOnEnd) RepositionColliderToCardTransform(); 
            });
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
        cardMaterial.SetFloat("_CanBePlayed", 1f);
    }

    public void SetCannotBePlayedAnimation()
    {
        cardMaterial.SetFloat("_CanBePlayed", 0f);
    }

    public void EnableMouseInteraction()
    {
        cardCollider.enabled = true;
    }
    public void DisableMouseInteraction()
    {
        cardCollider.enabled = false;
    }

    private IEnumerator ScuffedreinableMouseInteraction()
    {
        DisableMouseInteraction();
        yield return null;
        EnableMouseInteraction();

        if (cardState == CardStates.HOVERED) 
            if (OnCardUnhovered != null) OnCardUnhovered(this);
    }

    protected abstract void InitInfoVisuals();
    protected abstract void SetupCardInfo();
    public virtual void ShowInfo()
    {
        isShowingInfo = true;
        //Debug.Log("ShowInfo");
    }
    public virtual void HideInfo()
    {
        isShowingInfo = false;
        //Debug.Log("HideInfo");
    }


    public void PlayDrawAnimation()
    {
        cardMaterial.SetFloat("_BorderLoopEnabled", 1f);
        cardMaterial.SetFloat("_TimeStartBorderLoop", Time.time);

        StartCoroutine(InterfaceDrawAnimation());
    }

    private IEnumerator InterfaceDrawAnimation()
    {
        canInfoInteract = false;
        isPlayingDrawAnimation = true;


        for (int i = 0; i < cgsInfoHide.Length; ++i)
        {
            cgsInfoHide[i].alpha = 0f;
        }
        for (int i = 0; i < otherCfDrawAnimation.Length; ++i)
        {
            otherCfDrawAnimation[i].alpha = 0f;
        }


        float startStep = 10f;
        float step = startStep;
        float stepDec = startStep * 0.02f;
        for (float t = 0f; t < drawAnimLoopDuration; t+= Time.deltaTime * step)
        {
            GameAudioManager.GetInstance().PlayCardInfoMoveShown();
            yield return new WaitForSeconds(Time.deltaTime * step);

            step -= stepDec;
        }


        yield return new WaitForSeconds(drawAnimWaitDurationBeforeBlink);


        float tBlink = drawAnimBlinkDuration / drawAnimNumBlinks;
        for (int i = 0; i < drawAnimNumBlinks; ++i)
        {
            GameAudioManager.GetInstance().PlayCardInfoMoveHidden();
            yield return new WaitForSeconds(tBlink);
        }


        float t1 = 0.1f;
        for (int i = cgsInfoHide.Length-1; i >= 0; --i)
        {
            cgsInfoHide[i].DOFade(1f, t1);
            GameAudioManager.GetInstance().PlayCardInfoShown();
            yield return new WaitForSeconds(t1);
        }
        for (int i = otherCfDrawAnimation.Length - 1; i >= 0; --i)
        {
            otherCfDrawAnimation[i].DOFade(1f, t1);
            GameAudioManager.GetInstance().PlayCardInfoShown();
            yield return new WaitForSeconds(t1);
        }


        canInfoInteract = true;
        isPlayingDrawAnimation = false;
        cardMaterial.SetFloat("_BorderLoopEnabled", 0f);

        DisableMouseInteraction();
        yield return null;
        EnableMouseInteraction();
    }


    public void PlayCanNotBePlayedAnimation()
    {
        SetCannotBePlayedAnimation();
        cardMaterial.SetFloat("_TimeStartCanNotBePlayed", Time.time);

        CardTransform.DOComplete(true);
        CardTransform.DOPunchRotation(CardTransform.forward * 10f, canNotBePlayedAnimDuration, 8, 0.8f);
    }

}
