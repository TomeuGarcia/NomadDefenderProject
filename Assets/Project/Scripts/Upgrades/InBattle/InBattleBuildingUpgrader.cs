using DG.Tweening;
using NodeEnums;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum TurretUpgradeType { ATTACK, CADENCE, RANGE, SUPPORT, NONE };

public abstract class InBattleBuildingUpgrader : MonoBehaviour
{
    [SerializeField] private Transform building;
    [SerializeField] private RectTransform mouseDetectionPanel;
    [SerializeField] private TMP_Text lvlText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Image costCurrencyImage;

    [SerializeField] private List<int> upgradeCosts = new List<int>();
    [SerializeField] protected List<Image> fillBars = new List<Image>();
    
    [SerializeField] private Color32 disalbedTextColor;

    public bool IsOpenWindowInCooldown { get; private set; }
    public bool IsWindowOpen { get; private set; }

    private bool hasGameFinished = false;
    protected Coroutine automaticCloseCoroutine = null;


    [Header("BUILDING TYPE")]
    [SerializeField] private BuildingCard.CardBuildingType buildingType = BuildingCard.CardBuildingType.NONE;
    private TurretBuilding turretBuilding;
    private SupportBuilding supportBuilding;
    private int maxLevels;


    [Header("FEEDBACK")]
    [SerializeField] private ParticleSystem canUpgradeParticles;
    private bool canUpgardeParticlesAreActive = false;
    [SerializeField] private Transform canUpgradeTextHolder;
    private Vector3 canUpgradeTextHolderStartPosition;
    [SerializeField] private CanvasGroup cgCanUpgradeText;


    [Header("QUICK LEVEL DISPLAY UI")]
    [SerializeField] protected RectTransform quickLevelDisplay;
    [SerializeField] protected CanvasGroup cgQuickLevelDisplay;
    [SerializeField] protected TextMeshProUGUI quickLevelDisplayText;

    [Header("NEW UI")]
    [Header("General")]
    [SerializeField] protected RectTransform newUiParent;
    [SerializeField] protected Image barImage;
    [SerializeField] protected Image backgroundImage;
    [SerializeField] protected CanvasGroup cgLvlText;
    [SerializeField] protected CanvasGroup cgCostText;
    protected Coroutine openAnimationCoroutine = null;
    protected Coroutine closeAnimationCoroutine = null;

    protected static Color fadedInColor = Color.white;
    protected static Color fadedOutColor = new Color(0.7f, 0.7f, 0.7f);
    protected static Color disabledColor = new Color(0.15f, 0.15f, 0.15f);


    private bool buildingOwnerWasPlaced = false;

    private CurrencyCounter currencyCounter = null;

    private float xOffset;
    private const int maxStatLevel = 5;
    private const int maxSupportStatLevel = 3;
    private const int maxUpgradeCount = 3;
    protected int currentBuildingLevel = 0;

    protected int attackLvl;
    protected int cadenceLvl;
    protected int rangeLvl;
    protected int supportLvl;

    protected float turretFillBarCoef;
    protected float supportFillBarCoef;

    private bool visible;

    private float lastScroll;

    public delegate void TurretUpgradeEvent(int newLevel);
    public static TurretUpgradeEvent OnTurretUpgrade;

    public delegate void BuildingUpgraderEvent(TurretUpgradeType upgradeType, int upgradeLevel);
    public BuildingUpgraderEvent OnUpgrade;
    private void InvokeOnUpgrade(TurretUpgradeType upgradeType) { if (OnUpgrade != null) OnUpgrade(upgradeType, currentBuildingLevel); }


    private void Awake()
    {
        AwakeInit();
    }

    protected virtual void AwakeInit()
    {
        xOffset = newUiParent.anchoredPosition.x;

        newUiParent.gameObject.SetActive(false);

        IsOpenWindowInCooldown = false;
        IsWindowOpen = false;
        hasGameFinished = false;

        costText.text = upgradeCosts[0].ToString();

        turretFillBarCoef = 100.0f / ((float)maxStatLevel * 100.0f);
        supportFillBarCoef = 100.0f / ((float)maxSupportStatLevel * 100.0f);

        visible = false;

        quickLevelDisplay.gameObject.SetActive(false);
        cgQuickLevelDisplay.alpha = 0f;

        canUpgradeParticles.gameObject.SetActive(false);
        canUpgardeParticlesAreActive = false;
        canUpgradeTextHolder.gameObject.SetActive(false);
        canUpgradeTextHolderStartPosition = canUpgradeTextHolder.position;
        cgCanUpgradeText.alpha = 0f;

        buildingOwnerWasPlaced = false;
    }

