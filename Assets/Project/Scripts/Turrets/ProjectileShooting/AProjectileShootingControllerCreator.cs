using UnityEngine;

public abstract class AProjectileShootingControllerCreator : ScriptableObject
{
    public abstract AProjectileShootingController Create(AProjectileShootingController.CreateData createData);
}