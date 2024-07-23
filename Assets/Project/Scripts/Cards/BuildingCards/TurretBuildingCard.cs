using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static ICardDescriptionProvider;


public class TurretBuildingCard : BuildingCard, ICardDescriptionProvider
{
    public TurretCardParts turretCardParts { get; private set; }
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


    private TurretCardStatsController StatsController => turretCardParts.StatsController;
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
        TurretPartAttack turretPartAttack = turretCardParts.turretPartAttack;
        TurretPartBody turretPartBody = turretCardParts.turretPartBody;
        TurretPartBase turretPartBase = turretCardParts.turretPartBase;

        // Mesh Materials
        cardBodyMaterial.SetTexture("_MaskTexture", turretPartBody.materialTextureMap);

        cardBaseMaterial.SetTexture("_Texture", turretPartBase.materialTexture);
        cardBaseMaterial.SetColor("_Color", turretPartBase.materialColor);


        // Canvas
        _damageStatValueText.text = StatsController.DamageStatState.BaseDamageText;
        _fireRateStatValueText.text = StatsController.ShotsPerSecondStatState.BaseShotsPerSecondText;
        _rangeStatValueText.text = StatsController.RadiusRangeStatState.BaseRadiusRangeText;


        SetAttackIcon(turretPartAttack);


        hasBasePassiveAbility = turretCardParts.turretPassiveBase.passive.GetType() != typeof(BaseNullPassive);

        if (hasBasePassiveAbility)
        {
            basePassiveImage.transform.parent.gameObject.SetActive(true);

            basePassiveImage.sprite = turretCardParts.turretPassiveBase.visualInformation.sprite;
            basePassiveImage.color = turretCardParts.turretPassiveBase.visualInformation.color;
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
        PlayCost = turretCardParts.GetCardCost();
    }

