using System;
using DG.Tweening;
using NodeEnums;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum TurretUpgradeType { ATTACK, CADENCE, RANGE, SUPPORT, NONE };

public abstract class InBattleBuildingUpgrader : MonoBehaviour, InBattleUpgradeConditionChecker
{
    [SerializeField] private RangeBuilding _building;
    [SerializeField] private RectTransform mouseDetectionPanel;
    [SerializeField] private TMP_Text lvlText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Image costCurrencyImage;

    [SerializeField] private CardStatUpgradeCostsConfig _upgradeCostsConfig;
    private int CurrentUpgradeCosts => _upgradeCostsConfig.GetCostByLevel(CurrentBuildingLevel+1);
    
    [SerializeField] private Color32 disalbedTextColor;

    public bool IsOpenWindowInCooldown { get; private set; }
    public bool IsWindowOpen { get; private set; }

    private bool hasGameFinished = false;
    protected Coroutine automaticCloseCoroutine = null;


    [Header("BUILDING TYPE")]
    [SerializeField] private BuildingCard.CardBuildingType buildingType = BuildingCard.CardBuildingType.NONE;
    protected int maxLevels;


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
    [Header("Selling")] 
    [SerializeField] private SellBuildingMenu _sellBuildingMenu;
    
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
    protected const int maxUpgradeCount = 3;
    protected abstract int CurrentBuildingLevel { get; }
    protected IBuildingUpgradesController _buildingUpgradesController;


    private bool visible;

    private float lastScroll;

    public delegate void TurretUpgradeEvent(int newLevel);
    public static TurretUpgradeEvent OnTurretUpgrade;
    public static Action OnBuildingUpgraded;

    public delegate void BuildingUpgraderEvent(TurretUpgradeType upgradeType, int upgradeLevel);
    public BuildingUpgraderEvent OnUpgrade;
    protected void InvokeOnUpgrade(TurretUpgradeType upgradeType) { OnUpgrade?.Invoke(upgradeType, CurrentBuildingLevel); }


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

        costText.text = _upgradeCostsConfig.GetCostByLevel(1).ToString();

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

        _sellBuildingMenu.OnSellConfirmed += OnSellConfirmed;
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
        
        _sellBuildingMenu.OnSellConfirmed -= OnSellConfirmed;
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

    public virtual void InitTurret(TurretBuilding turretBuilding, 
        IBuildingUpgradesController buildingUpgradesController, ITurretStatsStateSource turretStatsState, 
        int numberOfUpgrades, CurrencyCounter newCurrencyCounter, TurretIconCanvasDisplay.ConfigData[] iconsDisplayData)
    {
        SharedInit(turretBuilding.CardData.PlayCost, buildingUpgradesController, newCurrencyCounter);
    }

    public virtual void InitSupport(SupportBuilding supportBuilding,
        IBuildingUpgradesController buildingUpgradesController,
        CurrencyCounter newCurrencyCounter, Sprite abilitySprite, Color abilityColor, SupportCardData supportCardData)
    {
        SharedInit(supportBuilding.CardData.PlayCost, buildingUpgradesController, newCurrencyCounter);
    }

    private void SharedInit(int cardPlayCost,
        IBuildingUpgradesController buildingUpgradesController, CurrencyCounter newCurrencyCounter)
    {
        _buildingUpgradesController = buildingUpgradesController;
        currencyCounter = newCurrencyCounter;
        currencyCounter.OnCurrencyAdded += CheckHoveredButtonsCanNowUpgrade;
        currencyCounter.OnCurrencyAdded += CheckStartParticlesCanUpgrade;
        currencyCounter.OnCurrencySpent += CheckStopParticlesCanUpgrade;
        
        _sellBuildingMenu.Init(cardPlayCost);
    }

