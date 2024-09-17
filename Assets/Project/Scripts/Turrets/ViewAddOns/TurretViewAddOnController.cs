using System.Collections.Generic;
using UnityEngine;

public class TurretViewAddOnController : ITurretViewAddOnController
{
    private readonly Transform _addOnsParent;
    private readonly List<ATurretViewAddOn> _dynamicViewAddOns;

    public TurretViewAddOnController(Transform addOnsParent)
    {
        _addOnsParent = addOnsParent;
        _dynamicViewAddOns = new List<ATurretViewAddOn>();
    }
    
    
    public ATurretViewAddOn AddViewAddOn(TurretViewAddOnConfig addOnConfig)
    {
        ATurretViewAddOn viewAddOn = ProjectileParticleFactory.GetInstance().CreateTurretAddOn(addOnConfig);
        _dynamicViewAddOns.Add(viewAddOn);
        return viewAddOn;
    }

    public void StartViewingAddOns()
    {
        foreach (ATurretViewAddOn viewAddOn in _dynamicViewAddOns)
        {
            viewAddOn.StartViewing(_addOnsParent);
        }
    }
    
    public void StopViewingAddOns()
    {
        foreach (ATurretViewAddOn viewAddOn in _dynamicViewAddOns)
        {
            viewAddOn.StopViewing();
        }
    }
}