    public override void CreateCopyBuildingPrefab(Transform spawnTransform, CurrencyCounter currencyCounter)
    {
        copyBuildingPrefab = Instantiate(buildingPrefab, Vector3.zero, Quaternion.identity);
        copyBuildingPrefab.transform.SetParent(spawnTransform);

        turretBuilding = copyBuildingPrefab.GetComponent<TurretBuilding>();
        turretBuilding.Init(StatsController, turretCardParts, currencyCounter);
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

    public void ResetParts(TurretCardParts turretCardParts)
    {
        this.turretCardParts = ScriptableObject.CreateInstance<TurretCardParts>();
        this.turretCardParts.InitCopyingReferences(turretCardParts);

        Init();
    }


    public void SetNewPartAttack(TurretPartAttack newTurretPartAttack)
    {
        ReplacedWithSamePart = HasSameAttackPart(newTurretPartAttack); // Check replaced with same part

        int costHolder = turretCardParts.turretPartAttack.cost;
        turretCardParts.turretPartAttack = newTurretPartAttack;
        turretCardParts.turretPartAttack.cost = costHolder;
        Init();
    }
    public bool HasSameAttackPart(TurretPartAttack newTurretPartAttack)
    {
        return turretCardParts.turretPartAttack == newTurretPartAttack;
    }

    public void SetNewPartBody(TurretPartBody newTurretPartBody)
    {
        ReplacedWithSamePart = HasSameBodyPart(newTurretPartBody); // Check replaced with same part

        int costHolder = turretCardParts.turretPartBody.cost;
        turretCardParts.turretPartBody = newTurretPartBody;
        turretCardParts.turretPartBody.cost = costHolder;
        Init();
    }
    public bool HasSameBodyPart(TurretPartBody newTurretPartBody)
    {
        return turretCardParts.turretPartBody == newTurretPartBody;
    }

    public void SetNewPartBase(TurretPartBase newTurretPartBase, TurretPassiveBase newTurretPassiveBase)
    {
        ReplacedWithSamePart = HasSameBasePart(newTurretPartBase, newTurretPassiveBase); // Check replaced with same part

        //int costHolder = turretCardParts.turretPartBase.cost + turretCardParts.turretPassiveBase.cost;
        int costHolder = turretCardParts.turretPartBase.cost + turretCardParts.turretPassiveBase.cost;

        turretCardParts.turretPartBase = newTurretPartBase;
        turretCardParts.turretPartBase.cost = costHolder;

        turretCardParts.turretPassiveBase = newTurretPassiveBase;
        turretCardParts.turretPassiveBase.cost = 0;
        
        Init();
    }

    public void AddPermanentBonusStats(CardPartBonusStats cardPartBonusStats)
    {
        cardPartBonusStats.ApplyStatsModification(StatsBonusController);
        Init();
    }


    public bool HasSameBasePart(TurretPartBase newTurretPartBase, TurretPassiveBase newTurretPassiveBase)
    {
        return turretCardParts.turretPartBase == newTurretPartBase &&
               turretCardParts.turretPassiveBase == newTurretPassiveBase &&
               turretCardParts.turretPartBase.RadiusRangeStat.ComputeValueByLevel(0).AlmostEquals(newTurretPartBase.RadiusRangeStat.ComputeValueByLevel(0));
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
        return turretCardParts.cardLevel;
    }

    public void IncrementCardLevel(int levelIncrement)
    {
        cardLevelAlredyDisplayedMax = IsCardLevelMaxed();
        turretCardParts.cardLevel = Mathf.Clamp(turretCardParts.cardLevel + levelIncrement, 1, TurretCardParts.MAX_CARD_LEVEL);        
    }

    public bool IsCardLevelMaxed()
    {
        return turretCardParts.cardLevel == TurretCardParts.MAX_CARD_LEVEL;
    }
    private string GetCardLevelString()
    {
        int level = turretCardParts.cardLevel;
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
        turretCardParts.cardCost = endValue;
        PlayCost = endValue;
        InitCostText();
    }

    public void PlayUpdatePlayCostAnimation(int amountToIncrement)
    {
        int endValue = Mathf.Max(PlayCost + amountToIncrement, TurretBuilding.MIN_PLAY_COST);
        turretCardParts.cardCost = endValue;

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

        TurretPartAttack turretPartAttack = turretCardParts.turretPartAttack;
        setupData[0] = new ICardDescriptionProvider.SetupData(
            turretPartAttack.abilityName,
            turretPartAttack.abilityDescription,
            turretPartAttack.abilitySprite,
            turretPartAttack.materialColor
        );

        if (hasBasePassiveAbility)
        {
            TurretPassiveBase turretPartBase = turretCardParts.turretPassiveBase;
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
            newTurretPartAttack = originalCard.turretCardParts.turretPartAttack;
        }
        else
        {
            replacingWithSamePart = originalCard.HasSameAttackPart(newTurretPartAttack);
        }



        // BODY
        //if (newTurretPartBody == null)
        if (partType != CardPartReplaceManager.PartType.BODY)
        {
            newTurretPartBody = originalCard.turretCardParts.turretPartBody;
        }
        else
        {
            replacingWithSamePart = originalCard.HasSameBodyPart(newTurretPartBody);
        }



        // BASE
        //if (newTurretPartBase == null && newTurretPassiveBase == null)
        if (partType != CardPartReplaceManager.PartType.BASE)
        {
            newTurretPartBase = originalCard.turretCardParts.turretPartBase;
            newTurretPassiveBase = originalCard.turretCardParts.turretPassiveBase;
        }
        else
        {
            replacingWithSamePart = originalCard.HasSameBasePart(newTurretPartBase, newTurretPassiveBase);
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
        turretCardParts = ScriptableObject.CreateInstance<TurretCardParts>();
        PlayCost = originalCard.PlayCost;

        turretCardParts.turretPartAttack = newTurretPartAttack;
        turretCardParts.turretPartBody = newTurretPartBody;
        turretCardParts.turretPartBase = newTurretPartBase;
        turretCardParts.turretPassiveBase = newTurretPassiveBase;


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
        _damageStatValueText.text = tempStatsController.DamageStatState.BaseDamageText;
        _fireRateStatValueText.text = tempStatsController.ShotsPerSecondStatState.BaseShotsPerSecondText;

        cardBaseMaterial.SetTexture("_Texture", newTurretPartBase.materialTexture);
        cardBaseMaterial.SetColor("_Color", newTurretPartBase.materialColor);
        _rangeStatValueText.text = tempStatsController.RadiusRangeStatState.BaseRadiusRangeText;

        InstantUpdatePlayCost(playCostsConfig.ComputeCardPlayCostIncrement(!replacingWithSamePart, this));


        // CARD LVL
        turretCardParts.cardLevel = originalCard.turretCardParts.cardLevel;        
        IncrementCardLevel(1);
        UpdateCardLevelText();
    }



    public DescriptionCornerPositions GetCornerPositions()
    {
        return new DescriptionCornerPositions(leftDescriptionPosition.position, rightDescriptionPosition.position);
    }


    public void InBattleReplaceAttack(TurretPartAttack newTurretPartAttack, float delayBeforeAnimation)
    {
        TurretPartAttack oldTurretPartAttack = turretCardParts.turretPartAttack;
        turretCardParts.turretPartAttack = newTurretPartAttack;

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
