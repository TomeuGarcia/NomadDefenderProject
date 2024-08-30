
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileShootingControllerCreator_ProjectileHoarding", 
    menuName = SOAssetPaths.TURRET_PARTS_PROJECTILE_SHOOTING + "ProjectileHoarding")]
public class ProjectileShootingControllerCreator_ProjectileHoarding : AProjectileShootingControllerCreator
{
    [SerializeField, Min(1)] private int _maxHoardedProjectilesCount = 3;
    
    public override AProjectileShootingController Create(AProjectileShootingController.CreateData createData)
    {
        return new ProjectileShootingController_ProjectileHoarding(createData, _maxHoardedProjectilesCount);
    }
}