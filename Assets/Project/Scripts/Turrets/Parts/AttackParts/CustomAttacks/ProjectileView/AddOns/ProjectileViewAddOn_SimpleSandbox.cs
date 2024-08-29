using UnityEngine;

public class ProjectileViewAddOn_SimpleSandbox : AProjectileViewAddOn
{
    [SerializeField] private TrailRenderer _projectileTrail;
    [SerializeField] private ParticleSystem _lifetimeParticles;
    [SerializeField] private ProjectileParticleType _hitTargetParticleType = ProjectileParticleType.None;

    

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
        if (_hitTargetParticleType != ProjectileParticleType.None)
        {
            ProjectileParticleFactory.GetInstance().CreateParticlesGameObject(_hitTargetParticleType,
                target.position, transform.rotation);
        }
    }
    

    
}