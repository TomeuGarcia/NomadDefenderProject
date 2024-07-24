using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CurrencyDropOverTimeCanvasView : MonoBehaviour, ICurrencyDropOverTimeView
{
    [Header("COMPONENTS")]
    [SerializeField] private CanvasGroup _holderCanvasGroup;
    [SerializeField] private Image _fillImage;
    [SerializeField] private Image _colorPunchImage;
    [SerializeField] private Transform _imageHolder;

    [Header("CONFIG")]
    [SerializeField] private TweenColorConfig _dropCurrencyColorPunch;
    [SerializeField] private TweenPunchConfig _dropCurrencyScalePunch;

    private void Awake()
    {
        Hide();
    }

    public void Show()
    {
        _holderCanvasGroup.alpha = 1f;
    }

    public void Hide()
    {
        _holderCanvasGroup.alpha = 0f;
    }


    public void Update(float value01)
    {
        _fillImage.fillAmount = value01.ToSine01();
    }

    public void PlayDropAnimation()
    {
        _colorPunchImage.PunchColor(_dropCurrencyColorPunch);
        _imageHolder.PunchScale(_dropCurrencyScalePunch);
    }
}
