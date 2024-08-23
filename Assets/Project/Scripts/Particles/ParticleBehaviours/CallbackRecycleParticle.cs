using System.Collections.Generic;
using Scripts.ObjectPooling;
using UnityEngine;

public class CallbackRecycleParticle : RecyclableObject
{
    [SerializeField] private List<ParticleSystem> _particleSystems = new();
    private int _completedParticles;

    internal override void RecycledInit()
    {
        _completedParticles = 0;

        foreach (var particleSystem in _particleSystems)
        {
            particleSystem.Play();
        }
    }

    public void OnParticleSystemStopped()
    {
        _completedParticles++;

        if (_completedParticles >= _particleSystems.Count)
        {
            RecycledReleased();
        }
    }

    public void ForceStop()
    {
        foreach (var particleSystem in _particleSystems)
        {
            if (particleSystem.isPlaying)
            {
                particleSystem.Stop();
            }
        }
    }

    internal override void RecycledReleased()
    {
        //
    }
}