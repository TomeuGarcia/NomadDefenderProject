using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowIdleAnimator : MonoBehaviour
{
    [Header("GENERAL")]
    [SerializeField] private Transform _target;

    [Header("ROTATION")]
    [SerializeField] private float _rotationDelay;
    [SerializeField] private float _rotationDuration;
    [SerializeField] private float _rotationDegrees;

    [Header("INSERTION")]
    [SerializeField] private float _insertionDelay;
    [SerializeField] private float _insertionDuration;
    [SerializeField] private float _insertionRecoverDelay;
    [SerializeField] private float _insertionRecoverDuration;
    [SerializeField] private float _insertionHeightDisplacement;
    private float _originalHeightPosition = 0.0f;

    private void OnEnable()
    {
        StartCoroutine(AnimateInsertion());
    }

    private IEnumerator AnimateInsertion()
    {
        yield return new WaitForSeconds(_insertionDelay);
        _target.DOLocalMoveY(_insertionHeightDisplacement, _insertionDuration);
        yield return new WaitForSeconds(_insertionDuration);

        yield return new WaitForSeconds(_insertionRecoverDelay);
        _target.DOLocalMoveY(_originalHeightPosition, _insertionRecoverDuration)
            .OnComplete(() => StartCoroutine(AnimateRotation()));
    }

    private IEnumerator AnimateRotation()
    {
        yield return new WaitForSeconds(_rotationDelay);
        _target.DOBlendableLocalRotateBy(_target.up * _rotationDegrees, _rotationDuration)
            .OnComplete(() => StartCoroutine(AnimateInsertion()));
    }

    private void OnDisable()
    {
        _target.DOComplete();
        _target.DOKill();
    }
}
