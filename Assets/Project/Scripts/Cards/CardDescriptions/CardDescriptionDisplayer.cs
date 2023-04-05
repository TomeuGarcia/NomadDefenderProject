using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDescriptionDisplayer : MonoBehaviour
{
    [SerializeField] private RectTransform parentUI;
    private Camera displayCamera;

    private void Awake()
    {
        displayCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = displayCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue))
            {
                BuildingCard buildingCard;

                if (hit.transform.TryGetComponent<BuildingCard>(out buildingCard))
                {                    
                    DisplayCardDescription(buildingCard);
                }

            }                
        }
    }


    private void DisplayCardDescription(BuildingCard card)
    {
        Vector3 cardPositionInScreen = displayCamera.WorldToScreenPoint(card.CardTransform.position);

        Vector3 positionOffset = Vector3.zero;

        if (cardPositionInScreen.x < displayCamera.pixelWidth * 0.75f)
        {
            // Display right
            positionOffset = Vector3.right * 1f;
        }
        else
        {
            // Display left
            positionOffset = Vector3.left * 1f;
        }


        parentUI.position = displayCamera.WorldToScreenPoint(card.CardTransform.position + positionOffset);
    }

}
