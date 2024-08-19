using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ICardDescriptionProvider;


public class TurretBuildingCard : BuildingCard, ICardDescriptionProvider
{
    public TurretCardData CardData { get; private set; }
    public TurretCardPartsGroup CardParts => CardData.SharedPartsGroup;
    
    private TurretBuilding turretBuilding;



    [Header("CARD INFO")]
    [Header("Attack card info")]
    [SerializeField] private RectTransform defaultAttackIcon; // used as Hidden info
    private Vector3 infoShownAttackIconPos;
    private Vector3 infoHiddenAttackIconPos;


    [Header("Base card info")]
    [SerializeField] private RectTransform defaultBaseIcon; // used as Hidden info
    private Vector3 infoShownBaseIconPos;
    private Vector3 infoHiddenBaseIconPos;



    [Header("VISUALS")]
    //[SerializeField] private MeshRenderer attackMeshRenderer;
    //[SerializeField] private MeshRenderer bodyMeshRenderer;
    //[SerializeField] private MeshRenderer baseMeshRenderer;
    [SerializeField] private Image attackImage;
    [SerializeField] private Image bodyImage;
    [SerializeField] private Image baseImage;
    private Material cardAttackMaterial, cardBodyMaterial, cardBaseMaterial;

    [SerializeField] private TextMeshProUGUI _damageStatValueText;
    [SerializeField] private TextMeshProUGUI _fireRateStatValueText;
    [SerializeField] private TextMeshProUGUI _rangeStatValueText;

    [SerializeField] private Image basePassiveImage;

    [SerializeField] protected TextMeshProUGUI cardLevelText;
    [SerializeField] private TextDecoder cardLevelTextDecoder;
    private bool cardLevelAlredyDisplayedMax = false;

    bool hasBasePassiveAbility = false;


    [HideInInspector] public bool ReplacedWithSamePart { get; private set; }
    private bool playingPlayCostAnimation = false;


    [Header("DESCRIPTION")]
    [SerializeField] private Transform leftDescriptionPosition;
    [SerializeField] private Transform rightDescriptionPosition;


    private TurretCardStatsController StatsController => CardData.StatsController;
    public ITurretStatsBonusController StatsBonusController => StatsController;
    private int PlayCost { get; set; }


    private void Awake()
    {
        AwakeInit(CardBuildingType.TURRET);
    }

    protected override void AwakeInit(CardBuildingType cardBuildingType)
    {
        base.AwakeInit(cardBuildingType);
        SetupCardInfo();        
    }

    protected override void GetMaterialsRefs() 
    {
        //cardAttackMaterial = attackMeshRenderer.material;
        //cardBodyMaterial = bodyMeshRenderer.material;
        //cardBaseMaterial = baseMeshRenderer.material;
        cardAttackMaterial = new Material(attackImage.material);
        attackImage.material = cardAttackMaterial;
        cardBodyMaterial = new Material(bodyImage.material);
        bodyImage.material = cardBodyMaterial;
        cardBaseMaterial = new Material(baseImage.material);
        baseImage.material = cardBaseMaterial;
    }

    protected override void InitVisuals()
    {
        TurretPartAttack turretPartAttack = CardParts.Projectile;
        TurretPartBody turretPartBody = CardParts.Body;
        TurretPartBase turretPartBase = CardParts.Base;

        // Mesh Materials
        cardBodyMaterial.SetTexture("_MaskTexture", turretPartBody.materialTextureMap);

        cardBaseMaterial.SetTexture("_Texture", turretPartBase.materialTexture);
        cardBaseMaterial.SetColor("_Color", turretPartBase.materialColor);


        // Canvas
        _damageStatValueText.text = StatsController.DamageStatState.BaseValueText;
        _fireRateStatValueText.text = StatsController.ShotsPerSecondInvertedStatState.BaseValueText;
        _rangeStatValueText.text = StatsController.RadiusRangeStatState.BaseValueText;


        SetAttackIcon(turretPartAttack);


        hasBasePassiveAbility = CardParts.Passive.passive.GetType() != typeof(BaseNullPassive);

        if (hasBasePassiveAbility)
        {
            basePassiveImage.transform.parent.gameObject.SetActive(true);

            basePassiveImage.sprite = CardParts.Passive.visualInformation.sprite;
            basePassiveImage.color = CardParts.Passive.visualInformation.color;
        }
        else {
            basePassiveImage.transform.parent.gameObject.SetActive(false);
        }

        // Level
        UpdateCardLevelText();
    }

