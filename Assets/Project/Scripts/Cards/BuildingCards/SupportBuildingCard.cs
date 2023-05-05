using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static TurretBuildingCard;


public class SupportBuildingCard : BuildingCard, ICardDescriptionProvider
{
    public SupportCardParts supportCardParts { get; private set; }

    private SupportBuilding.SupportBuildingStats supportBuildingStats;

    
    [Header("CARD INFO")]
    [SerializeField] private GameObject infoInterface;

    [Header("Base card info")]
    [SerializeField] private RectTransform infoShownPassiveIcon;
    [SerializeField] private RectTransform defaultPassiveIcon; // used as Hidden info
    private Vector3 infoShownPassiveIconPos;
    private Vector3 infoHiddenPassiveIconPos;
    [SerializeField] private TextMeshProUGUI passiveNameText;
    [SerializeField] private TextMeshProUGUI passiveDescriptionText;


    [Header("VISUALS")]
    //[SerializeField] private MeshRenderer baseMeshRenderer;
    [SerializeField] private Image baseImage;
    private Material material;
    [SerializeField] private Image rangeFillImage;
    [SerializeField] private Image abilityImage;


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

        copyBuildingPrefab.GetComponent<SupportBuilding>().Init(supportBuildingStats, supportCardParts.turretPartBase, currencyCounter, 
            abilityImage.sprite, abilityImage.color);
        copyBuildingPrefab.SetActive(false);
    }

    public override int GetCardPlayCost()
    {
        return supportBuildingStats.playCost;
    }
    public void ResetParts(SupportCardParts supportCardParts)
    {
        this.supportCardParts = ScriptableObject.CreateInstance("SupportCardParts") as SupportCardParts;
        this.supportCardParts.Init(supportCardParts);
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
        supportBuildingStats.playCost = supportCardParts.GetCardCost();
        supportBuildingStats.range = supportCardParts.turretPartBase.Range;
    }

    protected override void InitVisuals()
    {
        TurretPartBase turretPartBase = supportCardParts.turretPartBase;
        //Mesh Materials
        material.SetTexture("_Texture", turretPartBase.materialTexture);
        material.SetColor("_Color", turretPartBase.materialColor);

        // Canvas
        rangeFillImage.fillAmount = turretPartBase.GetRangePer1();
        abilityImage.transform.parent.gameObject.SetActive(true);

        abilityImage.sprite = turretPartBase.abilitySprite;
        abilityImage.color = turretPartBase.spriteColor;

        // Ability Info
        InitInfoVisuals();
    }


    public void SetNewPartBase(TurretPartBase newPartBase)
    {
        int costHolder = supportCardParts.turretPartBase.cost;
        supportCardParts.turretPartBase = newPartBase;
        supportCardParts.turretPartBase.cost = costHolder;
        Init();
    }

    protected override void InitInfoVisuals()
    {
        passiveNameText.text = '/' + supportCardParts.turretPartBase.abilityName;
        passiveDescriptionText.text = supportCardParts.turretPartBase.abilityDescription;
    }
    protected override void SetupCardInfo()
    {
        // general
        infoInterface.SetActive(true);
        isShowInfoAnimationPlaying = false;

        // base passive
        infoShownPassiveIconPos = infoShownPassiveIcon.localPosition;
        infoHiddenPassiveIconPos = defaultPassiveIcon.localPosition;
        passiveNameText.alpha = 0;
        passiveDescriptionText.alpha = 0;
    }

    public override void ShowInfo()
    {
        base.ShowInfo();
        CardDescriptionDisplayer.GetInstance().ShowCardDescription(this);
        return;
        

        showInfoCoroutine = StartCoroutine(ShowInfoAnimation());       
    }

    public override void HideInfo()
    {
        base.HideInfo();
        CardDescriptionDisplayer.GetInstance().HideCardDescription();
        return;

        if (isHideInfoAnimationPlaying) return;


        if (isShowInfoAnimationPlaying)
        {
            StopCoroutine(showInfoCoroutine);
        }

        StartCoroutine(HideInfoAnimation());
    }

    private IEnumerator ShowInfoAnimation()
    {
        canInfoInteract = false;
        isShowInfoAnimationPlaying = true;

        // hide generics
        float t = 0.05f;
        for (int i = 0; i < cgsInfoHide.Length; ++i)
        {
            cgsInfoHide[i].DOFade(0f, t);
            GameAudioManager.GetInstance().PlayCardInfoShown();
            yield return new WaitForSeconds(t);
        }


        float t2 = 0.1f;

        // show passive icon
        defaultPassiveIcon.DOLocalMove(infoShownPassiveIconPos, t2);
        GameAudioManager.GetInstance().PlayCardInfoMoveShown();
        yield return new WaitForSeconds(t2);

        // show passive text
        passiveNameText.DOFade(1f, t2);
        GameAudioManager.GetInstance().PlayCardInfoShown();
        yield return new WaitForSeconds(t2);
        passiveDescriptionText.DOFade(1f, t2);
        GameAudioManager.GetInstance().PlayCardInfoShown();
        yield return new WaitForSeconds(t2);


        canInfoInteract = true;
        isShowInfoAnimationPlaying = false;
    }

    private IEnumerator HideInfoAnimation()
    {
        canInfoInteract = false;


        float t2 = 0.1f;

        // hide passive text
        passiveDescriptionText.DOFade(0f, t2);
        GameAudioManager.GetInstance().PlayCardInfoHidden();
        yield return new WaitForSeconds(t2);
        passiveNameText.DOFade(0f, t2);
        GameAudioManager.GetInstance().PlayCardInfoHidden();
        yield return new WaitForSeconds(t2);

        // hide passive icon
        defaultPassiveIcon.DOLocalMove(infoHiddenPassiveIconPos, t2);
        GameAudioManager.GetInstance().PlayCardInfoMoveHidden();
        yield return new WaitForSeconds(t2);


        // show generics
        float t = 0.05f;

        for (int i = cgsInfoHide.Length - 1; i >= 0; --i)
        {
            cgsInfoHide[i].DOFade(1f, t);
            GameAudioManager.GetInstance().PlayCardInfoHidden();
            yield return new WaitForSeconds(t);
        }


        canInfoInteract = true;
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

}
