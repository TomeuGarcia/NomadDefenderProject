using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacilitySimplePositionTween : MonoBehaviour
{
    [SerializeField] private bool _useRealtime = false;
    [SerializeField] private float _toOffsetTime;
    [SerializeField] private Ease _toOffsetEase;
    [SerializeField] private float _toInitialtime;
    [SerializeField] private Ease _toInitialEase;

    [SerializeField] private Vector3 _positinOffset;
    private Vector3 _initialPosition;
    private Vector3 _finalPosition;


    public void OnEnable()
    {
        MoveToOffsetedPosition();
    }

    private void MoveToOffsetedPosition()
    {
        _initialPosition = transform.localPosition;
        _finalPosition = _initialPosition + _positinOffset;
        transform.DOLocalMove(_finalPosition, _toOffsetTime).SetEase(_toOffsetEase)
            .SetUpdate(_useRealtime)
            .OnComplete(MoveToInitialPosition);
    }

    private void MoveToInitialPosition()
    {
        transform.DOLocalMove(_initialPosition, _toInitialtime).SetEase(_toInitialEase)
            .SetUpdate(_useRealtime)
            .OnComplete(MoveToOffsetedPosition);
    }

    private void OnDisable()
    {
        transform.DOKill();
        transform.localPosition = _initialPosition;
    }
}
