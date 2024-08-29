using UnityEngine;

public interface ITurretProjectileView : ITurretProjectileViewAddOnController
{
    void OnProjectileSpawned();
    void OnProjectileDisappear();
    void OnProjectileHitsTarget(Transform target);
}