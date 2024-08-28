
using System;
using UnityEngine;

public class ProjectileViewAddOn_Berserker : AProjectileViewAddOn
{
    private float _timeStart;
    private const float MIN_TIME_ACTIVE = 0.3f;
    
    internal override void RecycledInit()
    {
        _timeStart = Time.time * GameTime.TimeScale;
    }

    internal override void RecycledReleased() { }

    protected override bool AllAffectsFinished()
    {
        return (Time.time * GameTime.TimeScale) - _timeStart > MIN_TIME_ACTIVE;
    }
}