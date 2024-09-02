using System;
using System.Collections.Generic;
using UnityEngine;

public class TurretSingleProjectileView : ITurretProjectileView
{
    private readonly Transform _addOnsParent;
    private readonly List<AProjectileViewAddOn> _dynamicAddOns;


    public TurretSingleProjectileView(Transform addOnsParent)
    {
        _addOnsParent = addOnsParent;
        _dynamicAddOns = new List<AProjectileViewAddOn>();
    }

    public void AddViewAddOn(ProjectileViewAddOnConfig addOnConfig)
    {
        AProjectileViewAddOn addOn = ProjectileParticleFactory.GetInstance().CreateProjectileAddOn(addOnConfig);
        _dynamicAddOns.Add(addOn);
    }
    

    public void OnProjectileSpawned()
    {
        foreach (AProjectileViewAddOn activeAddOn in _dynamicAddOns)
        {
            activeAddOn.OnProjectileSpawned(_addOnsParent);
        }
    }
    
    public void OnProjectileDisappear()
    {
        foreach (AProjectileViewAddOn activeAddOn in _dynamicAddOns)
        {
            activeAddOn.OnProjectileDisappear();
        }
        
        _dynamicAddOns.Clear();
    }
        
    public void OnProjectileHitsTarget(Transform target)
    {
        foreach (AProjectileViewAddOn activeAddOn in _dynamicAddOns)
        {
            activeAddOn.OnProjectileHitsTarget(target);
        }
    }
    
    
}