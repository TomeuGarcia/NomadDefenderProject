using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Turret_InBattleBuildingUpgrader : InBattleBuildingUpgrader
{
    [SerializeField] private InBattleUpgradeStat _damageUpgradeStat;
    [SerializeField] private InBattleUpgradeStat _fireRateUpgradeStat;
    [SerializeField] private InBattleUpgradeStat _rangeUpgradeStat;


    [Header("QUICK DISPLAY UI EXTRAS")]
    [SerializeField] private GameObject quickHoverBasePassiveImageHolder;
    [SerializeField] private Image quickHoverBasePassiveImage;

    private TurretBuilding _turret;
    private string DamageStatValueText => _turret.TurretCardParts.turretPartBody.GetDamageByLevelText(attackLvl);
    private string NextDamageStatValueText => IsStatMaxed(attackLvl) ? "" : _turret.TurretCardParts.turretPartBody.GetDamageByLevelText(attackLvl+1);
    private string FireRateStatValueText => _turret.TurretCardParts.turretPartBody.GetShotsPerSecondByLevelText(cadenceLvl);
    private string NextFireRateStatValueText => IsStatMaxed(cadenceLvl) ? "" : _turret.TurretCardParts.turretPartBody.GetShotsPerSecondByLevelText(cadenceLvl+1);
    private string RangeStatValueText => _turret.TurretCardParts.turretPartBase.GetRangeByLevelText(rangeLvl);
    private string NextRangeStatValueText => IsStatMaxed(rangeLvl) ? "" : _turret.TurretCardParts.turretPartBase.GetRangeByLevelText(rangeLvl+1);

    protected override void AwakeInit()
    {
        base.AwakeInit();

        _damageUpgradeStat.Init(this, OnUpgradeDamageButtonClicked, OnDamageButtonHovered, OnDamageButtonUnhovered);
        _fireRateUpgradeStat.Init(this, OnUpgradeFireRateButtonClicked, OnFireRateButtonHovered, OnFireRateButtonUnhovered);
        _rangeUpgradeStat.Init(this, OnUpgradeRangeButtonClicked, OnRangeButtonHovered, OnRangeButtonUnhovered);
    }


    public override void InitTurret(TurretBuilding turret, CurrencyCounter newCurrencyCounter,
        bool hasPassiveAbility, Sprite basePassiveSprite, Color basePassiveColor)
    {
        base.InitTurret(turret, newCurrencyCounter, hasPassiveAbility, basePassiveSprite, basePassiveColor);

        _turret = turret;
        maxLevels = _turret.CardLevel;

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

        UpdateAttackBar();
        UpdateCadenceBar();
        UpdateRangeBar();
    }



    protected override void UpdateAttackBar()
    {
        bool isCardUpgradedToMax = IsCardUpgradedToMax(currentBuildingLevel);
        _damageUpgradeStat.UpdateView(DamageStatValueText, isCardUpgradedToMax, IsStatMaxed(attackLvl));

        if (isCardUpgradedToMax)
        {
            DisableButtons();
        }
    }

    protected override void UpdateCadenceBar()
    {
        bool isCardUpgradedToMax = IsCardUpgradedToMax(currentBuildingLevel);
        _fireRateUpgradeStat.UpdateView(FireRateStatValueText, isCardUpgradedToMax, IsStatMaxed(cadenceLvl));

        if (isCardUpgradedToMax)
        {
            DisableButtons();
        }
    }
    protected override void UpdateRangeBar()
    {
        bool isCardUpgradedToMax = IsCardUpgradedToMax(currentBuildingLevel);
        _rangeUpgradeStat.UpdateView(RangeStatValueText, isCardUpgradedToMax, IsStatMaxed(rangeLvl));

        if (isCardUpgradedToMax)
        {
            DisableButtons();
        }
    }


    protected override void DisableButtons()
    {
        _damageUpgradeStat.DisableButton();
        _fireRateUpgradeStat.DisableButton();
        _rangeUpgradeStat.DisableButton();
    }


    protected override void CheckHoveredButtonsCanNowUpgrade()
    {
        if (_damageUpgradeStat.IsButtonHovered && CanUpgrade(attackLvl)) _damageUpgradeStat.SetBarAndButtonHighlighted();
        if (_fireRateUpgradeStat.IsButtonHovered && CanUpgrade(cadenceLvl)) _fireRateUpgradeStat.SetBarAndButtonHighlighted();
        if (_rangeUpgradeStat.IsButtonHovered && CanUpgrade(rangeLvl)) _rangeUpgradeStat.SetBarAndButtonHighlighted();
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

        _damageUpgradeStat.SetupOpenAnimation();
        _fireRateUpgradeStat.SetupOpenAnimation();
        _rangeUpgradeStat.SetupOpenAnimation();

        float t1 = 0.075f;
        float t3 = t1 * 3;
        bool isCardUpgradedToMax = IsCardUpgradedToMax(currentBuildingLevel);

        barImage.DOFillAmount(1f, t1);
        backgroundImage.DOFillAmount(1f, t3);
        GameAudioManager.GetInstance().PlayCardInfoMoveShown();
        yield return new WaitForSeconds(t1);

        cgLvlText.DOFade(1f, t1);
        yield return new WaitForSeconds(t1);

        yield return _damageUpgradeStat.PlayOpenAnimation(t1, !IsStatMaxed(attackLvl) && !isCardUpgradedToMax, t3);
        yield return _fireRateUpgradeStat.PlayOpenAnimation(t1, !IsStatMaxed(cadenceLvl) && !isCardUpgradedToMax, t3);
        yield return _rangeUpgradeStat.PlayOpenAnimation(t1, !IsStatMaxed(rangeLvl) && !isCardUpgradedToMax, t3);

        cgCostText.DOFade(1f, t1);
        yield return new WaitForSeconds(t1);

        _damageUpgradeStat.ButtonFadeIn();
        _fireRateUpgradeStat.ButtonFadeIn();
        _rangeUpgradeStat.ButtonFadeIn();

        openAnimationCoroutine = null;
    }


    protected override void PlayCloseAnimation()
    {
        if (openAnimationCoroutine != null)
        {
            StopCoroutine(openAnimationCoroutine);
        }

        if (_damageUpgradeStat.IsButtonHovered) OnDamageButtonUnhovered();
        if (_fireRateUpgradeStat.IsButtonHovered) OnFireRateButtonUnhovered();
        if (_fireRateUpgradeStat.IsButtonHovered) OnRangeButtonUnhovered();

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

        _damageUpgradeStat.SetupCloseAnimation();
        _fireRateUpgradeStat.SetupCloseAnimation();
        _rangeUpgradeStat.SetupCloseAnimation();

        float t1 = 0.075f;
        float t2 = 0.15f;
        float t3 = 0.225f;


        cgCostText.DOFade(0f, t1);
        yield return new WaitForSeconds(t1);

        yield return _damageUpgradeStat.PlayCloseAnimation(t1);

        yield return _fireRateUpgradeStat.PlayCloseAnimation(t1);

        backgroundImage.DOFillAmount(0f, t3);
        yield return _damageUpgradeStat.PlayCloseAnimation(t1);
        
        cgLvlText.DOFade(0f, t1);
        barImage.DOFillAmount(0f, t1);
        yield return new WaitForSeconds(t2);

        //yield return new WaitUntil(() => barImage.fillAmount > 0.001f);
        closeAnimationCoroutine = null;
        newUiParent.gameObject.SetActive(false);

        StopAllButtonsFade(false, false, false, false);
    }



    // DAMAGE
    private void OnDamageButtonHovered()
    {
        bool highlight = CanUpgrade(attackLvl);
        //float fillValue = (float)(attackLvl + 1) * turretFillBarCoef;
        _damageUpgradeStat.OnButtonHovered(highlight, IsCardUpgradedToMax(currentBuildingLevel), IsStatMaxed(attackLvl), NextDamageStatValueText);

        StopAllButtonsFade(false, true, true, highlight);
    }



    private void OnFireRateButtonHovered()
    {
        bool highlight = CanUpgrade(cadenceLvl);

        _fireRateUpgradeStat.OnButtonHovered(highlight, IsCardUpgradedToMax(currentBuildingLevel), IsStatMaxed(cadenceLvl), NextFireRateStatValueText);

        StopAllButtonsFade(true, false, true, highlight);
    }
    private void OnRangeButtonHovered()
    {
        bool highlight = CanUpgrade(rangeLvl);

        _rangeUpgradeStat.OnButtonHovered(highlight, IsCardUpgradedToMax(currentBuildingLevel), IsStatMaxed(rangeLvl), NextRangeStatValueText);

        StopAllButtonsFade(true, true, false, highlight);
    }


    public void OnDamageButtonUnhovered() // Attack button UN-hovered
    {
        //float fillValue = (float)attackLvl * turretFillBarCoef;
        _damageUpgradeStat.OnButtonUnhovered(IsCardUpgradedToMax(currentBuildingLevel), IsStatMaxed(attackLvl), DamageStatValueText);

        AllButtonsFadeIn();
    }
    public void OnFireRateButtonUnhovered() // FireRate button UN-hovered
    {
        // (float)cadenceLvl * turretFillBarCoef
        _fireRateUpgradeStat.OnButtonUnhovered(IsCardUpgradedToMax(currentBuildingLevel), IsStatMaxed(cadenceLvl), FireRateStatValueText);

        AllButtonsFadeIn();
    }
    public void OnRangeButtonUnhovered() // Range button UN-hovered
    {
        // (float)rangeLvl * turretFillBarCoef
        _rangeUpgradeStat.OnButtonUnhovered(IsCardUpgradedToMax(currentBuildingLevel), IsStatMaxed(rangeLvl), RangeStatValueText);

        AllButtonsFadeIn();
    }



    private void AllButtonsFadeIn()
    {
        _damageUpgradeStat.ButtonFadeIn();
        _fireRateUpgradeStat.ButtonFadeIn();
        _rangeUpgradeStat.ButtonFadeIn();
    }

    private void StopAllButtonsFade(bool attackFadeOut, bool fireRateFadeOut, bool rangeFadeOut, bool highlight)
    {
        attackFadeOut = attackFadeOut && _damageUpgradeStat.IsButtonInteractable;
        fireRateFadeOut = fireRateFadeOut && _fireRateUpgradeStat.IsButtonInteractable;
        rangeFadeOut = rangeFadeOut && _rangeUpgradeStat.IsButtonInteractable;

        bool attackHighlight = highlight && !attackFadeOut && _damageUpgradeStat.IsButtonInteractable;
        bool fireRateHighlight = highlight && !fireRateFadeOut && _fireRateUpgradeStat.IsButtonInteractable;
        bool rangeHighlight = highlight && !rangeFadeOut && _rangeUpgradeStat.IsButtonInteractable;

        _damageUpgradeStat.StopButtonFade(attackFadeOut, attackHighlight);
        _fireRateUpgradeStat.StopButtonFade(fireRateFadeOut, fireRateHighlight);
        _rangeUpgradeStat.StopButtonFade(rangeFadeOut, rangeHighlight);
    }

    protected override void OnCanNotUpgradeAttack() 
    {
        _damageUpgradeStat.ButtonPressedErrorFadeInOut();
    }
    protected override void OnCanNotUpgradeFireRate() 
    {
        _fireRateUpgradeStat.ButtonPressedErrorFadeInOut();
    }
    protected override void OnCanNotUpgradeRange() 
    {
        _rangeUpgradeStat.ButtonPressedErrorFadeInOut();
    }




    private void OnUpgradeDamageButtonClicked()
    {
        if (TryUpgradeStat(ref attackLvl, TurretUpgradeType.ATTACK))
        {
            UpdateAttackBar();
        }
        else
        {
            OnCanNotUpgradeAttack();
        }
    }

    private void OnUpgradeFireRateButtonClicked()
    {
        if (TryUpgradeStat(ref cadenceLvl, TurretUpgradeType.CADENCE))
        {
            UpdateCadenceBar();
        }
        else
        {
            OnCanNotUpgradeFireRate();
        }
    }

    private void OnUpgradeRangeButtonClicked()
    {
        if (TryUpgradeStat(ref rangeLvl, TurretUpgradeType.RANGE))
        {
            UpdateRangeBar();
        }
        else
        {
            OnCanNotUpgradeRange();
        }
    }

}
