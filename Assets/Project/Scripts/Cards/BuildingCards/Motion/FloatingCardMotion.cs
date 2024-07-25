using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FloatingCardMotion
{
    private Transform _effectTransform;
    private CardMotionConfig.RotationEffect _rotationEffect;
    private float _effectAmount;
    private float _time;

    public FloatingCardMotion(Transform effectTransform, CardMotionConfig.RotationEffect rotationEffect)
    {
        _effectTransform = effectTransform;
        _rotationEffect = rotationEffect;
        _effectAmount = 0f;
        _time = Time.time + Random.Range(0f, 2 * Mathf.PI);
    }

    public void Update(float deltaTime, float effectMultiplicator)
    {
        if (_effectAmount.AlmostZero())
        {
            return;
        }


        _time += deltaTime;
        float rotationTime = _rotationEffect.RotationSpeed * _time;
        Vector2 rotationAngles = _rotationEffect.MaxRotationAngles * (_effectAmount * effectMultiplicator);

        Vector3 localRotationEuler = new Vector3(
            Mathf.Sin(rotationTime) * rotationAngles.x,
            Mathf.Cos(rotationTime) * rotationAngles.y,
            0f
        );

        _effectTransform.localRotation = Quaternion.Euler(localRotationEuler);
    }


    public void Start()
    {
        TransitionEffectValue(1f);
    }

    public void Finish()
    {
        TransitionEffectValue(0f);
    }

    private void TransitionEffectValue(float finalValue)
    {
        float effectAmount = _effectAmount;
        DOTween.To(
            () => effectAmount,
            (value) => _effectAmount = effectAmount = value,
            finalValue,
            _rotationEffect.TransitionDuration
        )
        .SetEase(Ease.InOutSine);
    }
}
