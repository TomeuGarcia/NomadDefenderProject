using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class Support_InBattleBuildingUpgrader : InBattleBuildingUpgrader
{

    [Header("Ability")]
    [SerializeField] private CanvasGroup cgAbilityStat;
    [SerializeField] private Image supportIcon;
    [SerializeField] private Button abilityButton;
    [SerializeField] private Image abilityButtonImage;
    [SerializeField] private Image abilityBackFillImage;
    [SerializeField] private Image abilityFillImage;
    [SerializeField] private Image abilityBarToCurrencyCost;

    [Header("Upgrade Descriptions")]
    [SerializeField] private TextMeshProUGUI nextUpgradeText;
    [SerializeField] private TextMeshProUGUI nextUpgradeDescriptionText;
    [SerializeField] private CanvasGroup cgNextUpgradeDescription;
    private bool isAbilityButtonHovered = false;

    private TurretPartBase turretPartBase;


    protected override void AwakeInit()
    {
        base.AwakeInit();
        abilityBarToCurrencyCost.fillAmount = 0f;
    }

    public override void InitSupport(int newRangeLvl, CurrencyCounter newCurrencyCounter, Sprite abilitySprite, Color abilityColor, TurretPartBase turretPartBase)
    {
        base.InitSupport(newRangeLvl, newCurrencyCounter, abilitySprite, abilityColor, turretPartBase);
        supportIcon.sprite = abilitySprite;
        supportIcon.color = abilityColor;
        abilityFillImage.color = abilityColor;

        this.turretPartBase = turretPartBase;

        UpdateNextUpgradeDescriptionText();
    }

    protected override void DoUpgradeSupport()
    {
        UpdateNextUpgradeDescriptionText();
    }

    protected override void UpdateSupportBar()
    {
        fillBars[0].fillAmount = (float)supportLvl * supportFillBarCoef;

        // Update UI
        if (IsCardUpgradedToMax(currentBuildingLevel) || IsStatMaxed(supportLvl))
        {
            EmptyStatBar(abilityBarToCurrencyCost, abilityButtonImage, abilityBackFillImage, (float)supportLvl * turretFillBarCoef);
        }
        else if (!HasEnoughCurrencyToLevelUp())
        {
            ResetStatBarColor(abilityBarToCurrencyCost, abilityButtonImage);
        }
        PlayAnimationIconPunch(supportIcon.transform);

        if (IsStatMaxed(supportLvl)) DisableButton(abilityButton, abilityButtonImage);
        if (IsCardUpgradedToMax(currentBuildingLevel)) DisableButtons();
    }

    protected override void DisableButtons()
    {
        DisableButton(abilityButton, abilityButtonImage);
    }


    protected override void CheckHoveredButtonsCanNowUpgrade()
    {
        if (isAbilityButtonHovered && CanUpgrade(supportLvl)) SetBarAndButtonHighlighted(abilityBarToCurrencyCost, abilityButton.image);
    }


    // Animations
    protected override void PlayOpenAnimation()
    {
        if (closeAnimationCoroutine != null)
        {
            StopCoroutine(closeAnimationCoroutine);
        }

        openAnimationCoroutine = StartCoroutine(OpenAnimation());
    }
    private IEnumerator OpenAnimation()
    {
        barImage.DOComplete();
        backgroundImage.DOComplete();
        cgLvlText.DOComplete();
        cgCostText.DOComplete();
        cgAbilityStat.DOComplete();

        barImage.fillAmount = 0f;
        backgroundImage.fillAmount = 0f;
        cgLvlText.alpha = 0f;
        cgCostText.alpha = 0f;
        cgAbilityStat.alpha = 0f;
        cgNextUpgradeDescription.alpha = 0f;


        float t1 = 0.1f;
        float t3 = 0.3f;
        bool isCardUpgradedToMax = IsCardUpgradedToMax(currentBuildingLevel);

        barImage.DOFillAmount(1f, t1);
        backgroundImage.DOFillAmount(1f, t3);
        GameAudioManager.GetInstance().PlayCardInfoMoveShown();
        yield return new WaitForSeconds(t1);

        cgLvlText.DOFade(1f, t1);
        yield return new WaitForSeconds(t1);

        cgAbilityStat.DOFade(1f, t1);
        yield return new WaitForSeconds(t1);
        if (!IsStatMaxed(attackLvl) && !isCardUpgradedToMax) abilityButtonImage.transform.DOPunchScale(Vector3.one * 0.2f, t1, 8);


        cgCostText.DOFade(1f, t1);
        yield return new WaitForSeconds(t1);

        cgNextUpgradeDescription.DOFade(1f, t1);
        yield return new WaitForSeconds(t1);

        openAnimationCoroutine = null;

        ButtonFadeIn(abilityButton);

    }


    protected override void PlayCloseAnimation()
    {
        if (openAnimationCoroutine != null)
        {
            StopCoroutine(openAnimationCoroutine);
        }

        if (isAbilityButtonHovered)
        {
            EmptyAbilityBar();
        }

        closeAnimationCoroutine = StartCoroutine(CloseAnimation());
    }
    private IEnumerator CloseAnimation()
    {
        barImage.DOComplete();
        backgroundImage.DOComplete();
        cgLvlText.DOComplete();
        cgCostText.DOComplete();
        cgAbilityStat.DOComplete();


        barImage.fillAmount = 1f;
        backgroundImage.fillAmount = 1f;
        cgLvlText.alpha = 1f;
        cgCostText.alpha = 1f;
        cgAbilityStat.alpha = 1f;
        cgNextUpgradeDescription.alpha = 1f;



        float t1 = 0.075f;
        float t2 = 0.15f;
        float t3 = 0.225f;


        cgCostText.DOFade(0f, t1);
        yield return new WaitForSeconds(t1);

        cgAbilityStat.DOFade(0f, t1);
        yield return new WaitForSeconds(t1);

        cgNextUpgradeDescription.DOFade(0f, t1);
        yield return new WaitForSeconds(t1);

        backgroundImage.DOFillAmount(0f, t3);
        yield return new WaitForSeconds(t1);

        cgLvlText.DOFade(0f, t1);
        barImage.DOFillAmount(0f, t1);
        yield return new WaitForSeconds(t2);

        //yield return new WaitUntil(() => barImage.fillAmount > 0.001f);
        closeAnimationCoroutine = null;
        newUiParent.gameObject.SetActive(false);

        StopButtonFade(abilityButton, false, false);
    }



    public void FillAbilityBar()
    {
        bool highlight = CanUpgrade(supportLvl);

        StopButtonFade(abilityButton, false, highlight);

        if (!abilityButton.interactable) return;
        if (IsCardUpgradedToMax(currentBuildingLevel) || IsStatMaxed(supportLvl)) return;

        FillStatBar(abilityBarToCurrencyCost, abilityButtonImage, abilityBackFillImage, (float)(supportLvl + 1) * supportFillBarCoef, highlight);

        isAbilityButtonHovered = true;
    }

    public void EmptyAbilityBar()
    {
        if (!abilityButton.interactable) return;
        if (IsCardUpgradedToMax(currentBuildingLevel) || IsStatMaxed(supportLvl)) return;

        EmptyStatBar(abilityBarToCurrencyCost, abilityButtonImage, abilityBackFillImage, (float)supportLvl * supportFillBarCoef);

        ButtonFadeIn(abilityButton);

        isAbilityButtonHovered = false;
    }

    protected override void OnCanNotUpgradeSupport()
    {
        ButtonPressedErrorFadeInOut(abilityButton, abilityBarToCurrencyCost);
    }


    private void UpdateNextUpgradeDescriptionText()
    {
        nextUpgradeDescriptionText.text = turretPartBase.GetUpgradeDescriptionByLevel(currentBuildingLevel + 1);
        if (currentBuildingLevel >= 3)
        {
            nextUpgradeText.color = disabledColor;
        }
    }

    protected override bool IsBuildingUpgradeAvailable()
    {
        return base.IsBuildingUpgradeAvailable() || (StatsLevelBellowLimit(true) && !IsCardUpgradedToMax(currentBuildingLevel));
    }

    protected override bool IsBuildingUpgradeNotAvailable()
    {
        return base.IsBuildingUpgradeNotAvailable() || (!StatsLevelBellowLimit(true) && IsCardUpgradedToMax(currentBuildingLevel));
    }

}
