using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardMotionEffectsController : MonoBehaviour
{
    [SerializeField] private Transform _rotationEffectsTransform;
    private FloatingCardMotion _hoveredEffect;

    public void Init(CardMotionConfig.RotationEffect hoveredRotationEffect)
    {
        _hoveredEffect = new FloatingCardMotion(_rotationEffectsTransform, hoveredRotationEffect);
    }

    private void Update()
    {
        _hoveredEffect.Update();
    }

    public void StartHoverMotion()
    {
        _hoveredEffect.Start();
    }
    
    public void FinishHoverMotion()
    {
        _hoveredEffect.Finish();
    }

}
