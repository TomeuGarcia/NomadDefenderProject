using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ICardTooltipSource;


public class SupportBuildingCard : BuildingCard, ICardTooltipSource
{
    public SupportCardData CardData { get; private set; }
    public SupportCardPartsGroup CardParts => CardData.SharedPartsGroup;

    
    [Header("CARD INFO")]
    [Header("Base card info")]
    [SerializeField] private RectTransform defaultPassiveIcon; // used as Hidden info
    private Vector3 infoShownPassiveIconPos;
    private Vector3 infoHiddenPassiveIconPos;


    [Header("VISUALS")]
    //[SerializeField] private MeshRenderer baseMeshRenderer;
    [SerializeField] private Image baseImage;
    private Material material;
    [SerializeField] private TextMeshProUGUI _rangeStatValueText;
    [SerializeField] private Image abilityImage;



    [Header("DESCRIPTION")]
    [SerializeField] private Transform leftDescriptionPosition;
    [SerializeField] private Transform rightDescriptionPosition;

    private SupportCardStatsController StatsController => CardData.StatsController;
    private int PlayCost { get; set; }



    public void Awake()
    {
        AwakeInit(CardBuildingType.SUPPORT);
    }
    protected override void AwakeInit(CardBuildingType cardBuildingType)
    {
        base.AwakeInit(cardBuildingType);
        SetupCardInfo();
    }

    public override void CreateCopyBuildingPrefab(Transform spawnTransform, CurrencyCounter currencyCounter)
    {
        copyBuildingPrefab = Instantiate(buildingPrefab, Vector3.zero, Quaternion.identity);
        copyBuildingPrefab.transform.SetParent(spawnTransform);

        copyBuildingPrefab.GetComponent<SupportBuilding>().Init(StatsController, CardData, currencyCounter, 
            abilityImage.sprite, abilityImage.color);
        copyBuildingPrefab.SetActive(false);
    }

    public override int GetCardPlayCost()
    {
        return PlayCost;
    }

    public override void UpdatePlayCost(int newPlayCost)
    {
        PlayCost = newPlayCost;
        InitCostText();
    }

    public void InitWithData(SupportCardData cardData)
    {
        CardData = cardData;
        Init();
    }
    
    protected override void GetMaterialsRefs()
    {
        //material = baseMeshRenderer.material;
        material = new Material(baseImage.material);
        baseImage.material = material;
    }

    protected override void InitStatsFromTurretParts()
    {
        PlayCost = CardData.PlayCost;
    }

    protected override void InitVisuals()
    {
        SupportPartBase turretPartBase = CardParts.Base;
        //Mesh Materials
        material.SetTexture("_Texture", turretPartBase.BasePartPrimitive.MaterialTexture);
        material.SetColor("_Color", turretPartBase.BasePartPrimitive.MaterialColor);

        // Canvas
        _rangeStatValueText.text = StatsController.RadiusRangeStatState.BaseValueText;        
        abilityImage.transform.parent.gameObject.SetActive(true);

        abilityImage.sprite = turretPartBase.abilitySprite;
        abilityImage.color = turretPartBase.spriteColor;
    }


    protected override void SetupCardInfo()
    {
        // general
        isShowInfoAnimationPlaying = false;

        // base passive
        infoHiddenPassiveIconPos = defaultPassiveIcon.localPosition;
    }

    public override void ShowInfo()
    {
        base.ShowInfo();
        CardTooltipDisplayManager.GetInstance().StartDisplayingTooltip(this);
    }

    public override void HideInfo()
    {
        base.HideInfo();
        CardTooltipDisplayManager.GetInstance()?.StopDisplayingTooltip();
    }


    // ICardTooltipSource OVERLOADS
    public CardTooltipDisplayData MakeTooltipDisplayData()
    {
        return CardTooltipDisplayData.MakeForSupportCard(_descriptionTooltipPositioning, CardParts.Base, 
            CardData.AbilityDescriptions);
    }
}
