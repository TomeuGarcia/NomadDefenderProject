using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ICardTooltipSource;


public class TurretBuildingCard : BuildingCard, ICardTooltipSource
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
    [SerializeField] private Image bodyImage;
    [SerializeField] private Image baseImage;
    [SerializeField] private TurretIconCanvasDisplay[] _iconDisplays;
    
    private Material cardBodyMaterial, cardBaseMaterial;

    [SerializeField] private TextMeshProUGUI _damageStatValueText;
    [SerializeField] private TextMeshProUGUI _fireRateStatValueText;
    [SerializeField] private TextMeshProUGUI _rangeStatValueText;
    
    [SerializeField] protected TextMeshProUGUI cardLevelText;
    [SerializeField] private TextDecoder cardLevelTextDecoder;
    private bool cardLevelAlredyDisplayedMax = false;
    

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
        cardBodyMaterial = new Material(bodyImage.material);
        bodyImage.material = cardBodyMaterial;
        cardBaseMaterial = new Material(baseImage.material);
        baseImage.material = cardBaseMaterial;
    }

    protected override void InitVisuals()
    {
        TurretPartProjectileDataModel turretPartAttack = CardParts.Projectile;
        TurretPartBody turretPartBody = CardParts.Body;

        // Mesh Materials
        if (cardBodyMaterial == null)
        {
            GetMaterialsRefs();
        }
        
        cardBodyMaterial.SetTexture("_MaskTexture", turretPartBody.materialTextureMap);

        cardBaseMaterial.SetTexture("_Texture", turretPartBody.BasePartPrimitive.MaterialTexture);
        cardBaseMaterial.SetColor("_Color", turretPartBody.BasePartPrimitive.MaterialColor);


        // Canvas
        _damageStatValueText.text = StatsController.DamageStatState.BaseValueText;
        _fireRateStatValueText.text = StatsController.ShotsPerSecondInvertedStatState.BaseValueText;
        _rangeStatValueText.text = StatsController.RadiusRangeStatState.BaseValueText;
        
        UpdateIcons();
        

        // Level
        UpdateCardLevelText();
    }

    private void UpdateIcons()
    {
        TurretIconCanvasDisplay.InitDisplaysArray(_iconDisplays, CardData.MakeIconsDisplayData());

        UpdateTurretBodyImageColor();
    }

    
    private void SetAttackIcon(TurretPartProjectileDataModel projectileModel)
    {
        _iconDisplays[0].Init(
            new TurretIconCanvasDisplay.ConfigData(projectileModel.abilitySprite, projectileModel.materialColor));

        UpdateTurretBodyImageColor();
    }

    private void UpdateTurretBodyImageColor()
    {
        cardBodyMaterial.SetColor("_PaintColor", CardParts.Projectile.materialColor);
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
    
    public void SetNewPartAttack(TurretPartProjectileDataModel newTurretPartAttack)
    {
        ReplacedWithSamePart = HasSameAttackPart(newTurretPartAttack); // Check replaced with same part
        CardParts.SetProjectile(newTurretPartAttack);
        
        Init();
    }
    public bool HasSameAttackPart(TurretPartProjectileDataModel newTurretPartAttack)
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

    public void AddNewPassive(ATurretPassiveAbilityDataModel newTurretPassive)
    {
        ReplacedWithSamePart = AlreadyHasPassive(newTurretPassive); // Check replaced with same part
        if (!ReplacedWithSamePart)
        {
            CardData.AddPassiveAbility(newTurretPassive);
        }

        Init();
    }

    public void AddPermanentBonusStats(CardPartBonusStats cardPartBonusStats)
    {
        cardPartBonusStats.ApplyStatsModification(StatsBonusController);
        Init();
    }


    public bool AlreadyHasPassive(ATurretPassiveAbilityDataModel newTurretPassive)
    {
        return CardData.PassiveAbilitiesController.AlreadyContainsPassive(newTurretPassive);
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
        CardTooltipDisplayManager.GetInstance().StartDisplayingTooltip(this);
    }

    public override void HideInfo()
    {
        base.HideInfo();
        CardTooltipDisplayManager.GetInstance()?.StopDisplayingTooltip();
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


    
    

    public void PreviewChangeVisuals(TurretPartProjectileDataModel newTurretPartAttack, TurretPartBody newTurretPartBody,
        ATurretPassiveAbilityDataModel newTurretPassive,
                                     CardPartBonusStats cardPartBonusStats,
                                     TurretBuildingCard originalCard, CardPartReplaceManager.PartType partType,
                                     CardUpgradeTurretPlayCostConfig playCostsConfig)
    {
        bool replacingWithSamePart = false;

        // ATTACK
        if (partType != CardPartReplaceManager.PartType.ATTACK)
        {
            newTurretPartAttack = originalCard.CardParts.Projectile;
        }
        else
        {
            replacingWithSamePart = originalCard.HasSameAttackPart(newTurretPartAttack);
        }



        // BODY
        if (partType != CardPartReplaceManager.PartType.BODY)
        {
            newTurretPartBody = originalCard.CardParts.Body;
        }
        else
        {
            replacingWithSamePart = originalCard.HasSameBodyPart(newTurretPartBody);
        }



        // BASE
        if (partType != CardPartReplaceManager.PartType.BASE)
        {
            newTurretPassive = null;
        }
        else
        {
            replacingWithSamePart = originalCard.AlreadyHasPassive(newTurretPassive);
            if (replacingWithSamePart)
            {
                newTurretPassive = null;
            }
        }




        // PLAY COST
        CardData = new TurretCardData(originalCard.CardData);
        CardData.SetPlayCost(originalCard.PlayCost);
        CardParts.SetBody(newTurretPartBody);
        CardParts.SetProjectile(newTurretPartAttack);
        if (newTurretPassive)
        {
            CardData.AddPassiveAbility(newTurretPassive);
        }


        TurretCardStatsController tempStatsController = new TurretCardStatsController(originalCard.StatsController,
            newTurretPartBody.DamageStat, newTurretPartBody.ShotsPerSecondStat, newTurretPartBody.RadiusRangeStat);

        if (partType == CardPartReplaceManager.PartType.BONUS_STATS)
        {
            cardPartBonusStats.ApplyStatsModification(tempStatsController);
        }
        else
        {
            tempStatsController.UpdateCurrentStats();
        }

        cardBodyMaterial.SetColor("_PaintColor", newTurretPartAttack.materialColor); // Projectile color
        
        UpdateIcons();


        cardBodyMaterial.SetTexture("_MaskTexture", newTurretPartBody.materialTextureMap);
        _damageStatValueText.text = tempStatsController.DamageStatState.BaseValueText;
        _fireRateStatValueText.text = tempStatsController.ShotsPerSecondInvertedStatState.BaseValueText;

        cardBaseMaterial.SetTexture("_Texture", newTurretPartBody.BasePartPrimitive.MaterialTexture);
        cardBaseMaterial.SetColor("_Color", newTurretPartBody.BasePartPrimitive.MaterialColor);
        _rangeStatValueText.text = tempStatsController.RadiusRangeStatState.BaseValueText;

        PlayCost = CardData.PlayCost;
        InstantUpdatePlayCost(playCostsConfig.ComputeCardPlayCostIncrement(!replacingWithSamePart, this));


        // CARD LVL
        CardData.SetCardUpgradeLevel(originalCard.CardData.CardUpgradeLevel);        
        IncrementCardLevel(1);
        
    }





    public void InBattleReplaceAttack(TurretPartProjectileDataModel newTurretPartAttack, float delayBeforeAnimation)
    {
        TurretPartProjectileDataModel oldTurretPartAttack = CardParts.Projectile;
        CardParts.SetProjectile(newTurretPartAttack);

        turretBuilding.ResetProjectilePart(newTurretPartAttack);

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

    
    
        
    // ICardTooltipSource OVERLOADS
    public CardTooltipDisplayData MakeTooltipDisplayData()
    {
        return CardTooltipDisplayData.MakeForTurretCard(_descriptionTooltipPositioning, CardData);
    }
}
