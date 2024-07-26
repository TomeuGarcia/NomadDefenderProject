using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGeneralParticleFactory
{
    void SpawnTurretUpgradeParticles(TurretUpgradeType upgradeType, Vector3 position, Quaternion rotation);
}
