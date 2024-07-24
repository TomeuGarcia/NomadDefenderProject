using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.ObjectPooling;


public class GeneralParticleFactory : IGeneralParticleFactory
{
    private GeneralParticleFactoryConfig _config;
    private ObjectPool _buildingUpgradePool;

    public GeneralParticleFactory(Transform particlesParent, GeneralParticleFactoryConfig config)
    {
        _config = config;
        _buildingUpgradePool = config.BuildingUpgrade.MakeInitializedObjectPool(particlesParent);
    }

    public void SpawnTurretUpgradeParticles(TurretUpgradeType upgradeType, Vector3 position, Quaternion rotation)
    {
        RecyclableParticles particles = _buildingUpgradePool.Spawn<RecyclableParticles>(position, rotation);
        particles.SetMaterial(_config.BuildingsUtils.GetUpgradeMaterialByType(upgradeType));
    }

}
