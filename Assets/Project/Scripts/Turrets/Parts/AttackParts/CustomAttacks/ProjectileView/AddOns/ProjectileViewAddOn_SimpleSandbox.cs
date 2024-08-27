using UnityEngine;

public class ProjectileViewAddOn_SimpleSandbox : AProjectileViewAddOn
{
    [SerializeField] private TrailRenderer _projectileTrail;
    [SerializeField] private ParticleSystem _lifetimeParticles;
    [SerializeField] private ParticleSystem _hitTargetParticles;

    protected override bool AllAffectsFinished()
    {
        return (_hitTargetParticles && !_hitTargetParticles.isEmitting);
    }

    protected override void DoOnProjectileSpawned()
    {
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

    public override void OnProjectileHitsTarget(Transform target)
    {
        if (_hitTargetParticles)
        {
            _hitTargetParticles.Play();
        }
    }
    
}