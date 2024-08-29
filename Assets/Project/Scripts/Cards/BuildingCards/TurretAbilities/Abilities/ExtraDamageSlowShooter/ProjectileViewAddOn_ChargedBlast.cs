using UnityEngine;

public class ProjectileViewAddOn_ChargedBlast : AProjectileViewAddOn
{
    [SerializeField] private GameObject _fullyChargedHolder;
    
    public static IConfigurationSource ConfigurationSource { get; set; }
    public interface IConfigurationSource
    {
        bool IsFullyCharged();
    }

    protected override void DoOnProjectileSpawned()
    {
        _fullyChargedHolder.SetActive(ConfigurationSource.IsFullyCharged());
    }

    protected override bool AllAffectsFinished()
    {
        return true;
    }
}