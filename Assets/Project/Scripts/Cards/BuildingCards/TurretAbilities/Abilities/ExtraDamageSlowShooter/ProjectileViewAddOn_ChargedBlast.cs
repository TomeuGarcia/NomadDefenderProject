using UnityEngine;

public class ProjectileViewAddOn_ChargedBlast : AProjectileViewAddOn
{
    [SerializeField] private GameObject _fullyChargedHolder;
    [SerializeField] private GameObject _regularTrails;
    [SerializeField] private ParticleSystem _lifetimeParticles;

    public static IConfigurationSource ConfigurationSource { get; set; }
    public interface IConfigurationSource
    {
        bool IsFullyCharged();
    }

    protected override void DoOnProjectileSpawned()
    {
        if(ConfigurationSource.IsFullyCharged())
        {
            _fullyChargedHolder.SetActive(true);
            _regularTrails.SetActive(false);
        }
        else
        {
            _fullyChargedHolder.SetActive(false);
            _regularTrails.SetActive(true);
        }

        if (_lifetimeParticles)
        {
            _lifetimeParticles.Play();
        }

        transform.localRotation = Quaternion.identity;
    }

    protected override void DoOnProjectileDisappear()
    {
        if (_lifetimeParticles)
        {
            _lifetimeParticles.Stop();
        }
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
}