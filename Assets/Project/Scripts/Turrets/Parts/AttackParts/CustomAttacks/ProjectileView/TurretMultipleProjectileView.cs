using UnityEngine;

public class TurretMultipleProjectileView : ITurretProjectileView
{
    public interface ISource
    {
        Transform GetAddOnsParent();
    }
    
    
    private readonly TurretSingleProjectileView[] _singleProjectileViews;


    public TurretMultipleProjectileView(ISource[] sources)
    {
        _singleProjectileViews = new TurretSingleProjectileView[sources.Length];
        for (int i = 0; i < sources.Length; ++i)
        {
            _singleProjectileViews[i] = new TurretSingleProjectileView(sources[i].GetAddOnsParent());
        }
    }
    
    
    public void AddViewAddOn(ProjectileViewAddOnConfig addOnConfig)
    {
        foreach (TurretSingleProjectileView singleProjectileView in _singleProjectileViews)
        {
            singleProjectileView.AddViewAddOn(addOnConfig);
        }
    }


    public void OnProjectileSpawned()
    {
        foreach (TurretSingleProjectileView singleProjectileView in _singleProjectileViews)
        {
            singleProjectileView.OnProjectileSpawned();
        }
    }

    public void OnProjectileDisappear()
    {
        foreach (TurretSingleProjectileView singleProjectileView in _singleProjectileViews)
        {
            singleProjectileView.OnProjectileDisappear();
        }
    }

    public void OnProjectileHitsTarget(Transform target)
    {
        foreach (TurretSingleProjectileView singleProjectileView in _singleProjectileViews)
        {
            singleProjectileView.OnProjectileHitsTarget(target);
        }
    }
}