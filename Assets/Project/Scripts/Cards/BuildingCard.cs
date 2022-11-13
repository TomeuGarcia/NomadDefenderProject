using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using static Building;

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


    private Vector3 standardPosition;
    private Vector3 hoveredPosition;
    private Vector3 selectedPosition;
    private Vector3 HoveredTranslation => transform.up * 0.2f + transform.forward * -0.04f;
    public Vector3 SelectedPosition => transform.position + (transform.up * 1.3f) + (-transform.right * 1.3f);


    public delegate void BuildingCardAction(BuildingCard buildingCard);
    public static event BuildingCardAction OnCardHovered;
    public static event BuildingCardAction OnCardUnhovered;
    public static event BuildingCardAction OnCardSelected;

    public event BuildingCardAction OnCardSelectedNotHovered;




    private void OnValidate()
    {
        //InitStatsFromTurretParts();
        InitTexts();
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
        InitStatsFromTurretParts();
        InitTexts();
    }

    private void InitStatsFromTurretParts()
    {
        turretStats.playCost = turretPartAttack.cost + turretPartBody.cost + turretPartBase.cost;
        turretStats.damage = turretPartBody.damage;
        turretStats.range = turretPartBase.attackRange;
        turretStats.targetAmount = turretPartAttack.targetAmount;
        turretStats.cadence = turretPartBody.attackSpeed;
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
        copyTurretPrefab.GetComponent<Building>().Init(turretStats, turretPartAttack.prefab, turretPartBody.prefab, turretPartBase.prefab);
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
        turretPartAttack = newTurretPartAttack;
        Init();
    }

    public void SetNewPartBody(TurretPartBody newTurretPartBody)
    {
        turretPartBody = newTurretPartBody;
        Init();
    }

    public void SetNewPartBase(TurretPartBase newTurretPartBase)
    {
        turretPartBase = newTurretPartBase;
        Init();
    }


}
