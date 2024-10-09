using System;
using System.Collections.Generic;
using Scripts.ObjectPooling;
using UnityEngine;

public class BuildingDisableWave : RecyclableObject
{
    [SerializeField] private MeshRenderer _meshRenderer;
    private Material _mashMaterial;


    private BuildingDisableWaveConfig _config;
    
    private void Awake()
    {
        _mashMaterial = _meshRenderer.material;
    }

    internal override void RecycledInit()
    {
        
    }

    internal override void RecycledReleased()
    {
        
    }

    public void Init(BuildingDisableWaveConfig config)
    {
        _config = config;
    }

    private void ApplyWaveEffect()
    {
        Collider[] collidersInRange = Physics.OverlapSphere(transform.position, _config.Radius, 
            _config.BuildingsLayerMask, QueryTriggerInteraction.Collide);

        foreach (Collider colliderInRange in collidersInRange)
        {
            if (colliderInRange.TryGetComponent(out IDisableableBuilding disableableBuilding))
            {
                BuildingDisableManager.Instance.HandleNewBuilding(disableableBuilding, _config.DisableDuration);
            }
        }
    }
}