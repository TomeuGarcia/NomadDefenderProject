using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretViewAddOn_ExtraDamageSameEnemy : ATurretViewAddOn
{
    [SerializeField] private ParticleSystem _particleSystem;
    private int _numberOfAccumulatedHits = 0;


    public IConfigurationSource ConfigurationSource { get; set; }
    public interface IConfigurationSource
    {
        int GetNumberOfAccumulatedHits();
    }

    

    internal override void RecycledInit()
    {

    }

    internal override void RecycledReleased()
    {

    }

    protected override void StartPlayingEffects()
    {

    }

    private void Update()
    {
        if(_numberOfAccumulatedHits != ConfigurationSource.GetNumberOfAccumulatedHits())
        {
            UpdateEffect();
        }
    }

    private void UpdateEffect()
    {
        _particleSystem.Stop();
        _particleSystem.Play();

        _numberOfAccumulatedHits = ConfigurationSource.GetNumberOfAccumulatedHits();
        ParticleSystem.EmissionModule emissionModule = _particleSystem.emission;
        emissionModule.rateOverTime = _numberOfAccumulatedHits * 2f;
    }

    protected override void StopPlayingEffects()
    {
        _particleSystem.Stop();
    }
}