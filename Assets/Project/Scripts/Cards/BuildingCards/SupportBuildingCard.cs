using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ICardDescriptionProvider;
using static SupportBuilding;
using static TurretBuildingCard;


public class SupportBuildingCard : BuildingCard, ICardDescriptionProvider
{
    public SupportCardParts supportCardParts { get; private set; }

    
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

    private SupportCardStatsController StatsController => supportCardParts.StatsController;
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

        copyBuildingPrefab.GetComponent<SupportBuilding>().Init(StatsController, supportCardParts.turretPartBase, currencyCounter, 
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

    public void ResetParts(SupportCardParts supportCardParts)
    {
        this.supportCardParts = ScriptableObject.CreateInstance("SupportCardParts") as SupportCardParts;
        this.supportCardParts.InitCopyingReferences(supportCardParts);
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
        PlayCost = supportCardParts.GetCardCost();
    }

    protected override void InitVisuals()
    {
        TurretPartBase turretPartBase = supportCardParts.turretPartBase;
        //Mesh Materials
        material.SetTexture("_Texture", turretPartBase.materialTexture);
        material.SetColor("_Color", turretPartBase.materialColor);

        // Canvas
        turretPartBase.SetStatTexts(_rangeStatValueText);
        abilityImage.transform.parent.gameObject.SetActive(true);

        abilityImage.sprite = turretPartBase.abilitySprite;
        abilityImage.color = turretPartBase.spriteColor;
    }


    public void SetNewPartBase(TurretPartBase newPartBase)
    {
        int costHolder = supportCardParts.turretPartBase.cost;
        supportCardParts.turretPartBase = newPartBase;
        supportCardParts.turretPartBase.cost = costHolder;
        Init();
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
        CardDescriptionDisplayer.GetInstance().ShowCardDescription(this);
    }

    public override void HideInfo()
    {
        base.HideInfo();
        CardDescriptionDisplayer.GetInstance()?.HideCardDescription();
    }


    // ICardDescriptionProvider OVERLOADS
    public ICardDescriptionProvider.SetupData[] GetAbilityDescriptionSetupData()
    {
        ICardDescriptionProvider.SetupData[] setupData = new ICardDescriptionProvider.SetupData[2];

        setupData[0] = null;

        TurretPartBase turretPartBase = supportCardParts.turretPartBase;
        setupData[1] = new ICardDescriptionProvider.SetupData(
            turretPartBase.abilityName,
            turretPartBase.abilityDescription,
            turretPartBase.abilitySprite,
            turretPartBase.spriteColor
        );

        return setupData;
    }
    public Vector3 GetCenterPosition()
    {
        return CardTransform.position;
    }

    public DescriptionCornerPositions GetCornerPositions()
    {
        return new DescriptionCornerPositions(leftDescriptionPosition.position, rightDescriptionPosition.position);
    }
}
