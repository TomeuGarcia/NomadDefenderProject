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
    

    [Header("VISUALS")]
    [SerializeField] private Transform _buildingParentTransform;
    private Transform _buildingMeshPreview;
    private Material material;
    [SerializeField] private TextMeshProUGUI _rangeStatValueText;
    [SerializeField] private Image abilityImage;
    

    private SupportCardStatsController StatsController => CardData.StatsController;
    private int PlayCost { get; set; }



    public void Awake()
    {
        AwakeInit(CardBuildingType.SUPPORT);
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

    protected override void InitStatsFromTurretParts()
    {
        PlayCost = CardData.PlayCost;
    }

    protected override void InitVisuals()
    {
        SupportPartBase turretPartBase = CardParts.Base;

        //Mesh Preview
        InstantiateTurretPreviewMesh();

        // Canvas
        _rangeStatValueText.text = StatsController.RadiusRangeStatState.BaseValueText;        
        abilityImage.transform.parent.gameObject.SetActive(true);

        abilityImage.sprite = turretPartBase.abilitySprite;
        abilityImage.color = turretPartBase.spriteColor;
    }

    private void InstantiateTurretPreviewMesh()
    {
        if (_buildingMeshPreview != null)
        {
            Destroy(_buildingMeshPreview.transform.parent.gameObject);
        }

        _buildingMeshPreview = Instantiate(CardParts.Base.PreviewMeshPrefab, _buildingParentTransform).transform;
        _buildingMeshPreview.transform.localRotation = Quaternion.identity;
        _buildingMeshPreview.transform.localPosition = Vector3.zero;
        _buildingMeshPreview.transform.localScale = Vector3.one;
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
