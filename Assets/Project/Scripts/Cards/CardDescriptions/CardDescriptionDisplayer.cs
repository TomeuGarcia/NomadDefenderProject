using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDescriptionDisplayer : MonoBehaviour
{
    private static CardDescriptionDisplayer instance;

    [SerializeField] private RectTransform parentUI;


    [SerializeField] private CardAbilityDescriptionBox attackDescriptionBox;
    [SerializeField] private CardAbilityDescriptionBox baseDescriptionBox;
    private Camera displayCamera;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
            Init();
        }
        else
        {
            Destroy(this);
        }
    }

    public static CardDescriptionDisplayer GetInstance()
    {
        return instance;
    }


    private void Init()
    {
    }

    public void SetCamera(Camera camera)
    {
        displayCamera = camera;
    }


    public void ShowCardDescription(ICardDescriptionProvider descriptionProvider)
    {
        PositionCardInfo(descriptionProvider);
        SetupCardInfo(descriptionProvider);

        DoShowCardDescription();
    }

    private void DoShowCardDescription()
    {
        if (attackDescriptionBox.gameObject.activeInHierarchy) attackDescriptionBox.Show();
        if (baseDescriptionBox.gameObject.activeInHierarchy) baseDescriptionBox.Show();
    }

    public void HideCardDescription()
    {
        if (attackDescriptionBox.gameObject.activeInHierarchy) attackDescriptionBox.Hide();
        if (baseDescriptionBox.gameObject.activeInHierarchy) baseDescriptionBox.Hide();
    }


    private void PositionCardInfo(ICardDescriptionProvider descriptionProvider)
    {
        Vector3 cardPositionInScreen = displayCamera.WorldToScreenPoint(descriptionProvider.GetCenterPosition());

        float xDisplacement = 1.0f;
        Vector3 positionOffset;

        if (cardPositionInScreen.x < displayCamera.pixelWidth * 0.25f)
        {            
            positionOffset = Vector3.right * xDisplacement; // Display right
        }
        else
        {            
            positionOffset = Vector3.left * xDisplacement; // Display left
        }

        parentUI.position = displayCamera.WorldToScreenPoint(descriptionProvider.GetCenterPosition() + positionOffset);
    }

    private void SetupCardInfo(ICardDescriptionProvider descriptionProvider)
    {
        ICardDescriptionProvider.SetupData[] setupData = descriptionProvider.GetAbilityDescriptionSetupData();

        if (setupData[0] != null) 
        {
            attackDescriptionBox.gameObject.SetActive(true);
            attackDescriptionBox.SetupTextsAndIcon(setupData[0]);
        }
        else
        {
            attackDescriptionBox.gameObject.SetActive(false);
        }

        if (setupData[1] != null)
        {
            baseDescriptionBox.gameObject.SetActive(true);
            baseDescriptionBox.SetupTextsAndIcon(setupData[1]);
        }      
        else
        {
            baseDescriptionBox.gameObject.SetActive(false);
        }
        
    }



}
