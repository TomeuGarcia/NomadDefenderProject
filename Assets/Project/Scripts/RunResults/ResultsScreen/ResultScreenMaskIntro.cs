using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ResultScreenMaskIntro : MonoBehaviour
{
    [Header("GENERAL")]
    [SerializeField] private float _animationTime;
    [SerializeField] private AnimationCurve _ease;

    [Header("MASK")]
    [SerializeField] private RectMask2D _mask;
    [SerializeField] private Vector4 _initialMask;
    [SerializeField] private Vector4 _endMask;

    [Header("SCROLL")]
    [SerializeField] private float _spInitialPosition;
    [SerializeField] private float _spEndPosition;
    [SerializeField] private float _scrollTime;
    [SerializeField] private float _scrollFirstDelay;
    [SerializeField] private float _scrollWaitTime;

    [Header("IMAGE")]
    [SerializeField] private float _initialPosition;
    [SerializeField] private float _endPosition;

    private void Awake()
    {
        _initialPosition += 100.0f;
        _endPosition += 100.0f;

        _mask.padding = _initialMask;
    }

    public void StartScroll(RectTransform scrollFadeParent)
    {
        DOTween.To(() => _mask.padding,
        w => _mask.padding = w,
        _endMask,
        _animationTime
        ).SetEase(_ease);

        StartCoroutine(ScrollParent(scrollFadeParent));
    }

    private IEnumerator ScrollParent(RectTransform scrollFadeParent)
    {
        yield return new WaitForSeconds(_scrollFirstDelay);

        while (true)
        {
            scrollFadeParent.DOLocalMoveY(_spEndPosition, _scrollTime).SetEase(_ease);
            yield return new WaitForSeconds(_scrollWaitTime + _scrollTime);
            scrollFadeParent.DOKill();
            scrollFadeParent.localPosition = Vector3.up * _spInitialPosition;
        }
    }
}
