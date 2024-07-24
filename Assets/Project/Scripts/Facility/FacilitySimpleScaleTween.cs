using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacilitySimpleScaleTween : MonoBehaviour
{
    [SerializeField] private float _toOffsetTime;
    [SerializeField] private Ease _toOffsetEase;
    [SerializeField] private float _toInitialtime;
    [SerializeField] private Ease _toInitialEase;

    [SerializeField] private Vector3 _scaleOffset;
    private Vector3 _initialScale;
    private Vector3 _finalScale;

    private void Awake()
    {
        _initialScale = transform.localScale;
        _finalScale = _initialScale + _scaleOffset;
    }

    public void OnEnable()
    {
        ToOffsetedScale();
    }

    private void ToOffsetedScale()
    {
        transform.DOScale(_finalScale, _toOffsetTime).SetEase(_toOffsetEase).OnComplete(ToInitialScale);
    }

    private void ToInitialScale()
    {
        transform.DOScale(_initialScale, _toInitialtime).SetEase(_toInitialEase).OnComplete(ToOffsetedScale);
    }

    private void OnDisable()
    {
        transform.DOKill();
        transform.localScale = _initialScale;
    }
}
