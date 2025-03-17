public interface IProjectileTargetingController
{
    Enemy TargetedEnemy { get; }
    bool TargetEnemyExists();
    void ComputeNextTargetedEnemy();
    void ClearTargetedEnemy();
}