    private void Start()
    {
        if (buildingType == BuildingCard.CardBuildingType.TURRET)
        {
            turretBuilding = building.gameObject.GetComponent<TurretBuilding>();
            maxLevels = turretBuilding.CardLevel;
            UpdateAttackBar();
            UpdateCadenceBar();
            UpdateRangeBar();
        }
        else if (buildingType == BuildingCard.CardBuildingType.SUPPORT)
        {
            supportBuilding = building.gameObject.GetComponent<SupportBuilding>();
            maxLevels = maxUpgradeCount;
            UpdateSupportBar();
        }
        UpdateLevelText();
    }


    private void OnEnable()
    {
        TDGameManager.OnGameFinishStart += SetHasGameFinishedTrueAndCloseWindow;

        if (currencyCounter != null)
        {
            currencyCounter.OnCurrencyAdded += CheckHoveredButtonsCanNowUpgrade;
            currencyCounter.OnCurrencyAdded += CheckStartParticlesCanUpgrade;
            currencyCounter.OnCurrencySpent += CheckStopParticlesCanUpgrade;
        }        
    }
    private void OnDisable()
    {
        TDGameManager.OnGameFinishStart -= SetHasGameFinishedTrueAndCloseWindow;

        if (currencyCounter != null)
        {
            currencyCounter.OnCurrencyAdded -= CheckHoveredButtonsCanNowUpgrade;
            currencyCounter.OnCurrencyAdded -= CheckStartParticlesCanUpgrade;
            currencyCounter.OnCurrencySpent -= CheckStopParticlesCanUpgrade;
        }        
    }


    private void Update()
    {
        bool outOfArea = !IsHoveringWindow();

        if (outOfArea && Input.GetMouseButtonDown(0) && visible || Input.GetMouseButtonDown(1) || Input.mouseScrollDelta.y != lastScroll)
        {
            CloseWindow();
        }

        if (IsWindowOpen)
        {
            if (outOfArea && automaticCloseCoroutine == null)
            {
                AutomaticWindowCloseStart();
            }
            else if (!outOfArea && automaticCloseCoroutine != null)
            {
                AutomaticWindowCloseStop();
            }
        }

    }

    private void LateUpdate()
    {
        lastScroll = Input.mouseScrollDelta.y;
    }

    public virtual void InitTurret(int newAttackLvl, int newCadenceLvl, int newRangeLvl, CurrencyCounter newCurrencyCounter, 
        bool hasPassiveAbility, Sprite basePassiveSprite, Color basePassiveColor)
    {
        attackLvl = newAttackLvl;
        cadenceLvl = newCadenceLvl;
        rangeLvl = newRangeLvl;

        currencyCounter = newCurrencyCounter;
        currencyCounter.OnCurrencyAdded += CheckHoveredButtonsCanNowUpgrade;
        currencyCounter.OnCurrencyAdded += CheckStartParticlesCanUpgrade;
        currencyCounter.OnCurrencySpent += CheckStopParticlesCanUpgrade;
    }

    public virtual void InitSupport(int newRangeLvl, CurrencyCounter newCurrencyCounter, Sprite abilitySprite, Color abilityColor, TurretPartBase turretPartBase)
    {
        rangeLvl = newRangeLvl;

        supportLvl = 0;

        currencyCounter = newCurrencyCounter;
        currencyCounter.OnCurrencyAdded += CheckHoveredButtonsCanNowUpgrade;
        currencyCounter.OnCurrencyAdded += CheckStartParticlesCanUpgrade;
        currencyCounter.OnCurrencySpent += CheckStopParticlesCanUpgrade;
    }

    public void OnBuildingOwnerPlaced()
    {
        buildingOwnerWasPlaced = true;
        StartCoroutine(DelayedOnBuildingOwnerPlaced(0.5f));
    }
    private IEnumerator DelayedOnBuildingOwnerPlaced(float delay)
    {
        yield return new WaitForSeconds(delay);
        CheckStartParticlesCanUpgrade();
    }

