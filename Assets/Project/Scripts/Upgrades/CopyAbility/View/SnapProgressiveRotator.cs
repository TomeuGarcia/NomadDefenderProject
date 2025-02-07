using DG.Tweening;
using System.Collections;
using UnityEngine;

public class SnapProgressiveRotator : MonoBehaviour
{
    [SerializeField] private TweenConfig _config;
    [SerializeField] private AnimationCurve _curve;
    [SerializeField] private Vector2 _delay;

    private void Start()
    {
        StartCoroutine(Rotate());
    }

    private IEnumerator Rotate()
    {
        transform.DOBlendableLocalRotateBy(_config.Value, _config.Duration, RotateMode.FastBeyond360).SetEase(_curve);
        yield return new WaitForSeconds(_config.Duration);
        yield return new WaitForSeconds(Random.Range(_delay.x, _delay.y));
        StartCoroutine(Rotate());
    }
}
