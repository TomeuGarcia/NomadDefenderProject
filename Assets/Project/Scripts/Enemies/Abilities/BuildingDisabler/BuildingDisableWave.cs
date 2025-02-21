using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Scripts.ObjectPooling;
using UnityEngine;
using Random = UnityEngine.Random;

public class BuildingDisableWave : RecyclableObject
{
    [SerializeField] private ParticleSystem _EMPHitParticles;
    [SerializeField] private AudioSource _waveAudioSource;
    [SerializeField] private AudioSource _hitAudioSource;
    
    private BuildingDisableWaveConfig _config;
    
    internal override void RecycledInit()
    {

    }

    internal override void RecycledReleased()
    {
        
    }

    public void Init(BuildingDisableWaveConfig config)
    {
        _config = config;

        _EMPHitParticles.Play();
        ApplyWaveEffect();
    }

    private void ApplyWaveEffect()
    {
        Collider[] collidersInRange = Physics.OverlapSphere(transform.position, _config.Radius, 
            _config.BuildingsLayerMask, QueryTriggerInteraction.Collide);

        foreach (Collider colliderInRange in collidersInRange)
        {
            if (colliderInRange.TryGetComponent(out IDisableableBuilding disableableBuilding) &&
                disableableBuilding.CanBeDisabled())
            {
                BuildingDisableManager.Instance.HandleNewBuilding(disableableBuilding, _config.DisableDuration);
            }
        }
        
        if (collidersInRange.Length > 0)
        {
            _hitAudioSource.Play();
        }
    }
}