    private void SetHasGameFinishedTrueAndCloseWindow()
    {
        if (IsWindowOpen)
        {
            CloseWindow();
        }
        hasGameFinished = true;        
    }
    public bool CanOpenWindow()
    {
        return !IsOpenWindowInCooldown && !IsWindowOpen && !hasGameFinished;
    }
    public void OpenWindow()
    {
        if(!UIWindowManager.GetInstance().IsHoveringOtherWindow(this))
        {
            UIWindowManager.GetInstance().OpenedWindow(this);

            newUiParent.gameObject.SetActive(true);

            newUiParent.position = Camera.main.WorldToScreenPoint(building.position) + Vector3.up * 50.0f + (Vector3.right * xOffset);
            StartCoroutine(SetVisible());

            PlayOpenAnimation();

            IsWindowOpen = true;

            HideQuickLevelDisplay();
        }
    }

    public void CloseWindow()
    {
        UIWindowManager.GetInstance().ClosedWindow(this);

        visible = false;
        //UIParent.gameObject.SetActive(false);
        
        PlayCloseAnimation();

        StartCoroutine(OpenWindowCooldown());

        IsWindowOpen = false;

        AutomaticWindowCloseStop();
    }
    private IEnumerator OpenWindowCooldown()
    {
        IsOpenWindowInCooldown = true;
        yield return new WaitForSeconds(0.1f);
        IsOpenWindowInCooldown = false;
    }


    public bool IsHoveringWindow()
    {
        return RectTransformUtility.RectangleContainsScreenPoint(mouseDetectionPanel, Input.mousePosition);
    }

    private IEnumerator SetVisible()
    {
        yield return null;
        visible = true;
    }

    public bool StatsLevelBellowLimit(bool isSupport)
    {
        bool allStatsAreMaxed = true;
        if (isSupport) allStatsAreMaxed &= IsStatMaxed(supportLvl);
        else allStatsAreMaxed &= IsStatMaxed(attackLvl) && IsStatMaxed(cadenceLvl) && IsStatMaxed(rangeLvl);

        return !allStatsAreMaxed;
    }

    public TurretUpgradeType GetLowestStatUpgradeType(bool includeSupport)
    {
        List<(int, TurretUpgradeType)> levelsByUpgradeTypes = new List<(int, TurretUpgradeType)>(4);
        if (!IsStatMaxed(attackLvl))
        {
            levelsByUpgradeTypes.Add((attackLvl, TurretUpgradeType.ATTACK));
        }
        if (!IsStatMaxed(cadenceLvl))
        {
            levelsByUpgradeTypes.Add((cadenceLvl, TurretUpgradeType.CADENCE));
        }
        if (!IsStatMaxed(rangeLvl))
        {
            levelsByUpgradeTypes.Add((rangeLvl, TurretUpgradeType.RANGE));
        }
        if (includeSupport && !IsStatMaxed(supportLvl))
        {
            levelsByUpgradeTypes.Add((supportLvl, TurretUpgradeType.SUPPORT));
        }

        if (levelsByUpgradeTypes.Count == 0) return TurretUpgradeType.NONE;

        levelsByUpgradeTypes.Sort((a, b) => { 
            if (a.Item1 > b.Item1) return 1; 
            else if (a.Item1 == b.Item1) return Random.Range(0, 2); 
            else return -1; 
        } );

        return levelsByUpgradeTypes[0].Item2;
    }

    public void FreeTurretUpgrade(TurretUpgradeType turretUpgradeType)
    {
        //NextLevel();
        //CheckStopParticlesCanUpgrade();

        if (turretUpgradeType == TurretUpgradeType.ATTACK)
        {
            attackLvl++;
            UpdateAttackBar();

            turretBuilding.Upgrade(TurretUpgradeType.ATTACK, attackLvl);
        }
        else if (turretUpgradeType == TurretUpgradeType.CADENCE)
        {
            cadenceLvl++;
            UpdateCadenceBar();

            turretBuilding.Upgrade(TurretUpgradeType.CADENCE, cadenceLvl);
        }
        else if (turretUpgradeType == TurretUpgradeType.RANGE)
        {
            rangeLvl++;
            UpdateRangeBar();

            turretBuilding.Upgrade(TurretUpgradeType.RANGE, rangeLvl);            
        }

        InvokeOnUpgrade(turretUpgradeType);
        CheckStopParticlesCanUpgrade();
    }

    public void UpgradedAttack() // Called by button
    {
        if(CanUpgrade(attackLvl))
        {
            currencyCounter.SubtractCurrency(upgradeCosts[currentBuildingLevel]);

            NextLevel();

            attackLvl++;
            UpdateAttackBar();

            turretBuilding.Upgrade(TurretUpgradeType.ATTACK, attackLvl);

            CheckStopParticlesCanUpgrade();
            
            PlayPositiveAnimationTextCostPunch();

            InvokeOnUpgrade(TurretUpgradeType.ATTACK);
        }
        else
        {
            PlayNegativeAnimationTextCostPunch();
            OnCanNotUpgradeAttack();
        }
    }

