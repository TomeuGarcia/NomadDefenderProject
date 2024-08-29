
using System;
using UnityEngine;

public class ProjectileViewAddOn_Berserker : AProjectileViewAddOn
{
    private const float MIN_TIME_ACTIVE = 0.3f;
    
    protected override bool AllAffectsFinished()
    {
        return HasPassedTimeSinceSpawned(MIN_TIME_ACTIVE);
    }
}