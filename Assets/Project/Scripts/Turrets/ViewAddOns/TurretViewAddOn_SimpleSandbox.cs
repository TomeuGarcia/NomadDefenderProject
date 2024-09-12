using System.Collections.Generic;
using UnityEngine;

public class TurretViewAddOn_SimpleSandbox : ATurretViewAddOn
{
    [SerializeField] private ParticleSystem _lifetimeParticles;

    internal override void RecycledInit()
    {
        
    }

    internal override void RecycledReleased()
    {
        
    }

    protected override void StartPlayingEffects()
    {
        _lifetimeParticles.Play();
    }

    protected override void StopPlayingEffects()
    {
        _lifetimeParticles.Stop();
    }
}