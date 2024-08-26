using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class Support_InBattleBuildingUpgrader : InBattleBuildingUpgrader
{
    [SerializeField] InBattleUpgradeStat _abilityUpgradeStat;
    [SerializeField] private StatUpgradeButton _supportUpgradeButton;

    [SerializeField] private Image supportIcon;

    [Header("Upgrade Descriptions")]
    [SerializeField] private TextMeshProUGUI nextUpgradeDescriptionText;
    [SerializeField] private CanvasGroup cgNextUpgradeDescription;
    private bool isAbilityButtonHovered = false;

    private SupportCardData _supportCardData;
    private SupportBuilding _supportBuilding;

    protected override int CurrentBuildingLevel { get => _currentSupportLevel; }
    private int _currentSupportLevel;

    protected override void AwakeInit()
    {
        base.AwakeInit();

        _supportUpgradeButton.Init(OnUpgradeSupportButtonClicked, OnSupportButtonHovered, OnSupportButtonUnhovered);

        _abilityUpgradeStat.Init(this);
    }

    public override void InitSupport(SupportBuilding supportBuilding,
        IBuildingUpgradesController buildingUpgradesController,
        CurrencyCounter newCurrencyCounter, Sprite abilitySprite, Color abilityColor, SupportCardData supportCardData)
    {
        _supportBuilding = supportBuilding;
        maxLevels = maxUpgradeCount;
        _currentSupportLevel = 0;

        base.InitSupport(supportBuilding, buildingUpgradesController, 
            newCurrencyCounter, abilitySprite, abilityColor, supportCardData);
        supportIcon.sprite = abilitySprite;
        supportIcon.color = abilityColor;

        _supportCardData = supportCardData;


        UpdateNextUpgradeDescriptionText();

        UpdateSupportBar();
    }


    protected override void UpdateSupportBar()
    {
        bool isCardUpgradedToMax = IsCardUpgradedToMax(CurrentBuildingLevel);
        _abilityUpgradeStat.UpdateView("TODO", isCardUpgradedToMax);

        if (isCardUpgradedToMax)
        {
            DisableButtons();
        }
    }

    protected override void DisableButtons()
    {
        _supportUpgradeButton.DisableButton();        
    }


    protected override void CheckHoveredButtonsCanNowUpgrade()
    {
        if (_supportUpgradeButton.IsButtonHovered) _supportUpgradeButton.SetHighlighted();
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
        _supportUpgradeButton.SetupOpenAnimation();


        float t1 = 0.1f;
        float t3 = 0.3f;
        bool isCardUpgradedToMax = IsCardUpgradedToMax(CurrentBuildingLevel);

        barImage.DOFillAmount(1f, t1);
        backgroundImage.DOFillAmount(1f, t3);
        GameAudioManager.GetInstance().PlayCardInfoMoveShown();
        yield return new WaitForSeconds(t1);

        cgLvlText.DOFade(1f, t1);
        yield return new WaitForSeconds(t1);

        yield return _supportUpgradeButton.PlayOpenAnimation(t1, !isCardUpgradedToMax, t1);
        yield return _abilityUpgradeStat.PlayOpenAnimation(t1);


        cgCostText.DOFade(1f, t1);
        yield return new WaitForSeconds(t1);

        cgNextUpgradeDescription.DOFade(1f, t1);
        yield return new WaitForSeconds(t1);

        openAnimationCoroutine = null;

        _supportUpgradeButton.ButtonFadeIn();
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
        _supportUpgradeButton.SetupCloseAnimation();

        float t1 = 0.075f;
        float t2 = 0.15f;
        float t3 = 0.225f;


        cgCostText.DOFade(0f, t1);
        yield return new WaitForSeconds(t1);

        yield return _supportUpgradeButton.PlayCloseAnimation(t1);
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

        _supportUpgradeButton.StopButtonFade(false, false);
    }



    public void OnSupportButtonHovered()
    {
        bool highlight = CanUpgrade();
        //float fillValue = (float)(supportLvl + 1) * supportFillBarCoef
        //_supportUpgradeButton.OnButtonHovered();
    }

    public void OnSupportButtonUnhovered()
    {
        // (float)supportLvl * supportFillBarCoef
        //_supportUpgradeButton.OnButtonUnhovered();
        _supportUpgradeButton.ButtonFadeIn();
    }

    private void StopAllButtonsFade(bool abilityFadeOut, bool highlight)
    {
        abilityFadeOut = abilityFadeOut && _supportUpgradeButton.IsButtonInteractable;
        bool abilityHighlight = highlight && !abilityFadeOut && _supportUpgradeButton.IsButtonInteractable;

        _supportUpgradeButton.StopButtonFade(abilityFadeOut, abilityHighlight);
    }

    protected override void OnCanNotUpgradeSupport()
    {
        _supportUpgradeButton.ButtonPressedErrorFadeInOut();
    }


    private void UpdateNextUpgradeDescriptionText()
    {
        nextUpgradeDescriptionText.text = _supportCardData.GetUpgradeDescriptionByLevel(CurrentBuildingLevel + 1);
    }



    private void OnUpgradeSupportButtonClicked()
    {
        if (CanUpgrade())
        {
            SpendUpgradeCost();
            UpgradeAllSupportStats();
            UpdateNextUpgradeDescriptionText();
            UpdateSupportBar();
        }
        else
        {
            PlayNegativeAnimationTextCostPunch();
        }
    }

    private void UpgradeAllSupportStats()
    {
        //_buildingUpgradesController.IncrementUpgradeLevel(); // DON'T UPGARDE STATS EVERY TIME!
        ++_currentSupportLevel;
        NextLevel();
        _supportBuilding.ApplyUpgrade(CurrentBuildingLevel);

        CheckStopParticlesCanUpgrade();
        PlayPositiveAnimationTextCostPunch();
        InvokeOnUpgrade(TurretUpgradeType.SUPPORT);
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
