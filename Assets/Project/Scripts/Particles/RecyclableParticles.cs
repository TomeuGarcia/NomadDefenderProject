using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.ObjectPooling;

public class RecyclableParticles : RecyclableObject
{
    [SerializeField] private ParticleSystem _particles;

    private void OnParticleSystemStopped()
    {
        Recycle();
    }


    internal override void RecycledInit()
    {
        _particles.Play();
    }

    internal override void RecycledReleased()
    {

    }


    public void SetMaterial(Material material)
    {
        ParticleSystemRenderer particleRenderer = _particles.GetComponentInChildren<ParticleSystemRenderer>();
        particleRenderer.material = material;
    }


}