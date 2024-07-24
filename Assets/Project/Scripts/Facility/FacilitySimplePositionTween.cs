using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacilitySimplePositionTween : MonoBehaviour
{
    [SerializeField] private float _toOffsetTime;
    [SerializeField] private Ease _toOffsetEase;
    [SerializeField] private float _toInitialtime;
    [SerializeField] private Ease _toInitialEase;

    [SerializeField] private Vector3 _positinOffset;
    private Vector3 _initialPosition;
    private Vector3 _finalPosition;

    private void Awake()
    {
        _initialPosition = transform.localPosition;
        _finalPosition = _initialPosition + _positinOffset;
    }

    public void OnEnable()
    {
        MoveToOffsetedPosition();
    }

    private void MoveToOffsetedPosition()
    {
        transform.DOLocalMove(_finalPosition, _toOffsetTime).SetEase(_toOffsetEase).OnComplete(MoveToInitialPosition);
    }

    private void MoveToInitialPosition()
    {
        transform.DOLocalMove(_initialPosition, _toInitialtime).SetEase(_toInitialEase).OnComplete(MoveToOffsetedPosition);
    }

    private void OnDisable()
    {
        transform.DOKill();
        transform.localPosition = _initialPosition;
    }
}
