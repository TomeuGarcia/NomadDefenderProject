using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using static Building;
using UnityEngine.UI;

public class BuildingCard : MonoBehaviour
{
    public static float halfWidth = 0.7f;

    public enum CardStates { STANDARD, HOVERED, SELECTED }
    [HideInInspector] public CardStates cardState = CardStates.STANDARD;


    [Header("STATS")]
    [HideInInspector] public Turret.TurretStats turretStats;
    [SerializeField] private float hoverSpeed;
    [SerializeField] private float selectedSpeed;

    [Header("BUILDING PARTS")]
    [SerializeField] private TurretPartAttack turretPartAttack;
    [SerializeField] private TurretPartBody turretPartBody;
    [SerializeField] private TurretPartBase turretPartBase;
    [SerializeField] public GameObject turretPrefab;
    [HideInInspector] public GameObject copyTurretPrefab;
    public bool AlreadySpawnedCopyBuildingPrefab => copyTurretPrefab != null;

    [Header("CANVAS COMPONENTS")]
    [SerializeField] private TextMeshProUGUI playCostText;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI rangeText;
    [SerializeField] private TextMeshProUGUI targetAmountText;
    [SerializeField] private TextMeshProUGUI cadenceText;

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
    [SerializeField] private MeshRenderer attackMeshRenderer;
    [SerializeField] private MeshRenderer bodyMeshRenderer;
    [SerializeField] private MeshRenderer baseMeshRenderer;
    private Material cardAttackMaterial, cardBodyMaterial, cardBaseMaterial;
    [SerializeField] private MeshRenderer cardMeshRenderer;
    private Material cardMaterial;

    [SerializeField] private Image damageFillImage;
    [SerializeField] private Image cadenceFillImage;
    [SerializeField] private Image rangeFillImage;
    [SerializeField] private Image baseAbilityImage;



    public delegate void BuildingCardAction(BuildingCard buildingCard);
    public static event BuildingCardAction OnCardHovered;
    public static event BuildingCardAction OnCardUnhovered;
    public static event BuildingCardAction OnCardSelected;

    public event BuildingCardAction OnCardSelectedNotHovered;
    public event BuildingCardAction OnGetSaved;



    [System.Serializable]
    public struct CardComponents
    {
        public CardComponents(TurretPartAttack turretPartAttack, TurretPartBody turretPartBody, TurretPartBase turretPartBase)
        {
            this.turretPartAttack = turretPartAttack;
            this.turretPartBody = turretPartBody;
            this.turretPartBase = turretPartBase;
        }

        public CardComponents(CardComponents other)
        {
            this.turretPartAttack = other.turretPartAttack;
            this.turretPartBody = other.turretPartBody;
            this.turretPartBase = other.turretPartBase;
        }

        public TurretPartAttack turretPartAttack;
        public TurretPartBody turretPartBody;
        public TurretPartBase turretPartBase;
    }



    private void OnValidate()
    {
        //InitStatsFromTurretParts();
        InitTexts();
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
        cardAttackMaterial = attackMeshRenderer.material;
        cardBodyMaterial = bodyMeshRenderer.material;
        cardBaseMaterial = baseMeshRenderer.material;

        cardMaterial = cardMeshRenderer.material;
        SetCannotBePlayedAnimation();
        cardMaterial.SetFloat("_RandomTimeAdd", Random.Range(0f, Mathf.PI));
    }

    public void ResetParts(TurretPartAttack turretPartAttack,TurretPartBody turretPartBody, TurretPartBase turretPartBase)
    {
        this.turretPartAttack = turretPartAttack;
        this.turretPartBody = turretPartBody;
        this.turretPartBase = turretPartBase;

        Init();
    }

    private void Init()
    {
        initialPosition = transform.position;

        InitStatsFromTurretParts();
        InitTexts();

        InitVisuals();
    }

    private void InitStatsFromTurretParts()
    {
        turretStats.playCost = turretPartAttack.cost + turretPartBody.cost + turretPartBase.cost;
        turretStats.damage = turretPartBody.Damage;
        turretStats.range = turretPartBase.Range;
        turretStats.targetAmount = turretPartAttack.targetAmount;
        turretStats.cadence = turretPartBody.Cadence;
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

    private void InitTexts()
    {
        playCostText.text = turretStats.playCost.ToString();
        damageText.text = turretStats.damage.ToString();
        rangeText.text = turretStats.range.ToString();
        targetAmountText.text = turretStats.targetAmount.ToString();
        cadenceText.text = turretStats.cadence.ToString() + "s";

    }

    public void InitPositions(Vector3 selectedPosition)
    {
        standardPosition = transform.position;
        hoveredPosition = transform.position + HoveredTranslation;
        this.selectedPosition = selectedPosition;
    }

    public void CreateCopyBuildingPrefab()
    {
        copyTurretPrefab = Instantiate(turretPrefab, Vector3.zero, Quaternion.identity);
        copyTurretPrefab.GetComponent<Building>().Init(turretStats, turretPartAttack.prefab, turretPartBody.prefab, turretPartBase.prefab, turretPartBody.bodyType);
        copyTurretPrefab.SetActive(false);
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


    public void SetNewPartAttack(TurretPartAttack newTurretPartAttack)
    {
        int costHolder = turretPartAttack.cost;
        turretPartAttack = newTurretPartAttack;
        turretPartAttack.cost = costHolder;
        Init();
    }

    public void SetNewPartBody(TurretPartBody newTurretPartBody)
    {
        int costHolder = turretPartBody.cost;
        turretPartBody = newTurretPartBody;
        turretPartBody.cost = costHolder;
        Init();
    }

    public void SetNewPartBase(TurretPartBase newTurretPartBase)
    {
        int costHolder = turretPartBase.cost;
        turretPartBase = newTurretPartBase;
        turretPartBase.cost = costHolder;
        Init();
    }

    public TurretPartAttack GetTurretPartAttack()
    {
        return turretPartAttack;
    }
    public TurretPartBody GetTurretPartBody()
    {
        return turretPartBody;
    }
    public TurretPartBase GetTurretPartBase()
    {
        return turretPartBase;
    }


    private void InvokeGetSaved()
    {
        if (OnGetSaved != null) OnGetSaved(this);

    }


    private void InitVisuals()
    {
        // Mesh Materials
        cardAttackMaterial.SetTexture("_Texture", turretPartAttack.materialTexture);
        cardAttackMaterial.SetColor("_Color", turretPartAttack.materialColor);

        cardBodyMaterial.SetTexture("_MaskTexture", turretPartBody.materialTextureMap);
        cardBodyMaterial.SetColor("_PaintColor", turretPartAttack.materialColor); // Projectile color

        cardBaseMaterial.SetTexture("_Texture", turretPartBase.materialTexture);
        cardBaseMaterial.SetColor("_Color", turretPartBase.materialColor);


        // Canvas
        damageFillImage.fillAmount = turretPartBody.GetDamagePer1();
        cadenceFillImage.fillAmount = turretPartBody.GetCadencePer1();
        rangeFillImage.fillAmount = turretPartBase.GetRangePer1();

        bool hasAbility = turretPartBase.HasAbilitySprite();
        baseAbilityImage.transform.parent.gameObject.SetActive(hasAbility);
        if (hasAbility)
        {
            baseAbilityImage.sprite = turretPartBase.abilitySprite;
            baseAbilityImage.color = turretPartBase.spriteColor;
        }

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
