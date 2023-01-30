using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static BuildingCard;

public class TurretBuildingCard : BuildingCard
{
    [System.Serializable]
    public class TurretCardParts
    {
        public TurretCardParts(TurretPartAttack turretPartAttack, TurretPartBody turretPartBody,
               TurretPartBase turretPartBase, TurretPassiveBase turretPassiveBase, int cardCost)
        {
            this.turretPartAttack = turretPartAttack;
            this.turretPartBody = turretPartBody;
            this.turretPartBase = turretPartBase;
            this.turretPassiveBase = turretPassiveBase;
            this.cardCost = cardCost;
        }

        public TurretCardParts(TurretPartAttack turretPartAttack, TurretPartBody turretPartBody,
               TurretPartBase turretPartBase, TurretPassiveBase turretPassiveBase)
        {
            this.turretPartAttack = turretPartAttack;
            this.turretPartBody = turretPartBody;
            this.turretPartBase = turretPartBase;
            this.turretPassiveBase = turretPassiveBase;
            this.cardCost = GetCostCombinedParts();
        }

        public TurretCardParts(TurretCardParts other)
        {
            this.turretPartAttack = other.turretPartAttack;
            this.turretPartBody = other.turretPartBody;
            this.turretPartBase = other.turretPartBase;
            this.turretPassiveBase = other.turretPassiveBase;
            this.cardCost = other.cardCost;
        }

        public TurretPartAttack turretPartAttack;
        public TurretPartBody turretPartBody;
        public TurretPartBase turretPartBase;
        public TurretPassiveBase turretPassiveBase;
        public int cardCost;

        public int GetCostCombinedParts()
        {
            return turretPartAttack.cost + turretPartBody.cost + turretPartBase.cost + turretPassiveBase.cost;
        }

        public int GetCardCost()
        { return cardCost; }
    }


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

    bool hasBasePassiveAbility = false;

    Coroutine showInfoCoroutine = null;
    bool isShowInfoAnimationPlaying = false;
    bool isHideInfoAnimationPlaying = false;




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
        cardAttackMaterial.SetTexture("_Texture", turretPartAttack.materialTexture);
        cardAttackMaterial.SetColor("_Color", turretPartAttack.materialColor);

        cardBodyMaterial.SetTexture("_MaskTexture", turretPartBody.materialTextureMap);
        cardBodyMaterial.SetColor("_PaintColor", turretPartAttack.materialColor); // Projectile color

        cardBaseMaterial.SetTexture("_Texture", turretPartBase.materialTexture);
        cardBaseMaterial.SetColor("_Color", turretPartBase.materialColor);


        // Canvas
        damageFillImage.fillAmount = turretPartBody.GetDamagePer1();
        cadenceFillImage.fillAmount = turretPartBody.GetCadencePer1();
        rangeFillImage.fillAmount = turretPartBase.GetRangePer1();

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
        InitInfoVisals();
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
        this.turretCardParts = new TurretCardParts(turretCardParts);
        Init();
    }


    public void SetNewPartAttack(TurretPartAttack newTurretPartAttack)
    {
        int costHolder = turretCardParts.turretPartAttack.cost;
        turretCardParts.turretPartAttack = newTurretPartAttack;
        turretCardParts.turretPartAttack.cost = costHolder;
        Init();
    }

    public void SetNewPartBody(TurretPartBody newTurretPartBody)
    {
        int costHolder = turretCardParts.turretPartBody.cost;
        turretCardParts.turretPartBody = newTurretPartBody;
        turretCardParts.turretPartBody.cost = costHolder;
        Init();
    }

    public void SetNewPartBase(TurretPartBase newTurretPartBase, TurretPassiveBase newTurretPassiveBase)
    {
        //int costHolder = turretCardParts.turretPartBase.cost + turretCardParts.turretPassiveBase.cost;
        int costHolder = turretCardParts.turretPartBase.cost + turretCardParts.turretPassiveBase.cost;

        turretCardParts.turretPartBase = newTurretPartBase;
        turretCardParts.turretPartBase.cost = costHolder;

        turretCardParts.turretPassiveBase = newTurretPassiveBase;
        turretCardParts.turretPassiveBase.cost = 0;

        Init();
    }


    protected override void InitInfoVisals()
    {
        attackNameText.text = '/' + turretCardParts.turretPartAttack.abilityName;
        attackDescriptionText.text = turretCardParts.turretPartAttack.abilityDescription;

        baseNameText.text = '/' + turretCardParts.turretPassiveBase.passive.abilityName;
        baseDescriptionText.text = turretCardParts.turretPassiveBase.passive.abilityDescription;
    }

    public override void ShowInfo()
    {
        base.ShowInfo();
        //interfaceCanvasGroup.alpha = 0f;

        showInfoCoroutine = StartCoroutine(ShowInfoAnimation());
    }
    public override void HideInfo()
    {
        if (isHideInfoAnimationPlaying) return;

        base.HideInfo();
        //interfaceCanvasGroup.alpha = 1f;        

        if (isShowInfoAnimationPlaying)
        {
            StopCoroutine(showInfoCoroutine);
        }        

        StartCoroutine(HideInfoAnimation());
    }

    private void SetupCardInfo()
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

    private IEnumerator ShowInfoAnimation()
    {
        canInfoInteract = false;
        isShowInfoAnimationPlaying = true;

        // hide generics
        float t = 0.05f;
        for (int i = 0; i < cgsInfoHide.Length; ++i)            
        {
            cgsInfoHide[i].DOFade(0f, t);
            yield return new WaitForSeconds(t);
        }


        float t2 = 0.1f;


        // show attack icon
        defaultAttackIcon.DOLocalMove(infoShownAttackIconPos, t2);
        yield return new WaitForSeconds(t2);

        // show base icon
        if (hasBasePassiveAbility)
        {
            defaultBaseIcon.DOLocalMove(infoShownBaseIconPos, t2);
            yield return new WaitForSeconds(t2);
        }

        // show attack text
        attackNameText.DOFade(1f, t2);
        yield return new WaitForSeconds(t2);
        attackDescriptionText.DOFade(1f, t2);
        yield return new WaitForSeconds(t2);

        // show base text
        if (hasBasePassiveAbility)
        {
            baseNameText.DOFade(1f, t2);
            yield return new WaitForSeconds(t2);
            baseDescriptionText.DOFade(1f, t2);
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
            yield return new WaitForSeconds(t2);
            baseNameText.DOFade(0f, t2);
            yield return new WaitForSeconds(t2);
        }

        // hide attack text
        attackDescriptionText.DOFade(0f, t2);
        yield return new WaitForSeconds(t2);
        attackNameText.DOFade(0f, t2);
        yield return new WaitForSeconds(t2);


        // hide base icon
        if (hasBasePassiveAbility)
        {
            defaultBaseIcon.DOLocalMove(infoHiddenBaseIconPos, t2);
            yield return new WaitForSeconds(t2);
        }

        // hide attack icon
        defaultAttackIcon.DOLocalMove(infoHiddenAttackIconPos, t2);
        yield return new WaitForSeconds(t2);


        // show generics
        float t = 0.05f;

        for (int i = cgsInfoHide.Length - 1; i >= 0; --i)
        {
            cgsInfoHide[i].DOFade(1f, t);
            yield return new WaitForSeconds(t);
        }


        canInfoInteract = true;
    }


}
