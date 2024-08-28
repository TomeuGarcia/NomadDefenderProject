using System.Collections.Generic;
using UnityEngine;

public class TurretViewAddOnsController : ITurretViewAddOnsController
{
    private readonly Transform _addOnsParent;
    private readonly List<ATurretViewAddOn> _dynamicAddOns;


    public TurretViewAddOnsController(Transform addOnsParent)
    {
        _addOnsParent = addOnsParent;
        _dynamicAddOns = new List<ATurretViewAddOn>();
    }

    public void AddViewAddOnToTurret(TurretViewAddOnConfig viewAddOnConfig)
    {
        ATurretViewAddOn turretViewAddOn = ProjectileParticleFactory.GetInstance().CreateTurretAddOn(viewAddOnConfig);
        turretViewAddOn.StartViewing(_addOnsParent);
    }

    public void OnTurretDestroyed()
    {
        foreach (ATurretViewAddOn turretViewAddOn in _dynamicAddOns)
        {
            turretViewAddOn.StopViewing();
        }
        _dynamicAddOns.Clear();
    }
    
}