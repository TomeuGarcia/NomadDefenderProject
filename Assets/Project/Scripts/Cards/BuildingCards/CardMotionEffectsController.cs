using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardMotionEffectsController : MonoBehaviour
{
    [SerializeField] private Transform _rotationEffectsTransform;
    private FloatingCardMotion _floatingEffect;

    public void Init(CardMotionConfig.RotationEffect hoveredRotationEffect)
    {
        _floatingEffect = new FloatingCardMotion(_rotationEffectsTransform, hoveredRotationEffect);
    }

    private void Update()
    {
        _floatingEffect.Update();
    }

    public void StartHoverMotion()
    {
        _floatingEffect.Finish();
    }
    
    public void FinishHoverMotion()
    {
        _floatingEffect.Finish();
    }

}
