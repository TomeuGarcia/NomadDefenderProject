using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading.Tasks;
using TMPro;


public class InBattleUpgradeStat : MonoBehaviour
{
    [Header("STAT")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Image _iconImage;
    [SerializeField] private TMP_Text _statValueText;

    [Header("STAT PROGRESSION")]
    [SerializeField] private GameObject _statProgressionHolder;
    [SerializeField] private TMP_Text _statProgressionValueText;
    [SerializeField] private Image _arrowProgressionImage;

    private InBattleUpgradeConditionChecker _conditionChecker;


    public bool IsButtonHovered { get; private set; } = false;



    private static Color normalColor = Color.white;
    private static Color errorColor = Color.red;
    private static Color highlightedColor = Color.cyan;
    private static Color fadedInColor = Color.white;
    private static Color fadedOutColor = new Color(0.7f, 0.7f, 0.7f);
    private static Color disabledColor = new Color(0.15f, 0.15f, 0.15f);
   

    public void Init(InBattleUpgradeConditionChecker conditionChecker)
    {
        _conditionChecker = conditionChecker;

        _statProgressionValueText.color = highlightedColor;
        _arrowProgressionImage.color = highlightedColor;


        _statProgressionHolder.gameObject.SetActive(false);
    }


    public void SetupOpenAnimation()
    {
        _canvasGroup.DOComplete();
        _canvasGroup.alpha = 0f;
    }
    public async Task PlayOpenAnimation(float duration)
    {
        await _canvasGroup.DOFade(1f, duration).AsyncWaitForCompletion();
    }


    public void SetupCloseAnimation()
    {
        _canvasGroup.DOComplete();
        _canvasGroup.alpha = 1f;
    }
    public async Task PlayCloseAnimation(float duration)
    {
        await _canvasGroup.DOFade(0f, duration).AsyncWaitForCompletion();
    }

    public void PlayIconPunchAnimation()
    {
        _iconImage.transform.DOPunchScale(Vector3.one * 0.5f, 0.5f, 7)
            .OnComplete(() => _iconImage.transform.localScale = Vector3.one);
    }

    private void EmptyStatBar()
    {
        HideProgression();
    }

    private void SetStatValueText(string statValue)
    {
        _statValueText.text = statValue;
    }
    private void SetStatProgressionValueText(string statValue)
    {
        _statProgressionValueText.text = statValue;
    }




    public void UpdateView(string statValue, bool isCardUpgradedToMax)
    {
        //fillBars[(int)TurretUpgradeType.ATTACK].fillAmount = (float)attackLvl * turretFillBarCoef;

        PlayIconPunchAnimation();
        
        // Update UI
        if (isCardUpgradedToMax)
        {
            EmptyStatBar();
        }

        PlayIconPunchAnimation();

        SetStatValueText(statValue);
    }


    public void ViewProgression(string statValue)
    {
        SetStatProgressionValueText(statValue);
        _statProgressionHolder.SetActive(true);

        GameAudioManager.GetInstance().PlayCardInfoShown();
    }

    public void HideProgression()
    {
        _statProgressionHolder.SetActive(false);
    }


}
