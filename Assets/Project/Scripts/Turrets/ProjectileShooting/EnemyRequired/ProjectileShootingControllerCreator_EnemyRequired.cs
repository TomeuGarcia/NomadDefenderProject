
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileShootingControllerCreator_EnemyRequired", 
    menuName = SOAssetPaths.TURRET_PARTS_PROJECTILE_SHOOTING + "EnemyRequired")]
public class ProjectileShootingControllerCreator_EnemyRequired : AProjectileShootingControllerCreator
{
    public override AProjectileShootingController Create(AProjectileShootingController.CreateData createData)
    {
        return new ProjectileShootingController_EnemyRequired(createData);
    }
}