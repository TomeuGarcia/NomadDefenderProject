using UnityEngine;
using TMPro;

public class BuildingCard : MonoBehaviour
{
    public static float halfWidth = 0.7f;

    public enum CardStates { STANDARD, HOVERED, SELECTED }
    [HideInInspector] public CardStates cardState = CardStates.STANDARD;


    [Header("STATS")]
    [SerializeField] public Turret.TurretStats turretStats;
    [SerializeField] private float hoverSpeed;
    [SerializeField] private float selectedSpeed;

    [Header("BUILDING")]
    [SerializeField] private GameObject buildingPrefab;
    [HideInInspector] public GameObject copyBuildingPrefab;

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




    private void OnValidate()
    {
        InitTexts();

        if (turretStats.range % 2 == 0)
            --turretStats.range;
    }

    private void Awake()
    {
        InitTexts();
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
        if (cardState != CardStates.HOVERED) return;

        if (OnCardSelected != null) OnCardSelected(this);
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

    private void CreateCopyBuildingPrefab()
    {
        copyBuildingPrefab = Instantiate(buildingPrefab, Vector3.zero, Quaternion.identity);
        copyBuildingPrefab.GetComponent<Building>().Init(turretStats);
        copyBuildingPrefab.SetActive(false);
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

    public void DoOnCardIsDrawn()
    {
        CreateCopyBuildingPrefab();
    }

}