    public void UpgradedCadence() // Called by button
    {
        if (CanUpgrade(cadenceLvl))
        {
            currencyCounter.SubtractCurrency(upgradeCosts[currentBuildingLevel]);

            NextLevel();

            cadenceLvl++;
            UpdateCadenceBar();

            turretBuilding.Upgrade(TurretUpgradeType.CADENCE, cadenceLvl);

            CheckStopParticlesCanUpgrade();

            PlayPositiveAnimationTextCostPunch();

            InvokeOnUpgrade(TurretUpgradeType.CADENCE);
        }
        else
        {
            PlayNegativeAnimationTextCostPunch();
            OnCanNotUpgradeFireRate();
        }
    }

    public void UpgradedRange() // Called by button
    {
        if (CanUpgrade(rangeLvl))
        {
            currencyCounter.SubtractCurrency(upgradeCosts[currentBuildingLevel]);

            NextLevel();

            rangeLvl++;
            UpdateRangeBar();

            turretBuilding.Upgrade(TurretUpgradeType.RANGE, rangeLvl);

            CheckStopParticlesCanUpgrade();

            PlayPositiveAnimationTextCostPunch();

            InvokeOnUpgrade(TurretUpgradeType.RANGE);
        }
        else
        {
            PlayNegativeAnimationTextCostPunch();
            OnCanNotUpgradeRange();
        }
    }

    public void UpgradedSupport()
    {
        if (CanUpgrade(supportLvl))
        {
            currencyCounter.SubtractCurrency(upgradeCosts[currentBuildingLevel]);

            NextLevel();

            supportLvl++;
            UpdateSupportBar();

            supportBuilding.Upgrade(TurretUpgradeType.SUPPORT, supportLvl);

            CheckStopParticlesCanUpgrade();

            PlayPositiveAnimationTextCostPunch();

            InvokeOnUpgrade(TurretUpgradeType.SUPPORT);

            DoUpgradeSupport();
        }
        else
        {
            PlayNegativeAnimationTextCostPunch();
            OnCanNotUpgradeSupport();
        }
    }

    protected virtual void DoUpgradeSupport()
    {
    }

    protected bool CanUpgrade(int levelToCheck)
    {
        if (IsCardUpgradedToMax(currentBuildingLevel)) return false;

        return currentBuildingLevel < maxUpgradeCount && !IsStatMaxed(levelToCheck) && HasEnoughCurrencyToLevelUp();
    }
    protected bool IsCardUpgradedToMax(int levelToCheck)
    {
        return levelToCheck >= maxLevels;
    }
    protected bool IsStatMaxed(int levelToCheck)
    {
        return levelToCheck >= maxStatLevel;
    }
    protected bool HasEnoughCurrencyToLevelUp()
    {
        return currencyCounter.HasEnoughCurrency(upgradeCosts[currentBuildingLevel]);
    }

    private void NextLevel()
    {
        currentBuildingLevel++;
        UpdateLevelText();

        if(OnTurretUpgrade != null) { OnTurretUpgrade(currentBuildingLevel); }
    }

    private void UpdateLevelText()
    {
        lvlText.text = currentBuildingLevel.ToString() + "/" + maxLevels.ToString(); // Tomeu: I moved this here and commented if-else (A B)
        quickLevelDisplayText.text = currentBuildingLevel + "/" + maxLevels;

        //if (currentLevel < maxUpgradeCount)
        if (currentBuildingLevel < maxLevels)
        {
            //lvlText.text = "LVL " + currentLevel.ToString() + "/" + maxLevels.ToString(); // A
            costText.text = upgradeCosts[currentBuildingLevel].ToString();
        }
        else
        {
            //lvlText.text = "LVL MAX"; // B
            //costText.text = "NULL";
            costText.text = "0";
            costText.color = disalbedTextColor;
            costCurrencyImage.color = disalbedTextColor;
        }
    }

    protected virtual void UpdateAttackBar()
    {
    }
    protected virtual void UpdateCadenceBar()
    {
    }
    protected virtual void UpdateRangeBar()
    {
    }
    protected virtual void UpdateSupportBar()
    {
    }


