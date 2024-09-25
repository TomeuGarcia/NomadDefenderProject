using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CardMotionEffectsController : MonoBehaviour
{
    [SerializeField] private Transform _rotationEffectsTransform;
    private FloatingCardMotion _floatingEffect;
    private RotatingWithMouseCardMotion _hoveredRotationEffect;
    private ICameraHelpService _cameraHelp;

    public Transform RotationEffectsTransform => _rotationEffectsTransform;
    private Camera CardsCamera => _cameraHelp.CardsCamera;
    private float _motionEnabled01;

    public void Init(
        CardMotionConfig.RotationEffect idleRotationEffect, 
        CardMotionConfig.RotationEffect hoveredMouseRotationEffect)
    {
        _cameraHelp = ServiceLocator.GetInstance().CameraHelp;
        _floatingEffect = new FloatingCardMotion(_rotationEffectsTransform, idleRotationEffect);
        _hoveredRotationEffect = new RotatingWithMouseCardMotion(_cameraHelp, _rotationEffectsTransform, hoveredMouseRotationEffect);
        _motionEnabled01 = 1;
    }

    private void Update()
    {
        if (CardsCamera == null || _motionEnabled01.AlmostZero())
        {
            return;
        }

        Vector2 cardScreenPosition = CardsCamera.WorldToScreenPoint(_rotationEffectsTransform.position);
        float screenWidthRatio = 1f - Mathf.Abs(((cardScreenPosition.x / Screen.currentResolution.width) - 0.5f) * 2);

        float effectMultiplier = screenWidthRatio * _motionEnabled01;

        _floatingEffect.Update(Time.deltaTime, effectMultiplier);
        _hoveredRotationEffect.Update(effectMultiplier);
    }

    public void StartHoverMotion()
    {
        _floatingEffect.Finish();
        _hoveredRotationEffect.Start();
    }
    
    public void FinishHoverMotion()
    {
        _floatingEffect.Start();
        _hoveredRotationEffect.Finish();
    }

    public void EnableMotion()
    {
        TransitionMotionEnabled(1);
    }
    public void DisableMotion()
    {
        TransitionMotionEnabled(0);
    }

    private void TransitionMotionEnabled(float finalValue)
    {
        float motionEnabled01 = _motionEnabled01;
        DOTween.To(
            () => motionEnabled01,
            (value) => _motionEnabled01 = motionEnabled01 = value,
            finalValue,
            0.2f
        )
        .SetEase(Ease.InOutSine);
    }

}
