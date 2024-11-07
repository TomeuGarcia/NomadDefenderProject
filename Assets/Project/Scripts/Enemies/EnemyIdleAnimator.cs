using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleAnimator : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private Transform _target;

    [Header("PARAMETERS")]
    [SerializeField] private Vector2 _verticalDisplacementRange;
    [SerializeField] private Vector2 _verticalDisplacementDurationRange;
    [SerializeField] private Ease _verticalEaseIn;
    [SerializeField] private Ease _verticalEaseOut;

    [SerializeField] private Vector2 _horizontalDisplacementRange;
    [SerializeField] private Vector2 _horizontalDisplacementDurationRange;
    [SerializeField] private Ease _horizontalEaseIn;
    [SerializeField] private Ease _horizontalEaseOut;

    private void StartAnimation()
    {
        StartCoroutine(VerticalMovementAnimation());
        StartCoroutine(HorizontalMovementAnimation());
    }

    private IEnumerator VerticalMovementAnimation()
    {
        float verticalDisplacement = Random.Range(_verticalDisplacementRange.x, _verticalDisplacementRange.y);
        float verticalDisplacementDuration = Random.Range(_verticalDisplacementDurationRange.x, _verticalDisplacementDurationRange.y);

        _target.DOBlendableLocalMoveBy(Vector3.up * verticalDisplacement, verticalDisplacementDuration).SetEase(_verticalEaseIn);
        yield return new WaitForSeconds(verticalDisplacementDuration);
        _target.DOBlendableLocalMoveBy(-Vector3.up * verticalDisplacement, verticalDisplacementDuration).SetEase(_verticalEaseOut);
        yield return new WaitForSeconds(verticalDisplacementDuration);

        StartCoroutine(VerticalMovementAnimation());
    }

    private IEnumerator HorizontalMovementAnimation()
    {
        float horizontalDisplacement = Random.Range(_horizontalDisplacementRange.x, _horizontalDisplacementRange.y);
        float horizontalDisplacementDuration = Random.Range(_horizontalDisplacementDurationRange.x, _horizontalDisplacementDurationRange.y);

        _target.DOBlendableLocalMoveBy(Vector3.right * horizontalDisplacement, horizontalDisplacementDuration).SetEase(_horizontalEaseIn);
        yield return new WaitForSeconds(horizontalDisplacementDuration);
        _target.DOBlendableLocalMoveBy(-Vector3.right * horizontalDisplacement, horizontalDisplacementDuration).SetEase(_horizontalEaseOut);
        yield return new WaitForSeconds(horizontalDisplacementDuration);

        StartCoroutine(HorizontalMovementAnimation());
    }

    private void OnEnable()
    {
        StartAnimation();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        _target.DOComplete();
    }
}
