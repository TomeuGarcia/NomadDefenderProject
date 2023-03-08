using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Support_InBattleBuildingUpgrader : InBattleBuildingUpgrader
{

    [Header("Ability")]
    [SerializeField] private CanvasGroup cgAbilityStat;
    [SerializeField] private Button abilityButton;
    [SerializeField] private Image abilityButtonImage;
    [SerializeField] private Image abilityBackFillImage;
    [SerializeField] private Image abilityBarToCurrencyCost;


    protected override void AwakeInit()
    {
        base.AwakeInit();
        abilityBarToCurrencyCost.fillAmount = 0f;
    }


    protected override void UpdateSupportBar()
    {
        fillBars[0].fillAmount = (float)supportLvl * supportFillBarCoef;

        // Update UI
        if (!CanUpgrade(supportLvl)) EmptyStatBar(abilityBarToCurrencyCost, abilityButtonImage, abilityBackFillImage, (float)attackLvl * turretFillBarCoef);

        if (IsStatMaxed(attackLvl)) abilityButton.interactable = false;
        if (IsCardUpgradedToMax(currentLevel)) DisableButtons();
    }

    protected override void DisableButtons()
    {
        abilityButton.interactable = false;
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


        float t1 = 0.1f;
        float t3 = 0.3f;
        bool isCardUpgradedToMax = IsCardUpgradedToMax(currentLevel);

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
        cgAbilityStat.DOComplete();


        barImage.fillAmount = 1f;
        backgroundImage.fillAmount = 1f;
        cgLvlText.alpha = 1f;
        cgCostText.alpha = 1f;
        cgAbilityStat.alpha = 1f;



        float t1 = 0.075f;
        float t2 = 0.15f;
        float t3 = 0.225f;


        cgCostText.DOFade(0f, t1);
        yield return new WaitForSeconds(t1);

        cgAbilityStat.DOFade(0f, t1);
        yield return new WaitForSeconds(t1);

        backgroundImage.DOFillAmount(0f, t3);
        yield return new WaitForSeconds(t1);

        cgLvlText.DOFade(0f, t1);
        barImage.DOFillAmount(0f, t1);
        yield return new WaitForSeconds(t2);

        //yield return new WaitUntil(() => barImage.fillAmount > 0.001f);
        closeAnimationCoroutine = null;
        newUiParent.gameObject.SetActive(false);
    }



    public void FillAbilityBar()
    {
        if (!abilityButton.interactable) return;
        if (IsCardUpgradedToMax(currentLevel) || IsStatMaxed(supportLvl)) return;

        FillStatBar(abilityBarToCurrencyCost, abilityButtonImage, abilityBackFillImage, (float)(supportLvl + 1) * supportFillBarCoef);
    }

    public void EmptyAbilityBar()
    {
        if (!abilityButton.interactable) return;
        if (IsCardUpgradedToMax(currentLevel) || IsStatMaxed(supportLvl)) return;

        EmptyStatBar(abilityBarToCurrencyCost, abilityButtonImage, abilityBackFillImage, (float)supportLvl * supportFillBarCoef);
    }

}
