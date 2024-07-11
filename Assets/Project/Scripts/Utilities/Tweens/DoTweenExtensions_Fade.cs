using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public static class DoTweenExtensions_Fade
{
    // CANVAS GROUP
    public static Tweener Fade(this CanvasGroup target, TweenFadeConfig config, bool completeBeforeApplying = false)
    {
        if (completeBeforeApplying)
        {
            target.DOComplete();
        }

        return target.DOFade(config.Value, config.Duration)
            .SetEase(config.Ease);
    }

}