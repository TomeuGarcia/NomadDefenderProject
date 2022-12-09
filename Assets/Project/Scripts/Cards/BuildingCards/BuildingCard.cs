using UnityEngine;
using TMPro;
using static BuildingCard;

public class BuildingCard : MonoBehaviour
{
    public static float halfWidth = 0.7f;


    public enum CardBuildingType { NONE, TURRET }
    public CardBuildingType cardBuildingType { get; protected set; }



    public enum CardStates { STANDARD, HOVERED, SELECTED }
    [HideInInspector] public CardStates cardState = CardStates.STANDARD;

    [Header("LERP")]
    [SerializeField] private float hoverSpeed;
    [SerializeField] private float selectedSpeed;


    // BUILDING PARTS
    protected Turret.TurretStats turretStats;



    [Header("BUILDING PREFAB")]
    [SerializeField] public GameObject buildingPrefab;
    [HideInInspector] public GameObject copyBuildingPrefab;
    public bool AlreadySpawnedCopyBuildingPrefab => copyBuildingPrefab != null;

    public int PlayCost => turretStats.playCost;


    [Header("CANVAS COMPONENTS")]
    [SerializeField] private TextMeshProUGUI playCostText;

    [Header("OTHER COMPONENTS")]
    [SerializeField] private Lerp lerp;
    [SerializeField] private Collider cardCollider;


    private Vector3 initialPosition;
    private Vector3 standardPosition;
    private Vector3 hoveredPosition;
    private Vector3 selectedPosition;
    private Vector3 HoveredTranslation => transform.up * 0.2f + transform.forward * -0.14f;
    public Vector3 SelectedPosition => transform.position + (transform.up * 1.3f) + (-transform.right * 1.3f);


    [Header("VISUALS")]
    [SerializeField] private MeshRenderer cardMeshRenderer;
    private Material cardMaterial;


    public delegate void BuildingCardAction(BuildingCard buildingCard);
    public static event BuildingCardAction OnCardHovered;
    public static event BuildingCardAction OnCardUnhovered;
    public static event BuildingCardAction OnCardSelected;

    public event BuildingCardAction OnCardSelectedNotHovered;
    public event BuildingCardAction OnGetSaved;



    [System.Serializable]
    public class BuildingCardParts
    {
        public BuildingCardParts()
        {
        }
        public BuildingCardParts(BuildingCardParts other)
        {
            //this.turretPartAttack = other.turretPartAttack;
            //this.turretPartBody = other.turretPartBody;
            //this.turretPartBase = other.turretPartBase;
        }

        //public TurretPartAttack turretPartAttack;
        //public TurretPartBody turretPartBody;
        //public TurretPartBase turretPartBase;

        public virtual int GetCostCombinedParts()
        {
            return 0;
        }
    }

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
        initialPosition = transform.position;

        InitStatsFromTurretParts();
        InitCostText();

        InitVisuals();
    }

    protected virtual void InitStatsFromTurretParts()
    {
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

    private void InitCostText()
    {
        playCostText.text = turretStats.playCost.ToString();
    }

    public void InitPositions(Vector3 selectedPosition)
    {
        standardPosition = transform.position;
        hoveredPosition = transform.position + HoveredTranslation;
        this.selectedPosition = selectedPosition;
    }

    public virtual void CreateCopyBuildingPrefab()
    {
    }

    public void StandardState()
    {
        cardState = CardStates.STANDARD;
        transform.position = standardPosition;
    }

    public void HoveredState()
    {
        cardState = CardStates.HOVERED;
        lerp.SpeedLerpPosition(hoveredPosition, hoverSpeed);
    }

    public void SelectedState()
    {
        cardState = CardStates.SELECTED;
        lerp.SpeedLerpPosition(selectedPosition, selectedSpeed);
    }



    private void InvokeGetSaved()
    {
        if (OnGetSaved != null) OnGetSaved(this);
    }

    protected virtual void GetMaterialsRefs() { }
    protected virtual void InitVisuals() {}

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
