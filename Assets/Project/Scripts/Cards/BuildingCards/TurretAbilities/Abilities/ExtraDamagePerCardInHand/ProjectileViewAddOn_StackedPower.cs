using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileViewAddOn_StackedPower : AProjectileViewAddOn
{
    [SerializeField] private AnimationCurve _multiplierOverCards;
    [SerializeField] private ParticleSystem _stackQuantityParticles;
    private ParticleSystem.MainModule _stackQuantityMain;


    public static IConfigurationSource ConfigurationSource { get; set; }
    public interface IConfigurationSource
    {
        int GetNumberOfCards();
    }
    
    
    
    private void Start()
    {
        _stackQuantityMain = _stackQuantityParticles.main;
    }

    protected override void DoOnProjectileSpawned()
    {
        float rateOverTime = _multiplierOverCards.Evaluate(ConfigurationSource.GetNumberOfCards());
        var emission = _stackQuantityParticles.emission;
        emission.rateOverTime = rateOverTime;
        _stackQuantityParticles.Play();
    }
    protected override void DoOnProjectileDisappear()
    {
        _stackQuantityParticles.Stop();
    }

    protected override bool AllAffectsFinished()
    {
        return HasPassedTimeSinceSpawned(_stackQuantityMain.duration);
    }
}