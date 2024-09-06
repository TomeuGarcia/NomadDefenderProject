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



    [Header("VISUALS")]
    private TurretPartBody_View _turretMeshPreview;
    [SerializeField] private Transform _turretParentTransform;
    [SerializeField] private TurretIconCanvasDisplay[] _iconDisplays;
    
    private Material cardBodyMaterial, cardBaseMaterial;

    [SerializeField] private TextMeshProUGUI _damageStatValueText;
    [SerializeField] private TextMeshProUGUI _fireRateStatValueText;
    [SerializeField] private TextMeshProUGUI _rangeStatValueText;
    
    [SerializeField] protected TextMeshProUGUI cardLevelText;
    [SerializeField] private TextDecoder cardLevelTextDecoder;
    private bool cardLevelAlredyDisplayedMax = false;
    

    public bool ReplacedWithSamePart { get; private set; }
    private bool playingPlayCostAnimation = false;


    private TurretCardStatsController StatsController => CardData.StatsController;
    public ITurretStatsBonusController StatsBonusController => StatsController;
    private int PlayCost { get; set; }


    private void Awake()
    {
        AwakeInit(CardBuildingType.TURRET);
    }

    protected override void DoOnDestroy()
    {
        CardData.ResetProjectile();
    }

    protected override void InitVisuals()
    {
        TurretPartProjectileDataModel turretPartAttack = CardParts.Projectile;
        TurretPartBody turretPartBody = CardParts.Body;

        InstantiateTurretPreviewMesh();


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
        
        UpdateTurretBodyColor();
    }
    

    private void UpdateTurretBodyColor()
    {
        if(_turretMeshPreview == null)
        {
            InstantiateTurretPreviewMesh();
        }
        else
        {
            _turretMeshPreview.ResetProjectileMaterial(new Material(CardParts.Projectile.MaterialForTurretPreview));
        }
    }

    private void InstantiateTurretPreviewMesh()
    {
        if(_turretMeshPreview != null)
        {
            Destroy(_turretMeshPreview.transform.parent.gameObject);
        }

        _turretMeshPreview = Instantiate(CardParts.Body.previewPrefab, _turretParentTransform)
            .transform.GetChild(0).GetComponent<TurretPartBody_View>();
        _turretMeshPreview.InitMaterials(new Material(CardParts.Projectile.MaterialForTurretPreview));
        _turretMeshPreview.SetDefaultMaterial();

        _turretMeshPreview.transform.localRotation = Quaternion.identity;
        _turretMeshPreview.transform.localPosition = Vector3.zero;
        _turretMeshPreview.transform.localScale = Vector3.one;
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
        if (levelIncrement == 0)
        {
            return;
        }
        
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
        UpdateCardLevelView();
    }

    private void UpdateCardLevelTextWithDecoder()
    {
        cardLevelTextDecoder.ResetDecoder();
        cardLevelTextDecoder.SetTextStrings(GetCardLevelString());
        cardLevelTextDecoder.Activate();
        UpdateCardLevelView();
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

        UpdateIcons();

        //_turretMeshPreview.ResetProjectileMaterial(newTurretPartAttack.MaterialForTurret);

        _damageStatValueText.text = tempStatsController.DamageStatState.BaseValueText;
        _fireRateStatValueText.text = tempStatsController.ShotsPerSecondInvertedStatState.BaseValueText;

        _rangeStatValueText.text = tempStatsController.RadiusRangeStatState.BaseValueText;

        PlayCost = CardData.PlayCost;
        InstantUpdatePlayCost(playCostsConfig.ComputeCardPlayCostIncrement(!replacingWithSamePart, this));


        // CARD LVL
        CardData.SetCardUpgradeLevel(originalCard.CardData.CardUpgradeLevel);        
        IncrementCardLevel(1);
        
    }





    public void InBattleReplaceAttack(TurretPartProjectileDataModel newTurretProjectileModel, float delayBeforeAnimation)
    {
        TurretPartProjectileDataModel oldProjectile = CardParts.Projectile;
        
        CardData.SetProjectileTemporarily(newTurretProjectileModel);
        turretBuilding.ResetProjectilePart(newTurretProjectileModel);

        _turretMeshPreview.ResetProjectileMaterial(newTurretProjectileModel.MaterialForTurret);
        _turretMeshPreview.SetDefaultMaterial();

        StartCoroutine(
            PlayProjectileReplacementAnimation(oldProjectile, newTurretProjectileModel, delayBeforeAnimation));
    }

    private IEnumerator PlayProjectileReplacementAnimation(TurretPartProjectileDataModel oldProjectile,
        TurretPartProjectileDataModel newProjectile, float delayBeforeAnimation)
    {
        Material oldProjectileMaterial = new Material(oldProjectile.MaterialForTurretPreview);
        TurretIconCanvasDisplay.ConfigData oldProjectileIconData =
            new TurretIconCanvasDisplay.ConfigData(oldProjectile.abilitySprite, oldProjectile.materialColor);
        
        Material newProjectileMaterial = new Material(newProjectile.MaterialForTurretPreview);
        TurretIconCanvasDisplay.ConfigData newProjectileIconData =
            new TurretIconCanvasDisplay.ConfigData(newProjectile.abilitySprite, newProjectile.materialColor);
        
        TurretIconCanvasDisplay projectileIconDisplay = _iconDisplays[0];

        
        yield return new WaitForSeconds(delayBeforeAnimation);
        
        const float t1 = 0.1f;
        for (int i = 0; i < 2; ++i)
        {
            _turretMeshPreview.ResetProjectileMaterial(newProjectileMaterial);
            projectileIconDisplay.Init(newProjectileIconData);
            yield return new WaitForSeconds(t1);
            
            _turretMeshPreview.ResetProjectileMaterial(oldProjectileMaterial);
            projectileIconDisplay.Init(oldProjectileIconData);
            yield return new WaitForSeconds(t1);
        }
        _turretMeshPreview.ResetProjectileMaterial(newProjectileMaterial);
        projectileIconDisplay.Init(newProjectileIconData);
    }

    
    
        
    // ICardTooltipSource OVERLOADS
    public CardTooltipDisplayData MakeTooltipDisplayData()
    {
        return CardTooltipDisplayData.MakeForTurretCard(_descriptionTooltipPositioning, CardData);
    }
}
