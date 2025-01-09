public class DefaultProjectileTargetingController : IProjectileTargetingController
{
    private readonly RangeBuilding _ownerBuilding;
    public Enemy TargetedEnemy { get; private set; }


    public DefaultProjectileTargetingController(RangeBuilding ownerBuilding)
    {
        _ownerBuilding = ownerBuilding;
        TargetedEnemy = null;
    }
        
    public bool TargetEnemyExists()
    {
        return TargetedEnemy != null;
    }
        
    public void ComputeNextTargetedEnemy()
    {
        TargetedEnemy = _ownerBuilding.GetBestEnemyTarget(TargetedEnemy);  
    }
}