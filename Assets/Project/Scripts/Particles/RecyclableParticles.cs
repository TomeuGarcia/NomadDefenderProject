using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.ObjectPooling;

public class RecyclableParticles : RecyclableObject
{
    [SerializeField] private bool _playOnInit = true;
    [SerializeField] private ParticleSystem _particles;
    private Transform _originalParent;

    private void Awake()
    {
        _originalParent = transform.parent;
    }

    private void OnParticleSystemStopped()
    {
        Recycle();
    }


    internal override void RecycledInit()
    {
        if (_playOnInit) _particles.Play();
    }

    internal override void RecycledReleased()
    {
        transform.SetParent(_originalParent);
    }


    public void Stop()
    {
        _particles.Stop();
    }

    public void SetMaterial(Material material)
    {
        ParticleSystemRenderer particleRenderer = _particles.GetComponentInChildren<ParticleSystemRenderer>();
        particleRenderer.material = material;
    }


}