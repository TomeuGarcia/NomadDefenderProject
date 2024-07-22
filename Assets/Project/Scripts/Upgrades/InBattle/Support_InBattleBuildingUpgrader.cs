using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class Support_InBattleBuildingUpgrader : InBattleBuildingUpgrader
{
    [SerializeField] InBattleUpgradeStat _abilityUpgradeStat;
    [SerializeField] private Image supportIcon;

    [Header("Upgrade Descriptions")]
    [SerializeField] private TextMeshProUGUI nextUpgradeText;
    [SerializeField] private TextMeshProUGUI nextUpgradeDescriptionText;
    [SerializeField] private CanvasGroup cgNextUpgradeDescription;
    private bool isAbilityButtonHovered = false;

    private TurretPartBase turretPartBase;


    protected override void AwakeInit()
    {
        base.AwakeInit();

        _abilityUpgradeStat.Init(this, OnUpgradeSupportButtonClicked, OnSupportButtonHovered, OnSupportButtonUnhovered);
    }

    public override void InitSupport(int newRangeLvl, CurrencyCounter newCurrencyCounter, Sprite abilitySprite, Color abilityColor, TurretPartBase turretPartBase)
    {
        base.InitSupport(newRangeLvl, newCurrencyCounter, abilitySprite, abilityColor, turretPartBase);
        supportIcon.sprite = abilitySprite;
        supportIcon.color = abilityColor;

        this.turretPartBase = turretPartBase;

        UpdateNextUpgradeDescriptionText();

        maxLevels = maxUpgradeCount;
        UpdateSupportBar();
    }

    private void DoUpgradeSupport()
    {
        UpdateNextUpgradeDescriptionText();
    }

    protected override void UpdateSupportBar()
    {
        bool isCardUpgradedToMax = IsCardUpgradedToMax(currentBuildingLevel);
        _abilityUpgradeStat.UpdateView("TODO", isCardUpgradedToMax, IsStatMaxed(rangeLvl));

        if (isCardUpgradedToMax)
        {
            DisableButtons();
        }
    }

    protected override void DisableButtons()
    {
        _abilityUpgradeStat.DisableButton();
    }


    protected override void CheckHoveredButtonsCanNowUpgrade()
    {
        if (_abilityUpgradeStat.IsButtonHovered && CanUpgrade(attackLvl)) _abilityUpgradeStat.SetBarAndButtonHighlighted();
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

        barImage.fillAmount = 0f;
        backgroundImage.fillAmount = 0f;
        cgLvlText.alpha = 0f;
        cgCostText.alpha = 0f;
        cgNextUpgradeDescription.alpha = 0f;

        _abilityUpgradeStat.SetupOpenAnimation();


        float t1 = 0.1f;
        float t3 = 0.3f;
        bool isCardUpgradedToMax = IsCardUpgradedToMax(currentBuildingLevel);

        barImage.DOFillAmount(1f, t1);
        backgroundImage.DOFillAmount(1f, t3);
        GameAudioManager.GetInstance().PlayCardInfoMoveShown();
        yield return new WaitForSeconds(t1);

        cgLvlText.DOFade(1f, t1);
        yield return new WaitForSeconds(t1);

        yield return _abilityUpgradeStat.PlayOpenAnimation(t1, !IsStatMaxed(attackLvl) && !isCardUpgradedToMax, t1);


        cgCostText.DOFade(1f, t1);
        yield return new WaitForSeconds(t1);

        cgNextUpgradeDescription.DOFade(1f, t1);
        yield return new WaitForSeconds(t1);

        openAnimationCoroutine = null;

        _abilityUpgradeStat.ButtonFadeIn();
    }


    protected override void PlayCloseAnimation()
    {
        if (openAnimationCoroutine != null)
        {
            StopCoroutine(openAnimationCoroutine);
        }

        if (isAbilityButtonHovered)
        {
            OnSupportButtonUnhovered();
        }

        closeAnimationCoroutine = StartCoroutine(CloseAnimation());
    }
    private IEnumerator CloseAnimation()
    {
        barImage.DOComplete();
        backgroundImage.DOComplete();
        cgLvlText.DOComplete();
        cgCostText.DOComplete();


        barImage.fillAmount = 1f;
        backgroundImage.fillAmount = 1f;
        cgLvlText.alpha = 1f;
        cgCostText.alpha = 1f;
        cgNextUpgradeDescription.alpha = 1f;

        _abilityUpgradeStat.SetupCloseAnimation();

        float t1 = 0.075f;
        float t2 = 0.15f;
        float t3 = 0.225f;


        cgCostText.DOFade(0f, t1);
        yield return new WaitForSeconds(t1);

        yield return _abilityUpgradeStat.PlayCloseAnimation(t1);

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

        _abilityUpgradeStat.StopButtonFade(false, false);
    }



    public void OnSupportButtonHovered()
    {
        bool highlight = CanUpgrade(supportLvl);
        //float fillValue = (float)(supportLvl + 1) * supportFillBarCoef
        _abilityUpgradeStat.OnButtonHovered(highlight, IsCardUpgradedToMax(currentBuildingLevel), IsStatMaxed(supportLvl), "TODO n");

        StopAllButtonsFade(false, highlight);
    }

    public void OnSupportButtonUnhovered()
    {
        // (float)supportLvl * supportFillBarCoef
        _abilityUpgradeStat.OnButtonUnhovered(IsCardUpgradedToMax(currentBuildingLevel), IsStatMaxed(supportLvl), "TODO");

        _abilityUpgradeStat.ButtonFadeIn();
    }

    private void StopAllButtonsFade(bool abilityFadeOut, bool highlight)
    {
        abilityFadeOut = abilityFadeOut && _abilityUpgradeStat.IsButtonInteractable;
        bool abilityHighlight = highlight && !abilityFadeOut && _abilityUpgradeStat.IsButtonInteractable;

        _abilityUpgradeStat.StopButtonFade(abilityFadeOut, abilityHighlight);
    }

    protected override void OnCanNotUpgradeSupport()
    {
        _abilityUpgradeStat.ButtonPressedErrorFadeInOut();
    }


    private void UpdateNextUpgradeDescriptionText()
    {
        nextUpgradeDescriptionText.text = turretPartBase.GetUpgradeDescriptionByLevel(currentBuildingLevel + 1);
        if (currentBuildingLevel >= 3)
        {
            nextUpgradeText.color = disabledColor;
        }
    }



    private void OnUpgradeSupportButtonClicked()
    {
        if (TryUpgradeStat(ref supportLvl, TurretUpgradeType.SUPPORT))
        {
            UpdateSupportBar();
            DoUpgradeSupport();
        }
        else
        {
            OnCanNotUpgradeSupport();
        }
    }

    /*
    private void OnSupportButtonHovered()
    {
        bool highlight = CanUpgrade(attackLvl);
        //float fillValue = (float)(attackLvl + 1) * turretFillBarCoef;
        _damageUpgradeStat.OnButtonHovered(highlight, IsCardUpgradedToMax(currentBuildingLevel), IsStatMaxed(attackLvl), NextDamageStatValueText);

        StopAllButtonsFade(false, true, true, highlight);
    }
    */
}