    private void SetAttackIcon(TurretPartAttack turretPartAttack)
    {
        attackImage.sprite = turretPartAttack.abilitySprite;
        attackImage.color = turretPartAttack.materialColor;

        cardBodyMaterial.SetColor("_PaintColor", turretPartAttack.materialColor);
    }

    protected override void InitStatsFromTurretParts()
    {
        StatsController.ResetUpgradeLevel();
        PlayCost = CardData.PlayCost;
    }

    public override void CreateCopyBuildingPrefab(Transform spawnTransform, CurrencyCounter currencyCounter)
    {
        copyBuildingPrefab = Instantiate(buildingPrefab, Vector3.zero, Quaternion.identity);
        copyBuildingPrefab.transform.SetParent(spawnTransform);

        turretBuilding = copyBuildingPrefab.GetComponent<TurretBuilding>();
        turretBuilding.Init(StatsController, CardData, currencyCounter);
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

    public void InitWithData(TurretCardData cardData)
    {
        CardData = cardData;
        Init();
    }
    
    public void SetNewPartAttack(TurretPartAttack newTurretPartAttack)
    {
        ReplacedWithSamePart = HasSameAttackPart(newTurretPartAttack); // Check replaced with same part
        CardParts.SetProjectile(newTurretPartAttack);
        
        Init();
    }
    public bool HasSameAttackPart(TurretPartAttack newTurretPartAttack)
    {
        return CardParts.Projectile == newTurretPartAttack;
    }

    public void SetNewPartBody(TurretPartBody newTurretPartBody)
    {
        ReplacedWithSamePart = HasSameBodyPart(newTurretPartBody); // Check replaced with same part
        CardParts.SetBody(newTurretPartBody);

        Init();
    }
    public bool HasSameBodyPart(TurretPartBody newTurretPartBody)
    {
        return CardParts.Body == newTurretPartBody;
    }

    public void SetNewPartBasePassive(TurretPassiveBase newTurretPassiveBase)
    {
        ReplacedWithSamePart = HasSameBasePassivePart(newTurretPassiveBase); // Check replaced with same part
        CardParts.SetPassiveAbility(newTurretPassiveBase);
        
        Init();
    }

    public void AddPermanentBonusStats(CardPartBonusStats cardPartBonusStats)
    {
        cardPartBonusStats.ApplyStatsModification(StatsBonusController);
        Init();
    }


    public bool HasSameBasePassivePart(TurretPassiveBase newTurretPassiveBase)
    {
        return CardParts.Passive == newTurretPassiveBase;
    }

    protected override void SetupCardInfo()
    {
        // general
        isShowInfoAnimationPlaying = false;

        // attack
        infoHiddenAttackIconPos = defaultAttackIcon.localPosition;

        // base
        infoHiddenBaseIconPos = defaultBaseIcon.localPosition;
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
        return;

        if (isHideInfoAnimationPlaying) return;
    }




    // Card Level
    public override int GetCardLevel()
    {
        return CardData.CardUpgradeLevel;
    }

    public void IncrementCardLevel(int levelIncrement, bool updateText = true)
    {
        cardLevelAlredyDisplayedMax = IsCardLevelMaxed();
        CardData.IncrementUpgradeLevel(levelIncrement);

        if (updateText)
        {
            UpdateCardLevelText();
        }
    }

    public bool IsCardLevelMaxed()
    {
        return CardData.IsCardUpgradeLevelMaxed();
    }
    private string GetCardLevelString()
    {
        int level = CardData.CardUpgradeLevel;
        return IsCardLevelMaxed() ? "MAX" : "lvl " + level.ToString();
    }

    private void UpdateCardLevelText()
    {
        cardLevelText.enabled = true;
        cardLevelText.text = GetCardLevelString();
    }

    private void UpdateCardLevelTextWithDecoder()
    {
        cardLevelTextDecoder.ResetDecoder();
        cardLevelTextDecoder.SetTextStrings(GetCardLevelString());
        cardLevelTextDecoder.Activate();
    }

    public void PlayLevelUpAnimation()
    {
        if (!cardLevelAlredyDisplayedMax)
        {
            StartCoroutine(DoPlayLevelUpAnimation());
            
            if (IsCardLevelMaxed())
            {
                cardLevelAlredyDisplayedMax = true;
            }
        }        
    }

    private IEnumerator DoPlayLevelUpAnimation()
    {
        cardLevelText.enabled = false;

        yield return new WaitUntil(() => !playingPlayCostAnimation);

        yield return new WaitForSeconds(0.4f);
        UpdateCardLevelTextWithDecoder();
    }


    public void InstantUpdatePlayCost(int amountToIncrement)
    {
        int endValue = Mathf.Max(PlayCost + amountToIncrement, TurretBuilding.MIN_PLAY_COST);
        CardData.SetPlayCost(endValue);
        PlayCost = endValue;
        InitCostText();
    }

    public void PlayUpdatePlayCostAnimation(int amountToIncrement)
    {
        int endValue = Mathf.Max(PlayCost + amountToIncrement, TurretBuilding.MIN_PLAY_COST);
        CardData.SetPlayCost(endValue);

        if (endValue > PlayCost)
        {
            StartCoroutine(DoPlayPlayIncrementCostAnimation(endValue));
        }
        else
        {
            StartCoroutine(DoPlayDecrementPlayCostAnimation(endValue));
        }
    }

    private IEnumerator DoPlayDecrementPlayCostAnimation(int endValue, int decrementAmountPerTick = 1, 
        float tickDuration = 0.03f, float startDelay = 0.4f)
    {
        playingPlayCostAnimation = true;
        yield return new WaitForSeconds(startDelay);

        int beforeEndValue = endValue + decrementAmountPerTick;
        while (PlayCost > beforeEndValue)
        {
            PlayCost -= decrementAmountPerTick;
            InitCostText();
            GameAudioManager.GetInstance().PlayConsoleTyping(0);
            yield return new WaitForSeconds(tickDuration);
        }

        PlayCost = endValue;
        InitCostText();
        GameAudioManager.GetInstance().PlayConsoleTyping(0);

        playingPlayCostAnimation = false;
    }

    private IEnumerator DoPlayPlayIncrementCostAnimation(int endValue, int incrementAmountPerTick = 1, 
        float tickDuration = 0.03f, float startDelay = 0.4f)
    {
        playingPlayCostAnimation = true;
        yield return new WaitForSeconds(startDelay);

        int beforeEndValue = endValue - incrementAmountPerTick;
        while (PlayCost < beforeEndValue)
        {
            PlayCost += incrementAmountPerTick;
            InitCostText();
            GameAudioManager.GetInstance().PlayConsoleTyping(0);
            yield return new WaitForSeconds(tickDuration);
        }

        PlayCost = endValue;
        InitCostText();
        GameAudioManager.GetInstance().PlayConsoleTyping(0);

        playingPlayCostAnimation = false;
    }



    // ICardDescriptionProvider OVERLOADS
    public ICardDescriptionProvider.SetupData[] GetAbilityDescriptionSetupData()
    {
        ICardDescriptionProvider.SetupData[] setupData = new ICardDescriptionProvider.SetupData[2];

        TurretPartAttack turretPartAttack = CardParts.Projectile;
        setupData[0] = new ICardDescriptionProvider.SetupData(
            turretPartAttack.abilityName,
            turretPartAttack.abilityDescription,
            turretPartAttack.abilitySprite,
            turretPartAttack.materialColor
        );

        if (hasBasePassiveAbility)
        {
            TurretPassiveBase turretPartBase = CardParts.Passive;
            setupData[1] = new ICardDescriptionProvider.SetupData(
                turretPartBase.passive.abilityName,
                turretPartBase.passive.abilityDescription,
                turretPartBase.visualInformation.sprite,
                turretPartBase.visualInformation.color
            );
        }
        else
        {
            setupData[1] = null;
        }

        return setupData;
    }

    public Vector3 GetCenterPosition()
    {
        return CardTransform.position;
    }






    public void PreviewChangeVisuals(TurretPartAttack newTurretPartAttack, TurretPartBody newTurretPartBody,
                                     TurretPartBase newTurretPartBase, TurretPassiveBase newTurretPassiveBase,
                                     CardPartBonusStats cardPartBonusStats,
                                     TurretBuildingCard originalCard, CardPartReplaceManager.PartType partType,
                                     CardUpgradeTurretPlayCostConfig playCostsConfig)
    {
        bool replacingWithSamePart = false;

        // ATTACK
        //if (newTurretPartAttack == null)
        if (partType != CardPartReplaceManager.PartType.ATTACK)
        {
            newTurretPartAttack = originalCard.CardParts.Projectile;
        }
        else
        {
            replacingWithSamePart = originalCard.HasSameAttackPart(newTurretPartAttack);
        }



        // BODY
        //if (newTurretPartBody == null)
        if (partType != CardPartReplaceManager.PartType.BODY)
        {
            newTurretPartBody = originalCard.CardParts.Body;
        }
        else
        {
            replacingWithSamePart = originalCard.HasSameBodyPart(newTurretPartBody);
        }



        // BASE
        //if (newTurretPartBase == null && newTurretPassiveBase == null)
        newTurretPartBase = originalCard.CardParts.Base;
        if (partType != CardPartReplaceManager.PartType.BASE)
        {
            newTurretPassiveBase = originalCard.CardParts.Passive;
        }
        else
        {
            replacingWithSamePart = originalCard.HasSameBasePassivePart(newTurretPassiveBase);
        }


        bool hasBasePassiveAbility = newTurretPassiveBase.passive.GetType() != typeof(BaseNullPassive);
        if (hasBasePassiveAbility)
        {
            basePassiveImage.transform.parent.gameObject.SetActive(true);

            basePassiveImage.sprite = newTurretPassiveBase.visualInformation.sprite;
            basePassiveImage.color = newTurretPassiveBase.visualInformation.color;
        }
        else
        {
            basePassiveImage.transform.parent.gameObject.SetActive(false);
        }




        // PLAY COST
        CardData = new TurretCardData(originalCard.CardData);
        CardData.SetPlayCost(originalCard.PlayCost);
        CardParts.SetBody(newTurretPartBody);
        CardParts.SetProjectile(newTurretPartAttack);
        CardParts.SetBaseAbility(newTurretPartBase);
        CardParts.SetPassiveAbility(newTurretPassiveBase);
        

        TurretCardStatsController tempStatsController = new TurretCardStatsController(originalCard.StatsController,
            newTurretPartBody.DamageStat, newTurretPartBody.ShotsPerSecondStat, newTurretPartBase.RadiusRangeStat);

        if (partType == CardPartReplaceManager.PartType.BONUS_STATS)
        {
            cardPartBonusStats.ApplyStatsModification(tempStatsController);
        }
        else
        {
            tempStatsController.UpdateCurrentStats();
        }

        cardBodyMaterial.SetColor("_PaintColor", newTurretPartAttack.materialColor); // Projectile color
        attackImage.sprite = newTurretPartAttack.abilitySprite;
        attackImage.color = newTurretPartAttack.materialColor;

        cardBodyMaterial.SetTexture("_MaskTexture", newTurretPartBody.materialTextureMap);
        _damageStatValueText.text = tempStatsController.DamageStatState.BaseValueText;
        _fireRateStatValueText.text = tempStatsController.ShotsPerSecondInvertedStatState.BaseValueText;

        cardBaseMaterial.SetTexture("_Texture", newTurretPartBase.materialTexture);
        cardBaseMaterial.SetColor("_Color", newTurretPartBase.materialColor);
        _rangeStatValueText.text = tempStatsController.RadiusRangeStatState.BaseValueText;

        PlayCost = CardData.PlayCost;
        InstantUpdatePlayCost(playCostsConfig.ComputeCardPlayCostIncrement(!replacingWithSamePart, this));


        // CARD LVL
        CardData.SetCardUpgradeLevel(originalCard.CardData.CardUpgradeLevel);        
        IncrementCardLevel(1);
        
    }



    public DescriptionCornerPositions GetCornerPositions()
    {
        return new DescriptionCornerPositions(leftDescriptionPosition.position, rightDescriptionPosition.position);
    }


    public void InBattleReplaceAttack(TurretPartAttack newTurretPartAttack, float delayBeforeAnimation)
    {
        TurretPartAttack oldTurretPartAttack = CardParts.Projectile;
        CardParts.SetProjectile(newTurretPartAttack);

        turretBuilding.ResetAttackPart(newTurretPartAttack);

        cardBodyMaterial.SetColor("_PaintColor", newTurretPartAttack.materialColor); // Projectile color


        float iconViewDuration = 0.1f;
        Sequence replaceAttackAnimation = DOTween.Sequence();
        replaceAttackAnimation.AppendInterval(delayBeforeAnimation);
        replaceAttackAnimation.AppendCallback(() => SetAttackIcon(newTurretPartAttack));

        for (int i = 0; i < 4; i++)
        {
            replaceAttackAnimation.AppendInterval(iconViewDuration);
            replaceAttackAnimation.AppendCallback(() =>
            {
                SetAttackIcon(oldTurretPartAttack);
                GameAudioManager.GetInstance().PlayCardInfoHidden();
            });            
            

            replaceAttackAnimation.AppendInterval(iconViewDuration);
            replaceAttackAnimation.AppendCallback(() => 
            { 
                SetAttackIcon(newTurretPartAttack);
                GameAudioManager.GetInstance().PlayCardInfoShown();
            });
        }    

    }

}
