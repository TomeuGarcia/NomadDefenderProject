using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class ResultScreenMaskIntro : MonoBehaviour
{
    [Header("GENERAL")]
    [SerializeField] private float _animationTime;
    [SerializeField] private AnimationCurve _ease;

    [Header("MASK")]
    [SerializeField] private RectMask2D _mask;
    [SerializeField] private Vector4 _initialMask;
    [SerializeField] private Vector4 _endMask;

    [Header("IMAGE")]
    [SerializeField] private RectTransform _bottomImage;
    [SerializeField] private float _initialPosition;
    [SerializeField] private float _endPosition;

    private void Awake()
    {
        _initialPosition += 100.0f;
        _endPosition += 100.0f;
    }

    private void Start()
    {
        _bottomImage.localPosition = Vector3.up * _initialPosition;
        _mask.padding = _initialMask;
        StartCoroutine(AAAAAA());
    }
    public IEnumerator AAAAAA()
    {
        yield return new WaitForSeconds(3.0f);

        DOTween.To(() => _mask.padding,
        w => _mask.padding = w,
        _endMask,
        _animationTime
        ).SetEase(_ease)
         .OnComplete(() => Debug.Log("Tween completed!"));

        _bottomImage.DOLocalMoveY(_endPosition, _animationTime).SetEase(_ease);
    }
}