    public virtual void OnStatsUpdated()
    {
        UpdateAllStatsView();
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
        if(UIWindowManager.GetInstance().IsHoveringOtherWindow(this))
        {
            return;
        }

        UIWindowManager.GetInstance().OpenedWindow(this);

        newUiParent.gameObject.SetActive(true);

        newUiParent.position = Camera.main.WorldToScreenPoint(_building.Position) + Vector3.up * 50.0f + (Vector3.right * xOffset);
        StartCoroutine(SetVisible());

        PlayOpenAnimation();

        IsWindowOpen = true;

        HideQuickLevelDisplay();
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
        bool allStatsAreMaxed = false;
        if (isSupport) 
        {
            allStatsAreMaxed = CurrentBuildingLevel >= maxLevels;
        }

        return !allStatsAreMaxed;
    }

    public virtual void FreeTurretUpgrade() { }

    protected void SpendUpgradeCost()
    {
        currencyCounter.SubtractCurrency(CurrentUpgradeCosts);
    }


    protected bool CanUpgrade()
    {
        if (IsCardUpgradedToMax(CurrentBuildingLevel)) return false;

        return CurrentBuildingLevel < maxUpgradeCount && HasEnoughCurrencyToLevelUp();
    }
    protected bool IsCardUpgradedToMax(int levelToCheck)
    {
        return levelToCheck >= maxLevels;
    }

    public bool HasEnoughCurrencyToLevelUp()
    {
        return currencyCounter.HasEnoughCurrency(CurrentUpgradeCosts);
    }

    protected void NextLevel()
    {        
        UpdateLevelText();

        if (OnTurretUpgrade != null) { OnTurretUpgrade(CurrentBuildingLevel); }
    }   

    private void UpdateLevelText()
    {
        lvlText.text = CurrentBuildingLevel.ToString() + "/" + maxLevels.ToString(); // Tomeu: I moved this here and commented if-else (A B)
        quickLevelDisplayText.text = CurrentBuildingLevel + "/" + maxLevels;

        //if (currentLevel < maxUpgradeCount)
        if (CurrentBuildingLevel < maxLevels)
        {
            //lvlText.text = "LVL " + currentLevel.ToString() + "/" + maxLevels.ToString(); // A
            costText.text = CurrentUpgradeCosts.ToString();
        }
        else
        {
            //lvlText.text = "LVL MAX"; // B
            //costText.text = "NULL";
            costText.text = "MAX";
            costText.color = disalbedTextColor;
            costCurrencyImage.color = disalbedTextColor;
        }
    }

    protected virtual void UpdateAllStatsView()
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
        StartCoroutine(_sellBuildingMenu.PlayOpenAnimation());
    }


    protected virtual void PlayCloseAnimation()
    {
        StartCoroutine(_sellBuildingMenu.PlayCloseAnimation());
    }

    protected void OnUpgraded()
    {
        OnBuildingUpgraded?.Invoke();
        _sellBuildingMenu.AddSellValue(_upgradeCostsConfig.GetCostByLevel(CurrentBuildingLevel));
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
        PlayAnimationTextCostPunch(Color.cyan, IsCardUpgradedToMax(CurrentBuildingLevel) ? disalbedTextColor : Color.white, 0.4f, 0.3f, 8);
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
        yield return new WaitForSecondsRealtime(4f);

        CloseWindow();
        automaticCloseCoroutine = null;
    }

    public void ShowQuickLevelDisplay()
    {
        if (IsWindowOpen) return;
        
        quickLevelDisplay.position = Camera.main.WorldToScreenPoint(_building.Position) + Vector3.down * 35.0f;
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
        return buildingOwnerWasPlaced && !canUpgardeParticlesAreActive && !IsCardUpgradedToMax(CurrentBuildingLevel) && HasEnoughCurrencyToLevelUp();
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

        if (!IsCardUpgradedToMax(CurrentBuildingLevel))
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
        return buildingOwnerWasPlaced && canUpgardeParticlesAreActive && (IsCardUpgradedToMax(CurrentBuildingLevel) || !HasEnoughCurrencyToLevelUp());
    }
    protected void CheckStopParticlesCanUpgrade()
    {
        if (IsBuildingUpgradeNotAvailable())
        {
            canUpgradeParticles.Stop();
            canUpgardeParticlesAreActive = false;

            HideCanUpgradeText();
        }

        if (!IsCardUpgradedToMax(CurrentBuildingLevel))
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


    private void OnSellConfirmed()
    {
        CloseWindow();
    }

}
