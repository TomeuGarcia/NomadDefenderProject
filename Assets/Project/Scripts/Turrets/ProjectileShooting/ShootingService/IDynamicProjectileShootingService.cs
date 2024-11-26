public interface IDynamicProjectileShootingService
{
    void SpawnProjectile<T>(TurretBuilding turretBuilding, TurretPartProjectileDataModel projectileDataModel)
        where T : AProjectileShootingController;
    void Clear();
}