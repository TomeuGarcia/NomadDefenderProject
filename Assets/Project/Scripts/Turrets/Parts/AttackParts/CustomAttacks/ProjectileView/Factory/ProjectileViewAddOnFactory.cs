using System;
using System.Collections.Generic;
using Scripts.ObjectPooling;
using UnityEngine;

public class ProjectileViewAddOnFactory : MonoBehaviour, IProjectileViewAddOnFactory
{
    [SerializeField] private ProjectileViewAddOnConfig[] _addOnConfigs;

    private Dictionary<ProjectileViewAddOnConfig, ObjectPool> _addOnConfigToPool;


    private void Start()
    {
        _addOnConfigToPool = new Dictionary<ProjectileViewAddOnConfig, ObjectPool>(_addOnConfigs.Length);
        foreach (ProjectileViewAddOnConfig addOnConfig in _addOnConfigs)
        {
            _addOnConfigToPool.Add(addOnConfig, addOnConfig.ObjectPoolData.ToObjectPool(transform));
        }

        ServiceLocator.GetInstance().ProjectileViewAddOnFactory = this;
        
        DontDestroyOnLoad(gameObject);
    }


    public AProjectileViewAddOn CreateAddOn(ProjectileViewAddOnConfig addOnConfig)
    {
        return _addOnConfigToPool[addOnConfig].Spawn<AProjectileViewAddOn>(Vector3.zero, Quaternion.identity);
    }
}