    protected virtual void DisableButtons()
    {
    }



    // Animations
    protected virtual void PlayOpenAnimation()
    {
    }


    protected virtual void PlayCloseAnimation()
    {
    }
    


    protected void FillStatBar(Image bar, Image button, Image backFillBar, float backFill, bool highlight)
    {
        float duration = 0.2f;

        bar.DOComplete();
        bar.DOFillAmount(1f, duration);
        if (highlight)
        {
            bar.color = Color.cyan;
        }

        button.transform.DOComplete();
        button.transform.DOScale(Vector3.one * 1.1f, duration);
        button.DOBlendableColor(fadedInColor, duration*0.5f);

        backFillBar.fillAmount = backFill;

        GameAudioManager.GetInstance().PlayCardInfoShown();
    }

    protected void EmptyStatBar(Image bar, Image button, Image backFillBar, float backFill)
    {
        float duration = 0.2f;

        bar.DOComplete();
        bar.DOFillAmount(0f, duration);
        
        ResetStatBarColor(bar, button);

        button.transform.DOComplete();
        button.transform.DOScale(Vector3.one, duration);

        backFillBar.fillAmount = backFill;
    }

    protected void ResetStatBarColor(Image bar, Image button)
    {
        bar.color = Color.white;

        button.color = Color.white;
    }

    protected void PlayAnimationIconPunch(Transform iconTransform)
    {
        iconTransform.DOPunchScale(Vector3.one * 0.5f, 0.5f, 7).OnComplete(()=> iconTransform.localScale = Vector3.one);
    }

    protected void PlayAnimationTextCostPunch(Color flashColor, Color endColor, float duration, float punchScale, int punchVibrato)
    {
        costText.DOComplete();
        costText.transform.DOComplete();
        costCurrencyImage.DOComplete();

        //costText.DOColor(flashColor, duration).OnComplete(() => costText.DOColor(endColor, duration));
        costText.transform.DOPunchScale(Vector3.one * punchScale, duration * 2, punchVibrato);

        //costCurrencyImage.DOColor(flashColor, duration).OnComplete(() => costCurrencyImage.DOColor(endColor, duration));
    }
    protected void PlayPositiveAnimationTextCostPunch()
    {
        PlayAnimationTextCostPunch(Color.cyan, IsCardUpgradedToMax(currentBuildingLevel) ? disalbedTextColor : Color.white, 0.4f, 0.3f, 8);
    }
    protected void PlayNegativeAnimationTextCostPunch()
    {
        PlayAnimationTextCostPunch(Color.red, Color.white, 0.2f, 0.1f, 10);
        GameAudioManager.GetInstance().PlayError();
    }


    public void AutomaticWindowCloseStart()
    {
        automaticCloseCoroutine = StartCoroutine(DelayedAutomaticWindowClose());
    }

    public void AutomaticWindowCloseStop()
    {
        if (automaticCloseCoroutine == null) return;

        StopCoroutine(automaticCloseCoroutine);
        automaticCloseCoroutine = null;
    }

    private IEnumerator DelayedAutomaticWindowClose()
    {
        yield return new WaitForSeconds(4f);

        CloseWindow();
        automaticCloseCoroutine = null;
    }

    public void ShowQuickLevelDisplay()
    {
        if (IsWindowOpen) return;
        
        quickLevelDisplay.position = Camera.main.WorldToScreenPoint(building.position) + Vector3.down * 35.0f;
        quickLevelDisplay.gameObject.SetActive(true);
        cgQuickLevelDisplay.DOFade(1f, 0.1f);

        //ShowCanUpgradeText();
    }
    public void HideQuickLevelDisplay()
    {
        cgQuickLevelDisplay.DOFade(0f, 0.1f).OnComplete(() => quickLevelDisplay.gameObject.SetActive(false));        

        //HideCanUpgradeText();
    }




    protected bool IsBuildingUpgradeAvailable()
    {
        return buildingOwnerWasPlaced && !canUpgardeParticlesAreActive && !IsCardUpgradedToMax(currentBuildingLevel) && HasEnoughCurrencyToLevelUp();
    }
    private void CheckStartParticlesCanUpgrade()
    {
        if (IsBuildingUpgradeAvailable())
        {
            canUpgradeParticles.gameObject.SetActive(true);
            canUpgradeParticles.Play();
            canUpgardeParticlesAreActive = true;
            
            ShowCanUpgradeText();
        }

        if (!IsCardUpgradedToMax(currentBuildingLevel))
        {
            if (HasEnoughCurrencyToLevelUp())
            {
                costText.color = costCurrencyImage.color = fadedInColor;
            }
            else
            {
                costText.color = costCurrencyImage.color = Color.red;
            }
        }
    }
    protected virtual void CheckHoveredButtonsCanNowUpgrade()
    {
    }

