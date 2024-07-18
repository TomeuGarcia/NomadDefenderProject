using System.Collections;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StatUpgradeButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Image _buttonImage;
    [SerializeField] private CanvasGroup _canvasGroup;

    public bool IsButtonInteractable => _button.interactable;
    private Transform ButtonTransform => _button.transform;
    private Image ButtonImage => _buttonImage;
    private Transform ButtonImageTransform => ButtonImage.transform;


    private static Color normalColor = Color.white;
    private static Color disabledColor = new Color(0.15f, 0.15f, 0.15f);

    private static Color fadedInColor = Color.white;
    private static Color fadedOutColor = new Color(0.7f, 0.7f, 0.7f);

    private static Color errorColor = Color.red;
    private static Color highlightedColor = Color.cyan;

    private Action _onButtonHovered;
    private Action _onButtonUnhovered;

    public bool IsButtonHovered { get; private set; } = false;



    public void Init(Action clickedCallback, Action hoveredCallback, Action unhoveredCallback)
    {
        _onButtonHovered = hoveredCallback;
        _onButtonUnhovered = unhoveredCallback;
        _button.onClick.AddListener(new UnityEngine.Events.UnityAction(clickedCallback));
    }

    public void OnButtonHovered()
    {
        if (!_button.interactable) return;

        SetHovered();
        IsButtonHovered = true;
        _onButtonHovered();
    }

    public void OnButtonUnhovered()
    {
        if (!_button.interactable) return;

        ResetColor();
        IsButtonHovered = false;
        _onButtonUnhovered();
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
            ButtonImage.transform.DOPunchScale(Vector3.one * 0.1f, upgradeableDuration, 8);
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

    public void DisableButton()
    {
        _button.interactable = false;
        ButtonImage.color = disabledColor;

        _button.DOKill();
        ButtonImage.DOKill();
    }

    private void ResetColor()
    {
        ButtonImage.color = normalColor;
    }


    public void ButtonFadeIn(bool onEndFadeOut = true)
    {
        if (!IsButtonInteractable) { return; }

        ButtonImage.DOBlendableColor(fadedInColor, 1.0f);
        ButtonTransform.DOScale(0.9f, 0.8f)
            .OnComplete(() => { if (onEndFadeOut) ButtonFadeOut(); });
    }

    public void ButtonFadeOut(bool onEndFadeIn = true)
    {
        ButtonImage.DOBlendableColor(fadedOutColor, 1.0f);
        ButtonTransform.DOScale(1.0f, 0.8f)
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
            .OnComplete(() => ButtonImage.DOBlendableColor(IsButtonHovered ? highlightedColor : normalColor, 0.1f));
    }



    public void SetHighlighted()
    {
        if (!_button.interactable) return;

        ButtonImage.DOComplete();
        ButtonImage.DOBlendableColor(highlightedColor, 0.1f);
    }

    private void SetHovered()
    {
        float duration = 0.2f;

        ButtonTransform.DOKill();
        ButtonImage.DOKill();
        ButtonImageTransform.DOKill();

        ButtonImageTransform.DOScale(Vector3.one, duration);
        ButtonImage.DOBlendableColor(highlightedColor, duration * 0.5f);

        GameAudioManager.GetInstance().PlayCardInfoShown();
    }
}
