using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RotatingWithMouseCardMotion
{
    private Transform _effectTransform;
    private CardMotionConfig.RotationEffect _rotationEffect;
    private float _effectAmount;


    public RotatingWithMouseCardMotion(Transform effectTransform, CardMotionConfig.RotationEffect rotationEffect)
    {
        _effectTransform = effectTransform;
        _rotationEffect = rotationEffect;
        _effectAmount = 0f;
    }

    public void Update()
    {
        return;

        Camera camera = Camera.main;
        if (camera == null || _effectAmount.AlmostZero())
        {
            return;
        }

        Vector3 mousePosition = camera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 mousePositionOffset = mousePosition - _effectTransform.position;

        Vector2 tilt = new Vector2(-mousePositionOffset.y, mousePositionOffset.x);
        Debug.Log(tilt);

        Vector2 rotationAngles = _rotationEffect.MaxRotationAngles * _effectAmount;

        Vector3 localRotationEuler = new Vector3(
            Mathf.Sin(tilt.x) * rotationAngles.x,
            Mathf.Cos(tilt.y) * rotationAngles.y,
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
        );
    }
}
