using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Turret_InBattleBuildingUpgrader : InBattleBuildingUpgrader
{
    [Header("Attack")]
    [SerializeField] private CanvasGroup cgAttackStat;
    [SerializeField] private Image attackIcon;
    [SerializeField] private Button attackButton;
    [SerializeField] private Image attackButtonImage;
    [SerializeField] private Image attackBackFillImage;
    [SerializeField] private Image attackBarToCurrencyCost;
    [Header("Fire Rate")]
    [SerializeField] private CanvasGroup cgFireRateStat;
    [SerializeField] private Image fireRateIcon; 
    [SerializeField] private Button fireRateButton;
    [SerializeField] private Image fireRateButtonImage;
    [SerializeField] private Image fireRateBackFillImage;
    [SerializeField] private Image fireBarToCurrencyCost;
    [Header("Range")]
    [SerializeField] private CanvasGroup cgRangeStat;
    [SerializeField] private Image rangeIcon;
    [SerializeField] private Button rangeButton;
    [SerializeField] private Image rangeButtonImage;
    [SerializeField] private Image rangeBackFillImage;
    [SerializeField] private Image rangeBarToCurrencyCost;


    [Header("QUICK DISPLAY UI EXTRAS")]
    [SerializeField] private GameObject quickHoverBasePassiveImageHolder;
    [SerializeField] private Image quickHoverBasePassiveImage;


    protected override void AwakeInit()
    {
        base.AwakeInit();
        attackBarToCurrencyCost.fillAmount = 0f;
        fireBarToCurrencyCost.fillAmount = 0f;
        rangeBarToCurrencyCost.fillAmount = 0f;
    }


    public override void InitTurret(int newAttackLvl, int newCadenceLvl, int newRangeLvl, CurrencyCounter newCurrencyCounter,
        bool hasPassiveAbility, Sprite basePassiveSprite, Color basePassiveColor)
    {
        base.InitTurret(newAttackLvl, newCadenceLvl, newRangeLvl, newCurrencyCounter, hasPassiveAbility, basePassiveSprite, basePassiveColor);

        if (hasPassiveAbility)
        {
            quickHoverBasePassiveImageHolder.gameObject.SetActive(true);
            quickHoverBasePassiveImage.sprite = basePassiveSprite;
            quickHoverBasePassiveImage.color = basePassiveColor;
        }
        else
        {
            quickHoverBasePassiveImageHolder.gameObject.SetActive(false);
        }

        if (IsStatMaxed(newAttackLvl)) DisableButton(attackButton, attackButtonImage);
        if (IsStatMaxed(newCadenceLvl)) DisableButton(fireRateButton, fireRateButtonImage);
        if (IsStatMaxed(newRangeLvl)) DisableButton(rangeButton, rangeButtonImage);
    }



    protected override void UpdateAttackBar()
    {
        fillBars[(int)TurretUpgradeType.ATTACK].fillAmount = (float)attackLvl * turretFillBarCoef;

        attackIcon.transform.DOPunchScale(Vector3.one * 0.5f, 0.5f, 7);

        // Update UI
        if (!CanUpgrade(attackLvl))
        {
            EmptyStatBar(attackBarToCurrencyCost, attackButtonImage, attackBackFillImage, (float)attackLvl * turretFillBarCoef);
            PlayAnimationIconPunch(attackIcon.transform);
        }

        if (IsStatMaxed(attackLvl)) DisableButton(attackButton, attackButtonImage);
        if (IsCardUpgradedToMax(currentLevel)) DisableButtons();
    }
    protected override void UpdateCadenceBar()
    {
        fillBars[(int)TurretUpgradeType.CADENCE].fillAmount = (float)cadenceLvl * turretFillBarCoef;

        // Update UI
        if (!CanUpgrade(cadenceLvl))
        {
            EmptyStatBar(fireBarToCurrencyCost, fireRateButtonImage, fireRateBackFillImage, (float)cadenceLvl * turretFillBarCoef);
            PlayAnimationIconPunch(fireRateIcon.transform);
        }

        if (IsStatMaxed(cadenceLvl)) DisableButton(fireRateButton, fireRateButtonImage);
        if (IsCardUpgradedToMax(currentLevel)) DisableButtons();
    }
    protected override void UpdateRangeBar()
    {
        fillBars[(int)TurretUpgradeType.RANGE].fillAmount = (float)rangeLvl * turretFillBarCoef;

        // Update UI
        if (!CanUpgrade(rangeLvl))
        {
            EmptyStatBar(rangeBarToCurrencyCost, rangeButtonImage, rangeBackFillImage, (float)rangeLvl * turretFillBarCoef);
            PlayAnimationIconPunch(rangeIcon.transform);
        }

        if (IsStatMaxed(rangeLvl)) DisableButton(rangeButton, rangeButtonImage);
        if (IsCardUpgradedToMax(currentLevel)) DisableButtons();
    }


    protected override void DisableButtons()
    {
        DisableButton(attackButton, attackButtonImage);
        DisableButton(fireRateButton, fireRateButtonImage);
        DisableButton(rangeButton, rangeButtonImage);
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
        cgAttackStat.DOComplete();
        cgFireRateStat.DOComplete();
        cgRangeStat.DOComplete();

        barImage.fillAmount = 0f;
        backgroundImage.fillAmount = 0f;
        cgLvlText.alpha = 0f;
        cgCostText.alpha = 0f;
        cgAttackStat.alpha = 0f;
        cgFireRateStat.alpha = 0f;
        cgRangeStat.alpha = 0f;


        float t1 = 0.075f;
        float t3 = t1 * 3;
        bool isCardUpgradedToMax = IsCardUpgradedToMax(currentLevel);

        barImage.DOFillAmount(1f, t1);
        backgroundImage.DOFillAmount(1f, t3);
        GameAudioManager.GetInstance().PlayCardInfoMoveShown();
        yield return new WaitForSeconds(t1);

        cgLvlText.DOFade(1f, t1);
        yield return new WaitForSeconds(t1);

        cgAttackStat.DOFade(1f, t1);
        yield return new WaitForSeconds(t1);
        if (!IsStatMaxed(attackLvl) && !isCardUpgradedToMax) attackButtonImage.transform.DOPunchScale(Vector3.one * 0.2f, t3, 8);

        cgFireRateStat.DOFade(1f, t1);
        yield return new WaitForSeconds(t1);
        if (!IsStatMaxed(cadenceLvl) && !isCardUpgradedToMax) fireRateButtonImage.transform.DOPunchScale(Vector3.one * 0.2f, t3, 8);

        cgRangeStat.DOFade(1f, t1);
        yield return new WaitForSeconds(t1);
        if (!IsStatMaxed(rangeLvl) && !isCardUpgradedToMax) rangeButtonImage.transform.DOPunchScale(Vector3.one * 0.2f, t3, 8);

        cgCostText.DOFade(1f, t1);
        yield return new WaitForSeconds(t1);        

        ButtonFadeIn(attackButton);
        ButtonFadeIn(fireRateButton);
        ButtonFadeIn(rangeButton);

        openAnimationCoroutine = null;
    }


    protected override void PlayCloseAnimation()
    {
        if (openAnimationCoroutine != null)
        {
            StopCoroutine(openAnimationCoroutine);
        }

        closeAnimationCoroutine = StartCoroutine(CloseAnimation());
    }
    private IEnumerator CloseAnimation()
    {
        barImage.DOComplete();
        backgroundImage.DOComplete();
        cgLvlText.DOComplete();
        cgCostText.DOComplete();
        cgAttackStat.DOComplete();
        cgFireRateStat.DOComplete();
        cgRangeStat.DOComplete();

        barImage.fillAmount = 1f;
        backgroundImage.fillAmount = 1f;
        cgLvlText.alpha = 1f;
        cgCostText.alpha = 1f;
        cgAttackStat.alpha = 1f;
        cgFireRateStat.alpha = 1f;
        cgRangeStat.alpha = 1f;


        float t1 = 0.075f;
        float t2 = 0.15f;
        float t3 = 0.225f;


        cgCostText.DOFade(0f, t1);
        yield return new WaitForSeconds(t1);

        cgRangeStat.DOFade(0f, t1);
        yield return new WaitForSeconds(t1);

        cgFireRateStat.DOFade(0f, t1);
        yield return new WaitForSeconds(t1);

        cgAttackStat.DOFade(0f, t1);
        backgroundImage.DOFillAmount(0f, t3);
        yield return new WaitForSeconds(t1);

        cgLvlText.DOFade(0f, t1);
        barImage.DOFillAmount(0f, t1);
        yield return new WaitForSeconds(t2);

        //yield return new WaitUntil(() => barImage.fillAmount > 0.001f);
        closeAnimationCoroutine = null;
        newUiParent.gameObject.SetActive(false);

        StopAllButtonsFade(false, false, false, false);
    }



    public void FillAttackBar() // Attack button hovered
    {
        bool highlight = CanUpgrade(attackLvl);

        StopAllButtonsFade(false, true, true, highlight);

        if (!attackButton.interactable) return;
        if (IsCardUpgradedToMax(currentLevel) || IsStatMaxed(attackLvl)) return;

        FillStatBar(attackBarToCurrencyCost, attackButtonImage, attackBackFillImage, (float)(attackLvl + 1) * turretFillBarCoef, highlight);
    }
    public void FillFireRateBar() // FireRate button hovered
    {
        bool highlight = CanUpgrade(cadenceLvl);

        StopAllButtonsFade(true, false, true, highlight);

        if (!fireRateButton.interactable) return;
        if (IsCardUpgradedToMax(currentLevel) || IsStatMaxed(cadenceLvl)) return;

        FillStatBar(fireBarToCurrencyCost, fireRateButtonImage, fireRateBackFillImage, (float)(cadenceLvl + 1) * turretFillBarCoef, highlight);
    }
    public void FillRangeBar() // Range button hovered
    {
        bool highlight = CanUpgrade(rangeLvl);

        StopAllButtonsFade(true, true, false, highlight);

        if (!rangeButton.interactable) return;
        if (IsCardUpgradedToMax(currentLevel) || IsStatMaxed(rangeLvl)) return;

        FillStatBar(rangeBarToCurrencyCost, rangeButtonImage, rangeBackFillImage, (float)(rangeLvl + 1) * turretFillBarCoef, highlight);
    }


    public void EmptyAttackBar() // Attack button UN-hovered
    {
        if (!attackButton.interactable) return;
        if (IsCardUpgradedToMax(currentLevel) || IsStatMaxed(attackLvl)) return;

        EmptyStatBar(attackBarToCurrencyCost, attackButtonImage, attackBackFillImage, (float)attackLvl * turretFillBarCoef);

        AllButtonsFadeIn();
    }
    public void EmptyFireRateBar() // FireRate button UN-hovered
    {
        if (!fireRateButton.interactable) return;
        if (IsCardUpgradedToMax(currentLevel) || IsStatMaxed(cadenceLvl)) return;

        EmptyStatBar(fireBarToCurrencyCost, fireRateButtonImage, fireRateBackFillImage, (float)cadenceLvl * turretFillBarCoef);

        AllButtonsFadeIn();
    }
    public void EmptyRangeBar() // Range button UN-hovered
    {
        if (!rangeButton.interactable) return;
        if (IsCardUpgradedToMax(currentLevel) || IsStatMaxed(rangeLvl)) return;

        EmptyStatBar(rangeBarToCurrencyCost, rangeButtonImage, rangeBackFillImage, (float)rangeLvl * turretFillBarCoef);

        AllButtonsFadeIn();
    }



    private void AllButtonsFadeIn()
    {
        ButtonFadeIn(attackButton);
        ButtonFadeIn(fireRateButton);
        ButtonFadeIn(rangeButton);
    }

    private void StopAllButtonsFade(bool attackFadeOut, bool fireRateFadeOut, bool rangeFadeOut, bool highlight)
    {
        attackFadeOut = attackFadeOut && attackButton.interactable;
        fireRateFadeOut = fireRateFadeOut && fireRateButton.interactable;
        rangeFadeOut = rangeFadeOut && rangeButton.interactable;

        bool attackHighlight = highlight && !attackFadeOut && attackButton.interactable;
        bool fireRateHighlight = highlight && !fireRateFadeOut && fireRateButton.interactable;
        bool rangeHighlight = highlight && !rangeFadeOut && rangeButton.interactable;

        StopButtonFade(attackButton, attackFadeOut, attackHighlight);
        StopButtonFade(fireRateButton, fireRateFadeOut, fireRateHighlight);
        StopButtonFade(rangeButton, rangeFadeOut, rangeHighlight);
    }

    protected override void OnCanNotUpgradeAttack() 
    {
        ButtonPressedErrorFadeInOut(attackButton, attackBarToCurrencyCost);
    }
    protected override void OnCanNotUpgradeFireRate() 
    {
        ButtonPressedErrorFadeInOut(fireRateButton, fireBarToCurrencyCost);
    }
    protected override void OnCanNotUpgradeRange() 
    {
        ButtonPressedErrorFadeInOut(rangeButton, rangeBarToCurrencyCost);
    }


}
