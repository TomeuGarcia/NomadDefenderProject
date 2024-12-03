
using System;
using UnityEngine;

public class ProjectileViewAddOn_ExtraDamagePerCurrency : AProjectileViewAddOn
{
    [SerializeField] private TrailRenderer _projectileTrail;
    [SerializeField] private ParticleSystem _lifetimeParticles;

    
    public static IConfigurationSource ConfigurationSource { get; set; }
    public interface IConfigurationSource
    {
        int BonusDamageSteps();
    }


    protected override bool AllAffectsFinished()
    {
        bool allEffectsFinished = true; 
        
        if (_lifetimeParticles)
        {
            allEffectsFinished = HasPassedTimeSinceSpawned(_lifetimeParticles.main.duration);
        }

        return allEffectsFinished;
    }

    protected override void DoOnProjectileSpawned()
    {
        transform.localRotation = Quaternion.identity;

        int steps = ConfigurationSource.BonusDamageSteps();
        ParticleSystem.EmissionModule emissionModule = _lifetimeParticles.emission;
        emissionModule.rateOverDistance = steps * 2;
        ParticleSystem.MainModule shapeModule = _lifetimeParticles.main;
        float logSteps = Mathf.Log10((steps * 0.5f) + 1f);
        shapeModule.startSize = new ParticleSystem.MinMaxCurve(logSteps * 0.5f, logSteps);

        if (_projectileTrail)
        {
            _projectileTrail.emitting = true;
        }

        if (_lifetimeParticles)
        {
            _lifetimeParticles.Play();
        }
    }

    protected override void DoOnProjectileDisappear()
    {
        if (_projectileTrail)
        {
            _projectileTrail.emitting = false;
        }
        
        if (_lifetimeParticles)
        {
            _lifetimeParticles.Stop();
        }
    }

}
