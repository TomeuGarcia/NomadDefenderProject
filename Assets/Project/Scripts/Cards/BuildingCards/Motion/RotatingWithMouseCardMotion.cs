using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RotatingWithMouseCardMotion
{
    private ICameraHelpService _cameraHelp;
    private Transform _effectTransform;
    private CardMotionConfig.RotationEffect _rotationEffect;
    private float _effectAmount;

    private Camera CardsCamera => _cameraHelp.CardsCamera;


    public RotatingWithMouseCardMotion(ICameraHelpService cameraHelp, Transform effectTransform, CardMotionConfig.RotationEffect rotationEffect)
    {
        _cameraHelp = cameraHelp;
        _effectTransform = effectTransform;
        _rotationEffect = rotationEffect;
        _effectAmount = 0f;
    }

    public void Update(float effectMultiplicator)
    {
        if (_effectAmount.AlmostZero())
        {            
            return;
        }

        Vector2 mousePosition = Input.mousePosition;
        Vector2 cardScreenPosition = CardsCamera.WorldToScreenPoint(_effectTransform.position);

        Vector2 screenOffset = mousePosition - cardScreenPosition;
        screenOffset.x /= Screen.currentResolution.width;
        screenOffset.y /= Screen.currentResolution.height;
        screenOffset.x = Mathf.Min(1, screenOffset.x / 0.1f);
        screenOffset.y = Mathf.Min(1, screenOffset.y / 0.1f);

        Vector2 rotationAngles = _rotationEffect.MaxRotationAngles * (_effectAmount * effectMultiplicator);

        Vector2 tilt = new Vector2(
            screenOffset.y * rotationAngles.x, 
            -screenOffset.x * rotationAngles.y
        );

        Vector3 localRotationEuler = new Vector3(tilt.x, tilt.y, 0f);

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
