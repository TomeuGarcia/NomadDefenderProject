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

    public static CardDescriptionDisplayer GetInstance()
    {
        return instance;
    }


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


    public void ShowCardDescription(ICardDescriptionProvider descriptionProvider)
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
    private bool PositionCardInfo(ICardDescriptionProvider descriptionProvider)
    {
        Vector3 cardPositionInScreen = displayCamera.WorldToScreenPoint(descriptionProvider.GetCenterPosition());

        float xDisplacement = 1.0f;
        Vector3 positionOffset = Vector3.zero;

        bool positionAtTheRight = cardPositionInScreen.x < displayCamera.pixelWidth * 0.85f;

        if (positionAtTheRight)
        {            
            positionOffset = Vector3.right * xDisplacement; // Display right
        }
        else
        {            
            positionOffset = Vector3.left * xDisplacement; // Display left
        }


        RectTransform parentUI = GetParentUIDescriptionBox();
        parentUI.position = displayCamera.WorldToScreenPoint(descriptionProvider.GetCenterPosition() + positionOffset);

        return positionAtTheRight;
    }

    private void SetupCardInfo(ICardDescriptionProvider descriptionProvider)
    {
        ICardDescriptionProvider.SetupData[] setupData = descriptionProvider.GetAbilityDescriptionSetupData();

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
