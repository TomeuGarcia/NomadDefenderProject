using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingCard : MonoBehaviour
{
    public enum CardStates { STANDARD, HOVERED, SELECTED }
    [HideInInspector] public CardStates cardState = CardStates.STANDARD;


    [Header("STATS")]
    [SerializeField] public Turret.TurretStats turretStats;

    [Header("BUILDING")]
    [SerializeField] public GameObject buildingPrefab;

    [Header("CANVAS COMPONENTS")]
    [SerializeField] private TextMeshProUGUI playCostText;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI rangeText;
    [SerializeField] private TextMeshProUGUI targetAmountText;
    [SerializeField] private TextMeshProUGUI cadenceText;


    private Vector3 standardPosition;
    private Vector3 hoveredPosition;
    private Vector3 selectedPosition;
    private Vector3 HoveredTranslation => transform.up * 0.2f;



    public delegate void BuildingCardAction(BuildingCard buildingCard);
    public static event BuildingCardAction OnCardHovered;
    public static event BuildingCardAction OnCardUnhovered;
    public static event BuildingCardAction OnCardSelected;




    private void OnValidate()
    {
        InitTexts();
    }

    private void Awake()
    {
        InitTexts();
        InitPositions();
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
        cadenceText.text = turretStats.cadence.ToString();
    }

    private void InitPositions()
    {
        standardPosition = transform.position;
        hoveredPosition = transform.position + HoveredTranslation;
        selectedPosition = transform.position + transform.up + (-transform.right);
    }

    public void StandardState()
    {
        cardState = CardStates.STANDARD;
        transform.position = standardPosition;
    }

    public void HoveredState()
    {
        cardState = CardStates.HOVERED;
        transform.position = hoveredPosition;
    }

    public void SelectedState()
    {
        cardState = CardStates.SELECTED;
        transform.position = selectedPosition;
    }

}
