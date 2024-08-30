using UnityEngine;

public abstract class AProjectileShootingController
{
    public class CreateData
    {
        public readonly TurretBuilding TurretOwner;
        public readonly TurretStatsSnapshot Stats;
        public readonly TurretPartProjectileDataModel ProjectileDataModel;
        public readonly TurretPartBody_Prefab Body;
        public readonly ITurretShootingLifetimeCycle TurretShootingLifetimeCycle;

        public CreateData(TurretBuilding turretOwner, TurretStatsSnapshot stats, 
            TurretPartProjectileDataModel projectileDataModel, TurretPartBody_Prefab body,
            ITurretShootingLifetimeCycle turretShootingLifetimeCycle)
        {
            TurretOwner = turretOwner;
            Stats = stats;
            ProjectileDataModel = projectileDataModel;
            Body = body;
            TurretShootingLifetimeCycle = turretShootingLifetimeCycle;
        }
    }
    
    
    
    
    protected readonly TurretBuilding _turretOwner;
    private readonly TurretPartProjectileDataModel _projectileDataModel;
    private readonly TurretPartBody_Prefab _body;
    private readonly ITurretShootingLifetimeCycle _turretShootingLifetimeCycle;
    
    public float TimeSinceLastShot { get; protected set; }
    public Vector3 LastTargetedPosition { get; protected set; }
    public abstract void UpdateShoot();


    protected AProjectileShootingController(CreateData createData)
    {
        _turretOwner = createData.TurretOwner;
        _projectileDataModel = createData.ProjectileDataModel;
        _body = createData.Body;
        _turretShootingLifetimeCycle = createData.TurretShootingLifetimeCycle;
    }
    
    protected ATurretProjectileBehaviour Shoot(Enemy targetedEnemy)
    {
        Vector3 shootPoint = _body.GetNextShootingPoint();
        ATurretProjectileBehaviour currentAttack = ProjectileAttacksFactory.GetInstance()
            .GetAttackGameObject(_projectileDataModel.ProjectileType, shootPoint, Quaternion.identity)
            .GetComponent<ATurretProjectileBehaviour>();

        //currentAttack.transform.parent = _turretOwner.transform;
        currentAttack.gameObject.SetActive(true);
        currentAttack.ProjectileShotInit(_turretShootingLifetimeCycle, targetedEnemy, _turretOwner);


        // Spawn particle (shoot particles are the same as hit particles)
        GameObject particles = ProjectileParticleFactory.GetInstance()
            .CreateParticlesGameObject(currentAttack.HitParticlesType, shootPoint, _body.transform.rotation);
        //particles.transform.parent = _turretOwner.transform;


        // Audio
        GameAudioManager.GetInstance().PlayProjectileShot(_body.BodyType);
        _turretOwner.OnEnemyShot(targetedEnemy);

        return currentAttack;
    }
    
    public virtual void ClearAllProjectiles() { }
}