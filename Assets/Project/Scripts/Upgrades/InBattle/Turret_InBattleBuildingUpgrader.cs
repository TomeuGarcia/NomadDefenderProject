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
    [SerializeField] private StatUpgradeButton _allStatsUpgradeButton;

    [Header("QUICK DISPLAY UI EXTRAS")] 
    [SerializeField] private TurretIconCanvasDisplay[] _iconDisplays;

    private TurretBuilding _turretBuilding;
    private ITurretStatsStateSource _turretStatsState;

    protected override int CurrentBuildingLevel => _buildingUpgradesController.CurrentUpgradeLevel;
    private string DamageStatValueText => _turretStatsState.DamageStatState.GetValueTextByLevel(CurrentBuildingLevel);
    private string NextDamageStatValueText => _turretStatsState.DamageStatState.GetValueTextByLevel(CurrentBuildingLevel + 1);
    private string FireRateStatValueText => _turretStatsState.ShotsPerSecondInvertedStatState.GetValueTextByLevel(CurrentBuildingLevel);
    private string NextFireRateStatValueText => _turretStatsState.ShotsPerSecondInvertedStatState.GetValueTextByLevel(CurrentBuildingLevel + 1);
    private string RangeStatValueText => _turretStatsState.RadiusRangeStatState.GetValueTextByLevel(CurrentBuildingLevel);
    private string NextRangeStatValueText => _turretStatsState.RadiusRangeStatState.GetValueTextByLevel(CurrentBuildingLevel + 1);

    protected override void AwakeInit()
    {
        base.AwakeInit();

        _allStatsUpgradeButton.Init(OnUpgradeAllStatsButtonClicked, OnUpgradeAllStatsButtonHovered, OnUpgradeAllStatsButtonUnhovered);

        _damageUpgradeStat.Init(this);
        _fireRateUpgradeStat.Init(this);
        _rangeUpgradeStat.Init(this);
    }


    public override void InitTurret(TurretBuilding turretBuilding, 
        IBuildingUpgradesController buildingUpgradesController, ITurretStatsStateSource turretStatsState, 
        int numberOfUpgrades, 
        CurrencyCounter newCurrencyCounter, 
        TurretIconCanvasDisplay.ConfigData[] iconsDisplayData)
    {
        base.InitTurret(turretBuilding, buildingUpgradesController, turretStatsState, numberOfUpgrades, newCurrencyCounter, 
            iconsDisplayData);

        _turretBuilding = turretBuilding;
        _turretStatsState = turretStatsState;
        maxLevels = numberOfUpgrades;
        
        UpdateIcons(iconsDisplayData);
        UpdateAllStatsView();
    }

    private void UpdateIcons(TurretIconCanvasDisplay.ConfigData[] iconsDisplayData)
    {
        for (int i = 0; i < iconsDisplayData.Length; ++i)
        {
            _iconDisplays[i].Init(iconsDisplayData[i]);
        }
        for (int i = iconsDisplayData.Length; i < _iconDisplays.Length; ++i)
        {
            _iconDisplays[i].InitHidden();
        }
    }

    protected override void UpdateAllStatsView()
    {
        bool isCardUpgradedToMax = IsCardUpgradedToMax(CurrentBuildingLevel);

        _damageUpgradeStat.UpdateView(DamageStatValueText, isCardUpgradedToMax);
        _fireRateUpgradeStat.UpdateView(FireRateStatValueText, isCardUpgradedToMax);
        _rangeUpgradeStat.UpdateView(RangeStatValueText, isCardUpgradedToMax);

        if (isCardUpgradedToMax)
        {
            DisableButtons();
        }
    }

    protected override void DisableButtons()
    {
        _allStatsUpgradeButton.DisableButton();
    }


    protected override void CheckHoveredButtonsCanNowUpgrade()
    {
        if (_allStatsUpgradeButton.IsButtonHovered) _allStatsUpgradeButton.SetHighlighted();
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
        _allStatsUpgradeButton.SetupOpenAnimation();

        float t1 = 0.075f;
        float t3 = t1 * 3;
        bool isCardUpgradedToMax = IsCardUpgradedToMax(CurrentBuildingLevel);

        barImage.DOFillAmount(1f, t1);
        backgroundImage.DOFillAmount(1f, t3);
        GameAudioManager.GetInstance().PlayCardInfoMoveShown();
        yield return new WaitForSeconds(t1);

        cgLvlText.DOFade(1f, t1);
        yield return new WaitForSeconds(t1);

        yield return _allStatsUpgradeButton.PlayOpenAnimation(t1, !isCardUpgradedToMax, t3);
        yield return _damageUpgradeStat.PlayOpenAnimation(t1);
        yield return _fireRateUpgradeStat.PlayOpenAnimation(t1);
        yield return _rangeUpgradeStat.PlayOpenAnimation(t1);

        cgCostText.DOFade(1f, t1);
        yield return new WaitForSeconds(t1);

        _allStatsUpgradeButton.ButtonFadeIn();

        openAnimationCoroutine = null;
    }


    protected override void PlayCloseAnimation()
    {
        if (openAnimationCoroutine != null)
        {
            StopCoroutine(openAnimationCoroutine);
        }

        if (_allStatsUpgradeButton.IsButtonHovered) _allStatsUpgradeButton.OnButtonUnhovered();

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
        _allStatsUpgradeButton.SetupCloseAnimation();

        float t1 = 0.075f;
        float t2 = 0.15f;
        float t3 = 0.225f;


        cgCostText.DOFade(0f, t1);
        yield return new WaitForSeconds(t1);

        yield return _rangeUpgradeStat.PlayCloseAnimation(t1);
        yield return _fireRateUpgradeStat.PlayCloseAnimation(t1);
        yield return _damageUpgradeStat.PlayCloseAnimation(t1);
        backgroundImage.DOFillAmount(0f, t3);
        yield return _allStatsUpgradeButton.PlayCloseAnimation(t1);
        
        cgLvlText.DOFade(0f, t1);
        barImage.DOFillAmount(0f, t1);
        yield return new WaitForSeconds(t2);

        //yield return new WaitUntil(() => barImage.fillAmount > 0.001f);
        closeAnimationCoroutine = null;
        newUiParent.gameObject.SetActive(false);

        StopAllButtonsFade(false);
    }


    private void AllButtonsFadeIn()
    {
        _allStatsUpgradeButton.ButtonFadeIn();
    }

    private void StopAllButtonsFade(bool highlight)
    {
        _allStatsUpgradeButton.StopButtonFade(false, highlight);
    }

    
    private void OnCanNotUpgradeStats() 
    {
        _allStatsUpgradeButton.ButtonPressedErrorFadeInOut();
    }




    private void OnUpgradeAllStatsButtonClicked()
    {
        bool canUpgradeStat = CanUpgrade();

        if (canUpgradeStat)
        {
            SpendUpgradeCost();
            UpgradeAllTurretStats();
        }
        else
        {
            PlayNegativeAnimationTextCostPunch();
            OnCanNotUpgradeStats();
        }
    }

    private void UpgradeAllTurretStats()
    {
        _buildingUpgradesController.IncrementUpgradeLevel();
        NextLevel();

        _turretBuilding.InvokeOnBuildingUpgraded();

        CheckStopParticlesCanUpgrade();
        PlayPositiveAnimationTextCostPunch();
        InvokeOnUpgrade(TurretUpgradeType.ATTACK);
        InvokeOnUpgrade(TurretUpgradeType.CADENCE);
        InvokeOnUpgrade(TurretUpgradeType.RANGE);
    }

    public override void FreeTurretUpgrade() 
    { 
        UpgradeAllTurretStats(); 
    }

    private void OnUpgradeAllStatsButtonHovered()
    {
        _damageUpgradeStat.ViewProgression(NextDamageStatValueText);
        _fireRateUpgradeStat.ViewProgression(NextFireRateStatValueText);
        _rangeUpgradeStat.ViewProgression(NextRangeStatValueText);
    }
    private void OnUpgradeAllStatsButtonUnhovered()
    {
        _damageUpgradeStat.HideProgression();
        _fireRateUpgradeStat.HideProgression();
        _rangeUpgradeStat.HideProgression();

        AllButtonsFadeIn();
    }

}