    protected bool IsBuildingUpgradeNotAvailable()
    {
        return buildingOwnerWasPlaced && canUpgardeParticlesAreActive && (IsCardUpgradedToMax(currentBuildingLevel) || !HasEnoughCurrencyToLevelUp());
    }
    private void CheckStopParticlesCanUpgrade()
    {
        if (IsBuildingUpgradeNotAvailable())
        {
            canUpgradeParticles.Stop();
            canUpgardeParticlesAreActive = false;

            HideCanUpgradeText();
        }

        if (!IsCardUpgradedToMax(currentBuildingLevel))
        {
            if (HasEnoughCurrencyToLevelUp())
            {
                costText.color = costCurrencyImage.color = fadedInColor;
            }
            else
            {
                costText.color = costCurrencyImage.color = Color.red;
            }
        }
    }


    private void ShowCanUpgradeText()
    {
        canUpgradeTextHolder.gameObject.SetActive(true);
        canUpgradeTextHolder.localPosition = canUpgradeTextHolderStartPosition;
        cgCanUpgradeText.DOFade(1f, 0.3f);
        MoveUpCanUpgradeText();
    }
    private void HideCanUpgradeText()
    {
        canUpgradeTextHolder.DOComplete(false);
        cgCanUpgradeText.DOComplete(false);
        cgCanUpgradeText.DOFade(0f, 0.1f).OnComplete(
            () => canUpgradeTextHolder.gameObject.SetActive(false) );
    }

    private void MoveUpCanUpgradeText()
    {
        canUpgradeTextHolder.DOLocalMoveY(canUpgradeTextHolderStartPosition.y + 0.25f, 1f)
            .OnComplete( () => MoveDownCanUpgradeText() );
    }

    private void MoveDownCanUpgradeText()
    {
        canUpgradeTextHolder.DOLocalMoveY(canUpgradeTextHolderStartPosition.y, 1f)
            .OnComplete(() => MoveUpCanUpgradeText());
    }




    protected virtual void OnCanNotUpgradeAttack() { }
    protected virtual void OnCanNotUpgradeFireRate() { }
    protected virtual void OnCanNotUpgradeRange() { }
    protected virtual void OnCanNotUpgradeSupport() { }

    protected void ButtonPressedErrorFadeInOut(Button button, Image bar)
    {
        button.image.DOBlendableColor(Color.red, 0.1f).OnComplete(() => button.image.DOBlendableColor(fadedInColor, 0.1f));
        bar.DOBlendableColor(Color.red, 0.1f).OnComplete(() => bar.DOBlendableColor(Color.white, 0.1f));
    }

    protected void DisableButton(Button button, Image buttonImage)
    {
        button.interactable = false;
        buttonImage.color = disabledColor;

        button.DOKill();
        button.image.DOKill();
    }

    protected void ButtonFadeIn(Button button, bool onEndFadeOut = true)
    {
        if (!button.interactable) { return; }

        button.transform.DOScale(1.2f, 1.0f).OnComplete(() => { if (onEndFadeOut) ButtonFadeOut(button); });
        button.image.DOBlendableColor(fadedInColor, 1.0f);
    }

    protected void ButtonFadeOut(Button button, bool onEndFadeIn = true)
    {
        button.transform.DOScale(1.0f, 1.0f).OnComplete(() => { if (onEndFadeIn) ButtonFadeIn(button); });
        button.image.DOBlendableColor(fadedOutColor, 1.0f);
    }

    protected void StopButtonFade(Button button, bool goToFadedOut, bool highlight)
    {
        button.transform.DOKill();
        button.image.DOKill();
        
        if (highlight)
        {
            button.image.DOKill();
            button.image.DOBlendableColor(Color.cyan, 0.1f);
        }        

        if (goToFadedOut && button.interactable)
        {
            ButtonFadeOut(button, false);
        }
    }

    protected void SetBarAndButtonHighlighted(Image bar, Image button)
    {
        bar.DOComplete();
        bar.DOBlendableColor(Color.cyan, 0.1f);

        button.DOComplete();
        button.DOBlendableColor(Color.cyan, 0.1f);
    }
}
