using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public static class DoTweenExtensions_Punch
{
    // SCALE
    public static Tweener PunchScale(this Transform target, TweenPunchConfig config, bool completeBeforeApplying = false)
    {
        if (completeBeforeApplying)
        {
            target.DOComplete();
        }

        return target.DOPunchScale(config.Value, config.Duration, config.Vibrato, config.Elasticity)
            .SetEase(config.Ease);
    }

    // ROTATION
    public static Tweener PunchRotation(this Transform target, TweenPunchConfig config, bool completeBeforeApplying = false)
    {
        if (completeBeforeApplying)
        {
            target.DOComplete();
        }

        return target.DOPunchRotation(config.Value, config.Duration, config.Vibrato, config.Elasticity)
            .SetEase(config.Ease);
    }

    // POSITION
    public static Tweener PunchPosition(this Transform target, TweenPunchConfig config, bool completeBeforeApplying = false)
    {
        if (completeBeforeApplying)
        {
            target.DOComplete();
        }

        return target.DOPunchPosition(config.Value, config.Duration, config.Vibrato, config.Elasticity)
            .SetEase(config.Ease);
    }


    // COLOR
    public static Tweener PunchColor(this Graphic target, TweenColorConfig config)
    {
        Color originalColor = target.color;
        return target.DOColor(config.Value, config.Duration).SetEase(config.Ease)
            .OnComplete(() => target.DOColor(originalColor, config.Duration).SetEase(config.Ease));
    }
        
        
        
}