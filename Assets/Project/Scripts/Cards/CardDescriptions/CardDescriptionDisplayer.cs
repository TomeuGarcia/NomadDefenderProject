using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDescriptionDisplayer : MonoBehaviour
{
    private static CardDescriptionDisplayer instance;

    private Camera displayCamera;

    [SerializeField] private CardAbilityDescriptionBoxPack[] descriptionBoxPacks;
    private int currentDescriptionBoxPackI = 0;

    [System.Serializable]
    private struct CardAbilityDescriptionBoxPack
    {
        [SerializeField] public RectTransform parentUI;
        [SerializeField] public CardAbilityDescriptionBox attackDescriptionBox;
        [SerializeField] public CardAbilityDescriptionBox baseDescriptionBox;
    }


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

    private void OnEnable()
    {
        MapSceneNotifier.OnMapSceneFinished += HideCardDescription;
    }
    private void OnDisable()
    {
        MapSceneNotifier.OnMapSceneFinished -= HideCardDescription;
    }

    /*
    public static CardDescriptionDisplayer GetInstance()
    {
        return instance;
    }
    */


    private void Init()
    {
    }

    private void IncrementCurrentDescriptionBoxPackI()
    {
        currentDescriptionBoxPackI = ++currentDescriptionBoxPackI % descriptionBoxPacks.Length;
    }
    private CardAbilityDescriptionBoxPack GetDescriptionBoxPack()
    {
        return descriptionBoxPacks[currentDescriptionBoxPackI];
    }
    private RectTransform GetParentUIDescriptionBox()
    {
        return GetDescriptionBoxPack().parentUI;
    }
    private CardAbilityDescriptionBox GetAttackDescriptionBox()
    {
        return GetDescriptionBoxPack().attackDescriptionBox;
    }
    private CardAbilityDescriptionBox GetBaseDescriptionBox()
    {
        return GetDescriptionBoxPack().baseDescriptionBox;
    }


    public void SetCamera(Camera camera)
    {
        displayCamera = camera;
    }


    public void ShowCardDescription(ICardTooltipSource descriptionProvider)
    {
        bool positionedAtTheRight = PositionCardInfo(descriptionProvider);
        SetupCardInfo(descriptionProvider);

        DoShowCardDescription(positionedAtTheRight);
    }

    private void DoShowCardDescription(bool positionedAtTheRight)
    {
        CardAbilityDescriptionBox attackDescriptionBox = GetAttackDescriptionBox();
        CardAbilityDescriptionBox baseDescriptionBox = GetBaseDescriptionBox();

        if (attackDescriptionBox.gameObject.activeInHierarchy) attackDescriptionBox.Show(positionedAtTheRight);
        if (baseDescriptionBox.gameObject.activeInHierarchy) baseDescriptionBox.Show(positionedAtTheRight);
    }

    public void HideCardDescription()
    {
        CardAbilityDescriptionBox attackDescriptionBox = GetAttackDescriptionBox();
        CardAbilityDescriptionBox baseDescriptionBox = GetBaseDescriptionBox();

        if (attackDescriptionBox.gameObject.activeInHierarchy) attackDescriptionBox.Hide();
        if (baseDescriptionBox.gameObject.activeInHierarchy) baseDescriptionBox.Hide();

        IncrementCurrentDescriptionBoxPackI();
    }


    // true if positioned at right, false if left
    private bool PositionCardInfo(ICardTooltipSource descriptionProvider)
    {
        ICardTooltipSource.DescriptionCornerPositions cornerPositions = descriptionProvider.GetCornerPositions();
        Vector3 cardPositionInScreen = displayCamera.WorldToScreenPoint(descriptionProvider.GetCenterPosition());

        Vector3 displayPosition = descriptionProvider.GetCenterPosition();

        RectTransform parentUI = GetParentUIDescriptionBox();

        float xDisplacement = 1.0f;
        Vector3 screenPositionOffset = Vector3.zero;

        bool positionAtTheRight = cardPositionInScreen.x < displayCamera.pixelWidth * 0.80f;
        if (positionAtTheRight)
        {
            displayPosition = cornerPositions.rightPosition;
            screenPositionOffset.x += parentUI.rect.width / 2; // Display right
        }
        else
        {
            screenPositionOffset.x -= parentUI.rect.width / 2; // Display left
            displayPosition = cornerPositions.leftPosition;
        }

        screenPositionOffset.y -= parentUI.rect.height / 2;

        bool positionUpper = cardPositionInScreen.y < displayCamera.pixelHeight * 0.30f;
        if (positionUpper)
        {
            screenPositionOffset.y += parentUI.rect.height / 8;
        }
        bool positionDown = cardPositionInScreen.y > displayCamera.pixelHeight * 0.70f;
        if (positionDown)
        {
            screenPositionOffset.y -= parentUI.rect.height / 8;
        }
        

        parentUI.position = displayCamera.WorldToScreenPoint(displayPosition) + screenPositionOffset;

        return positionAtTheRight;
    }

    private void SetupCardInfo(ICardTooltipSource descriptionProvider)
    {
        ICardTooltipSource.SetupData[] setupData = descriptionProvider.GetAbilityDescriptionSetupData();

        CardAbilityDescriptionBox attackDescriptionBox = GetAttackDescriptionBox();
        CardAbilityDescriptionBox baseDescriptionBox = GetBaseDescriptionBox();

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
