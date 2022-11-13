using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using static Building;

public class CardPart : MonoBehaviour
{
    public static float halfWidth = 0.7f;

    public enum CardPartStates { STANDARD, HOVERED, SELECTED }
    [HideInInspector] public CardPartStates cardState = CardPartStates.STANDARD;

    [Header("OTHER COMPONENTS")]
    [SerializeField] private float hoverSpeed;
    [SerializeField] private float selectedSpeed; 
    [SerializeField] private Lerp lerp;


    private Vector3 standardPosition;
    private Vector3 hoveredPosition;
    private Vector3 selectedPosition;
    private Vector3 HoveredTranslation => transform.up * 0.2f + transform.forward * -0.04f;
    public Vector3 SelectedPosition => transform.position + (transform.up * 1.3f) + (-transform.right * 1.3f);


    public delegate void BuildingCardPartAction(CardPart cardPart);
    public static event BuildingCardPartAction OnCardHovered;
    public static event BuildingCardPartAction OnCardUnhovered;
    public static event BuildingCardPartAction OnCardSelected;

    public event BuildingCardPartAction OnCardSelectedNotHovered;



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

    public virtual void Init()
    {

    }

    public void InitPositions(Vector3 selectedPosition)
    {
        standardPosition = transform.position;
        hoveredPosition = transform.position + HoveredTranslation;
        this.selectedPosition = selectedPosition;
    }

    public void StandardState()
    {
        cardState = CardPartStates.STANDARD;
        transform.position = standardPosition;
    }

    public void HoveredState()
    {
        cardState = CardPartStates.HOVERED;
        lerp.SpeedLerpPosition(hoveredPosition, hoverSpeed);
    }

    public void SelectedState()
    {
        cardState = CardPartStates.SELECTED;
        lerp.SpeedLerpPosition(selectedPosition, selectedSpeed);
    }

}
