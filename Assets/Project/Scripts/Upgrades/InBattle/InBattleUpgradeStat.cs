using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading.Tasks;
using TMPro;

public class InBattleUpgradeStat : MonoBehaviour
{
    [Header("Range")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Image _iconImage;
    [SerializeField] private Button _button;
    [SerializeField] private TMP_Text _statValueText;
    [SerializeField] private Image _barToCurrencyCost;

    private InBattleUpgradeConditionChecker _conditionChecker;

    public bool IsButtonHovered { get; private set; } = false;
    public bool IsButtonInteractable => _button.interactable;

    private Image ButtonImage => _button.image;
    private Transform ButtonImageTransform => ButtonImage.transform;
    private Transform ButtonTransform => _button.transform;


    private static Color normalColor = Color.white;
    private static Color errorColor = Color.red;
    private static Color highlightedColor = Color.cyan;
    private static Color fadedInColor = Color.white;
    private static Color fadedOutColor = new Color(0.7f, 0.7f, 0.7f);
    private static Color disabledColor = new Color(0.15f, 0.15f, 0.15f);

    public void Init(InBattleUpgradeConditionChecker conditionChecker)
    {
        _conditionChecker = conditionChecker;
        _barToCurrencyCost.fillAmount = 0f;
    }

    public void SetupOpenAnimation()
    {
        _canvasGroup.DOComplete();
        _canvasGroup.alpha = 0f;
    }
    public async Task PlayOpenAnimation(float duration, bool canBeUpgraded, float upgradeableDuration)
    {
        await _canvasGroup.DOFade(1f, duration).AsyncWaitForCompletion();

        if (canBeUpgraded)
        {
            ButtonImage.transform.DOPunchScale(Vector3.one * 0.2f, upgradeableDuration, 8);
        }
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


    public void DisableButton()
    {
        _button.interactable = false;
        ButtonImage.color = disabledColor;

        _button.DOKill();
        ButtonImage.DOKill();
    }

    public void EmptyStatBar(string statValue)
    {
        float duration = 0.2f;

        _barToCurrencyCost.DOComplete();
        _barToCurrencyCost.DOFillAmount(0f, duration);

        ResetStatBarColor();

        ButtonTransform.DOComplete();
        ButtonTransform.DOScale(Vector3.one, duration);

        _statValueText.text = statValue;
    }


    public void ResetStatBarColor()
    {
        _barToCurrencyCost.color = normalColor;
        ButtonImage.color = normalColor;
    }

    public void ButtonFadeIn(bool onEndFadeOut = true)
    {
        if (!IsButtonInteractable) { return; }

        ButtonImage.DOBlendableColor(fadedInColor, 1.0f);
        ButtonTransform.DOScale(1.2f, 1.0f)
            .OnComplete(() => { if (onEndFadeOut) ButtonFadeOut(); });
    }

    public void ButtonFadeOut(bool onEndFadeIn = true)
    {
        ButtonImage.DOBlendableColor(fadedOutColor, 1.0f);
        ButtonTransform.DOScale(1.0f, 1.0f)
            .OnComplete(() => { if (onEndFadeIn) ButtonFadeIn(); });
    }

    public void StopButtonFade(bool goToFadedOut, bool highlight)
    {
        ButtonTransform.DOKill();
        ButtonImage.DOKill();

        if (highlight)
        {
            ButtonImage.DOKill();
            ButtonImage.DOBlendableColor(Color.cyan, 0.1f);
        }

        if (goToFadedOut && IsButtonInteractable)
        {
            ButtonFadeOut(false);
        }
    }

    public void ButtonPressedErrorFadeInOut()
    {
        ButtonImage.DOBlendableColor(errorColor, 0.1f)
            .OnComplete(() => ButtonImage.DOBlendableColor(normalColor, 0.1f));
        _barToCurrencyCost.DOBlendableColor(errorColor, 0.1f)
            .OnComplete(() => _barToCurrencyCost.DOBlendableColor(normalColor, 0.1f));
    }


    public void UpdateView(string statValue, bool isCardUpgradedToMax, bool isStatMaxed)
    {
        //fillBars[(int)TurretUpgradeType.ATTACK].fillAmount = (float)attackLvl * turretFillBarCoef;

        PlayIconPunchAnimation();

        // Update UI
        if (isCardUpgradedToMax || isStatMaxed)
        {
            EmptyStatBar(statValue);
        }
        else if (!_conditionChecker.HasEnoughCurrencyToLevelUp())
        {
            ResetStatBarColor();
        }
        PlayIconPunchAnimation();

        if (isStatMaxed)
        {
            DisableButton();
        }
    }

    public void SetBarAndButtonHighlighted()
    {
        _barToCurrencyCost.DOComplete();
        _barToCurrencyCost.DOBlendableColor(highlightedColor, 0.1f);

        ButtonImage.DOComplete();
        ButtonImage.DOBlendableColor(highlightedColor, 0.1f);
    }


    public void OnButtonHovered(bool highlight, bool isCardUpgradedToMax, bool isStatMaxed, string statValue)
    {
        if (!IsButtonInteractable || isCardUpgradedToMax || isStatMaxed) return;

        FillStatBar(highlight, statValue);

        IsButtonHovered = true;
    }

    private void FillStatBar(bool highlight, string statValue)
    {
        float duration = 0.2f;

        _barToCurrencyCost.DOComplete();
        _barToCurrencyCost.DOFillAmount(1f, duration);
        if (highlight)
        {
            _barToCurrencyCost.color = highlightedColor;
        }

        ButtonImageTransform.DOComplete();
        ButtonImageTransform.DOScale(Vector3.one * 1.1f, duration);
        ButtonImage.DOBlendableColor(fadedInColor, duration * 0.5f);

        _statValueText.text = statValue;

        GameAudioManager.GetInstance().PlayCardInfoShown();
    }


    public void OnButtonUnhovered(bool isCardUpgradedToMax, bool isStatMaxed, string statValue)
    {
        if (!IsButtonInteractable || isCardUpgradedToMax || isStatMaxed) return;

        EmptyStatBar(statValue);

        IsButtonHovered = false;
    }
}
