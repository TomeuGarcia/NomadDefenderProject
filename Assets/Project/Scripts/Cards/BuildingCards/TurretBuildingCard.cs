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

    private TurretBuilding.TurretBuildingStats turretStats;


    [Header("CARD INFO")]
    [SerializeField] private GameObject infoInterface;

    [Header("Attack card info")]
    [SerializeField] private RectTransform infoShownAttackIcon;
    [SerializeField] private RectTransform defaultAttackIcon; // used as Hidden info
    private Vector3 infoShownAttackIconPos;
    private Vector3 infoHiddenAttackIconPos;
    [SerializeField] private TextMeshProUGUI attackNameText;
    [SerializeField] private TextMeshProUGUI attackDescriptionText;

    [Header("Base card info")]
    [SerializeField] private RectTransform infoShownBaseIcon;
    [SerializeField] private RectTransform defaultBaseIcon; // used as Hidden info
    private Vector3 infoShownBaseIconPos;
    private Vector3 infoHiddenBaseIconPos;
    [SerializeField] private TextMeshProUGUI baseNameText;
    [SerializeField] private TextMeshProUGUI baseDescriptionText;



    [Header("VISUALS")]
    //[SerializeField] private MeshRenderer attackMeshRenderer;
    //[SerializeField] private MeshRenderer bodyMeshRenderer;
    //[SerializeField] private MeshRenderer baseMeshRenderer;
    [SerializeField] private Image attackImage;
    [SerializeField] private Image bodyImage;
    [SerializeField] private Image baseImage;
    private Material cardAttackMaterial, cardBodyMaterial, cardBaseMaterial;

    [SerializeField] private Image damageFillImage;
    [SerializeField] private Image cadenceFillImage;
    [SerializeField] private Image rangeFillImage;
    [SerializeField] private Image basePassiveImage;

    [SerializeField] protected TextMeshProUGUI cardLevelText;
    [SerializeField] private TextDecoder cardLevelTextDecoder;
    private bool cardLevelAlredyDisplayedMax = false;

    bool hasBasePassiveAbility = false;


    [HideInInspector] public bool ReplacedWithSamePart { get; private set; }
    private bool isPlayingSubtractCostAnimation = false;


    [Header("DESCRIPTION")]
    [SerializeField] private Transform leftDescriptionPosition;
    [SerializeField] private Transform rightDescriptionPosition;




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
        cardBodyMaterial.SetColor("_PaintColor", turretPartAttack.materialColor); // Projectile color

        cardBaseMaterial.SetTexture("_Texture", turretPartBase.materialTexture);
        cardBaseMaterial.SetColor("_Color", turretPartBase.materialColor);


        // Canvas
        damageFillImage.fillAmount = turretPartBody.GetDamagePer1();
        cadenceFillImage.fillAmount = turretPartBody.GetCadencePer1();
        rangeFillImage.fillAmount = turretPartBase.GetRangePer1();


        attackImage.sprite = turretPartAttack.abilitySprite;
        attackImage.color = turretPartAttack.materialColor;


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

        // Ability Info
        InitInfoVisuals();

        // Level
        UpdateCardLevelText();
    }


    protected override void InitStatsFromTurretParts()
    {

        turretStats.playCost = turretCardParts.GetCardCost();

        turretStats.damage = turretCardParts.turretPartBody.Damage;
        turretStats.range = turretCardParts.turretPartBase.Range;
        turretStats.cadence = turretCardParts.turretPartBody.Cadence;
    }

    public override void CreateCopyBuildingPrefab(Transform spawnTransform, CurrencyCounter currencyCounter)
    {
        copyBuildingPrefab = Instantiate(buildingPrefab, Vector3.zero, Quaternion.identity);
        copyBuildingPrefab.transform.SetParent(spawnTransform);

        copyBuildingPrefab.GetComponent<TurretBuilding>().Init(turretStats, turretCardParts, currencyCounter);
        copyBuildingPrefab.SetActive(false);
    }

    public override int GetCardPlayCost()
    {
        return turretStats.playCost;
    }


    public void ResetParts(TurretCardParts turretCardParts)
    {
        this.turretCardParts = ScriptableObject.CreateInstance("TurretCardParts") as TurretCardParts;
        this.turretCardParts.Init(turretCardParts);

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
    public bool HasSameBasePart(TurretPartBase newTurretPartBase, TurretPassiveBase newTurretPassiveBase)
    {
        return turretCardParts.turretPartBase == newTurretPartBase &&
               turretCardParts.turretPassiveBase == newTurretPassiveBase &&
               turretCardParts.turretPartBase.rangeLvl == newTurretPartBase.rangeLvl;
    }


    protected override void InitInfoVisuals()
    {
        attackNameText.text = '/' + turretCardParts.turretPartAttack.abilityName;
        attackDescriptionText.text = turretCardParts.turretPartAttack.abilityDescription;

        baseNameText.text = '/' + turretCardParts.turretPassiveBase.passive.abilityName;
        baseDescriptionText.text = turretCardParts.turretPassiveBase.passive.abilityDescription;
    }
    protected override void SetupCardInfo()
    {
        // general
        infoInterface.SetActive(true);
        isShowInfoAnimationPlaying = false;

        // attack
        infoShownAttackIconPos = infoShownAttackIcon.localPosition;
        infoHiddenAttackIconPos = defaultAttackIcon.localPosition;
        attackNameText.alpha = 0;
        attackDescriptionText.alpha = 0;

        // base
        infoShownBaseIconPos = infoShownBaseIcon.localPosition;
        infoHiddenBaseIconPos = defaultBaseIcon.localPosition;
        baseNameText.alpha = 0;
        baseDescriptionText.alpha = 0;
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
        CardDescriptionDisplayer.GetInstance()?.HideCardDescription();
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


        // show attack icon
        defaultAttackIcon.DOLocalMove(infoShownAttackIconPos, t2);
        GameAudioManager.GetInstance().PlayCardInfoMoveShown();
        yield return new WaitForSeconds(t2);

        // show base icon
        if (hasBasePassiveAbility)
        {
            defaultBaseIcon.DOLocalMove(infoShownBaseIconPos, t2);
            GameAudioManager.GetInstance().PlayCardInfoMoveShown();
            yield return new WaitForSeconds(t2);
        }

        // show attack text
        attackNameText.DOFade(1f, t2);
        GameAudioManager.GetInstance().PlayCardInfoShown();
        yield return new WaitForSeconds(t2);
        attackDescriptionText.DOFade(1f, t2);
        GameAudioManager.GetInstance().PlayCardInfoShown();
        yield return new WaitForSeconds(t2);

        // show base text
        if (hasBasePassiveAbility)
        {
            baseNameText.DOFade(1f, t2);
            GameAudioManager.GetInstance().PlayCardInfoShown();
            yield return new WaitForSeconds(t2);
            baseDescriptionText.DOFade(1f, t2);
            GameAudioManager.GetInstance().PlayCardInfoShown();
            yield return new WaitForSeconds(t2);
        }


        canInfoInteract = true;
        isShowInfoAnimationPlaying = false;
    }

    private IEnumerator HideInfoAnimation()
    {
        canInfoInteract = false;


        float t2 = 0.1f;

        // hide base text
        if (hasBasePassiveAbility)
        {
            baseDescriptionText.DOFade(0f, t2);
            GameAudioManager.GetInstance().PlayCardInfoHidden();
            yield return new WaitForSeconds(t2);
            baseNameText.DOFade(0f, t2);
            GameAudioManager.GetInstance().PlayCardInfoHidden();
            yield return new WaitForSeconds(t2);
        }

        // hide attack text
        attackDescriptionText.DOFade(0f, t2);
        GameAudioManager.GetInstance().PlayCardInfoHidden();
        yield return new WaitForSeconds(t2);
        attackNameText.DOFade(0f, t2);
        GameAudioManager.GetInstance().PlayCardInfoHidden();
        yield return new WaitForSeconds(t2);


        // hide base icon
        if (hasBasePassiveAbility)
        {
            defaultBaseIcon.DOLocalMove(infoHiddenBaseIconPos, t2);
            GameAudioManager.GetInstance().PlayCardInfoMoveHidden();
            yield return new WaitForSeconds(t2);
        }

        // hide attack icon
        defaultAttackIcon.DOLocalMove(infoHiddenAttackIconPos, t2);
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

        yield return new WaitUntil(() => !isPlayingSubtractCostAnimation);

        yield return new WaitForSeconds(0.4f);
        UpdateCardLevelTextWithDecoder();
    }


    
    public void SubtractPlayCost(int amountToSubtract, bool useAnimation = true)
    {
        int endValue = Mathf.Max(turretStats.playCost - amountToSubtract, TurretBuilding.MIN_PLAY_COST);
        turretCardParts.cardCost = endValue;
        if (useAnimation)
        {
            StartCoroutine(SubtractPlayCostAnimation(endValue));
        }
        else
        {
            turretStats.playCost = endValue;
            InitCostText();
        }
    }
    private IEnumerator SubtractPlayCostAnimation(int endValue)
    {
        isPlayingSubtractCostAnimation = true;

        yield return new WaitForSeconds(0.4f);

        while (turretStats.playCost > endValue)
        {
            --turretStats.playCost;
            InitCostText();
            GameAudioManager.GetInstance().PlayConsoleTyping(0);
            yield return new WaitForSeconds(0.05f);
        }

        isPlayingSubtractCostAnimation = false;
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
                                     TurretBuildingCard originalCard, CardPartReplaceManager.PartType partType,
                                     int playCostSubtractAmountSamePart)
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
        cardBodyMaterial.SetColor("_PaintColor", newTurretPartAttack.materialColor); // Projectile color
        attackImage.sprite = newTurretPartAttack.abilitySprite;
        attackImage.color = newTurretPartAttack.materialColor;


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
        cardBodyMaterial.SetTexture("_MaskTexture", newTurretPartBody.materialTextureMap);
        damageFillImage.fillAmount = newTurretPartBody.GetDamagePer1();
        cadenceFillImage.fillAmount = newTurretPartBody.GetCadencePer1();


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
        cardBaseMaterial.SetTexture("_Texture", newTurretPartBase.materialTexture);
        cardBaseMaterial.SetColor("_Color", newTurretPartBase.materialColor);
        rangeFillImage.fillAmount = newTurretPartBase.GetRangePer1();

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
        turretCardParts = new TurretCardParts();
        turretStats.playCost = originalCard.turretStats.playCost;
        if (replacingWithSamePart) SubtractPlayCost(playCostSubtractAmountSamePart, useAnimation: false);
        InitCostText();


        // CARD LVL
        turretCardParts.cardLevel = originalCard.turretCardParts.cardLevel;        
        IncrementCardLevel(1);
        UpdateCardLevelText();
    }



    public DescriptionCornerPositions GetCornerPositions()
    {
        return new DescriptionCornerPositions(leftDescriptionPosition.position, rightDescriptionPosition.position);
    }

}
