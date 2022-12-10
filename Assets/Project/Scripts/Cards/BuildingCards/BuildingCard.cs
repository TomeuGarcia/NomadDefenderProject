using UnityEngine;
using TMPro;
using static BuildingCard;
using DG.Tweening;

public abstract class BuildingCard : MonoBehaviour
{
    public static float halfWidth = 0.7f;


    public enum CardBuildingType { NONE, TURRET }
    public CardBuildingType cardBuildingType { get; protected set; }


    public enum CardStates { STANDARD, HOVERED, SELECTED }
    [HideInInspector] public CardStates cardState = CardStates.STANDARD;


    [Header("BUILDING PREFAB")]
    [SerializeField] public GameObject buildingPrefab;
    [HideInInspector] public GameObject copyBuildingPrefab;
    public bool AlreadySpawnedCopyBuildingPrefab => copyBuildingPrefab != null;


    [Header("CANVAS COMPONENTS")]
    [SerializeField] private TextMeshProUGUI playCostText;

    [Header("OTHER COMPONENTS")]
    [SerializeField] private BoxCollider cardCollider;
    [SerializeField] private Transform cardHolder;
    public Transform CardTransform => cardHolder;

    private const float unhoverTime = 0.05f;
    private const float hoverTime = 0.02f; // This numebr needs to be VERY SMALL
    private const float selectedTime = 0.3f;


    private Vector3 initialPosition;
    private Vector3 standardPosition;
    private Vector3 hoveredPosition;
    private Vector3 selectedPosition;

    private Vector3 local_standardPosition;
    private Vector3 local_hoveredPosition;
    private Vector3 local_selectedPosition;
    private Vector3 hiddenDisplacement;
    private Vector3 local_standardRotation_euler;

    private Vector3 HoveredTranslation => CardTransform.up * 0.2f + CardTransform.forward * -0.14f;
    public Vector3 SelectedPosition => CardTransform.position + (CardTransform.up * 1.3f) + (-CardTransform.right * 1.3f);


    [Header("VISUALS")]
    [SerializeField] private MeshRenderer cardMeshRenderer;
    private Material cardMaterial;


    public delegate void BuildingCardAction(BuildingCard buildingCard);
    public static event BuildingCardAction OnCardHovered;
    public static event BuildingCardAction OnCardUnhovered;
    public static event BuildingCardAction OnCardSelected;

    public event BuildingCardAction OnCardSelectedNotHovered;
    public event BuildingCardAction OnGetSaved;


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
        if (cardState != CardStates.STANDARD) return;

        if (OnCardHovered != null) OnCardHovered(this);
    }

    private void OnMouseExit()
    {
        if (cardState != CardStates.HOVERED) return;

        if (OnCardUnhovered != null) OnCardUnhovered(this);
    }

    private void OnMouseDown()
    {
        if (cardState == CardStates.HOVERED)
        {
            if (OnCardSelected != null) OnCardSelected(this);
        }
        else
        {
            if (OnCardSelectedNotHovered != null) OnCardSelectedNotHovered(this);
        }
    }


    // ABSTRACT METHODS to implement
    protected abstract void InitStatsFromTurretParts();
    public abstract void CreateCopyBuildingPrefab();
    public abstract int GetCardPlayCost();
    protected abstract void GetMaterialsRefs();
    protected abstract void InitVisuals();



    protected virtual void AwakeInit(CardBuildingType cardBuildingType)
    {
        this.cardBuildingType = cardBuildingType;
        GetMaterialsRefs();

        cardMaterial = cardMeshRenderer.material;
        SetCannotBePlayedAnimation();
        cardMaterial.SetFloat("_RandomTimeAdd", Random.Range(0f, Mathf.PI));
    }


    protected void Init()
    {
        initialPosition = CardTransform.position;

        InitStatsFromTurretParts();
        InitCostText();

        InitVisuals();
    }

    private void InitCostText()
    {
        playCostText.text = GetCardPlayCost().ToString();
    }


    // CARD MOVEMENT
    public void ResetCardPosition()
    {
        CardTransform.localPosition = Vector3.zero;
    }

    public void InitPositions(Vector3 selectedPosition, Vector3 hiddenDisplacement)
    {
        ResetCardPosition();

        local_standardPosition = CardTransform.localPosition;
        local_hoveredPosition = CardTransform.localPosition + HoveredTranslation;
        //local_selectedPosition = CardTransform.InverseTransformPoint(selectedPosition);
        local_standardRotation_euler = transform.localRotation.eulerAngles;
        this.hiddenDisplacement = hiddenDisplacement;

        standardPosition = CardTransform.position;
        hoveredPosition = CardTransform.position + HoveredTranslation;
        this.selectedPosition = selectedPosition;
    }

    public void StandardState()
    {
        cardState = CardStates.STANDARD;

        DisableMouseInteraction();
        CardTransform.DOBlendableLocalMoveBy(local_standardPosition - CardTransform.localPosition, unhoverTime);
        CardTransform.DOBlendableLocalRotateBy(local_standardRotation_euler - CardTransform.rotation.eulerAngles, unhoverTime)
            .OnComplete(() => EnableMouseInteraction());
    }

    public void HoveredState()
    {
        cardState = CardStates.HOVERED;

        CardTransform.DOBlendableLocalMoveBy(local_hoveredPosition - CardTransform.localPosition, hoverTime);
    }

    public void SelectedState()
    {
        cardState = CardStates.SELECTED;

        DisableMouseInteraction();
        CardTransform.DOBlendableMoveBy(selectedPosition - CardTransform.position, selectedTime);
        CardTransform.DOBlendableLocalRotateBy(Vector3.zero - CardTransform.rotation.eulerAngles, selectedTime)
            .OnComplete(() => EnableMouseInteraction());
    }

    // CARD MOVEMENT end

    public void normalCollider()
    {
        cardCollider.size = new Vector3(1f, 2f, 0.2f);
    }
    public void bigCollider()
    {
        cardCollider.size = new Vector3(1.3f, 2f, 0.2f);
    }


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

}
