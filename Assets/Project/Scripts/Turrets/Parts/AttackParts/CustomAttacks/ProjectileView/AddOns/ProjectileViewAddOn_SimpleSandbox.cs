using UnityEngine;

public class ProjectileViewAddOn_SimpleSandbox : AProjectileViewAddOn
{
    [SerializeField] private TrailRenderer _projectileTrail;
    [SerializeField] private ParticleSystem _lifetimeParticles;
    [SerializeField] private ParticleSystem _hitTargetParticles;

    private float _spawnTime;

    protected override bool AllAffectsFinished()
    {
        bool allEffectsFinished = true; 
        
        if (_lifetimeParticles)
        {
            float currentTime = Time.time * GameTime.TimeScale;
            allEffectsFinished = (currentTime - _spawnTime) > _lifetimeParticles.main.duration;
        }
        
        if (_hitTargetParticles)
        {
            allEffectsFinished = !_hitTargetParticles.isEmitting;
        }
        
        return allEffectsFinished;
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
            _spawnTime = Time.time * GameTime.TimeScale;
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
    

    internal override void RecycledInit()
    {
        
    }
    internal override void RecycledReleased()
    {
        
    }
}