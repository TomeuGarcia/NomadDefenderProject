using System;
using System.Collections.Generic;

public class DynamicProjectileShootingService : IDynamicProjectileShootingService
{
    private readonly Dictionary<Type, Dictionary<TurretBuilding, AProjectileShootingController>>
        _shootingControllerToTurretMap;


    public DynamicProjectileShootingService()
    {
        _shootingControllerToTurretMap =
            new Dictionary<Type, Dictionary<TurretBuilding, AProjectileShootingController>>();
    }
    

    public void SpawnProjectile<T>(TurretBuilding turretBuilding, TurretPartProjectileDataModel projectileDataModel) 
        where T : AProjectileShootingController
    {
        Type type = typeof(T);
        if (!_shootingControllerToTurretMap.ContainsKey(type))
        {
            _shootingControllerToTurretMap.Add(type, new Dictionary<TurretBuilding, AProjectileShootingController>());
        }

        var map = _shootingControllerToTurretMap[typeof(T)];
        if (!map.ContainsKey(turretBuilding))
        {
            map.Add(turretBuilding, turretBuilding.MakeShootingController(projectileDataModel));
        }
        
        map[turretBuilding].DoShoot();
    }

    public void Clear()
    {
        foreach (var map in _shootingControllerToTurretMap)
        {
            map.Value.Clear();
        }
    }
}