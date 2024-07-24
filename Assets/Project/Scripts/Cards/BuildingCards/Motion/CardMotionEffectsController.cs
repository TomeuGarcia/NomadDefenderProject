using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardMotionEffectsController : MonoBehaviour
{
    [SerializeField] private Transform _rotationEffectsTransform;
    private FloatingCardMotion _floatingEffect;
    private RotatingWithMouseCardMotion _hoveredRotationEffect;

    public void Init(
        CardMotionConfig.RotationEffect idleRotationEffect, 
        CardMotionConfig.RotationEffect hoveredMouseRotationEffect)
    {
        _floatingEffect = new FloatingCardMotion(_rotationEffectsTransform, idleRotationEffect);
        _hoveredRotationEffect = new RotatingWithMouseCardMotion(_rotationEffectsTransform, hoveredMouseRotationEffect);
    }

    private void Update()
    {
        _floatingEffect.Update();
        _hoveredRotationEffect.Update();
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

}
