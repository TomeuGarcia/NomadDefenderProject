


public class TurretDamageAttack
{
    public readonly ATurretProjectileBehaviour ProjectileSource;
    public Enemy Target { get; private set; }
    public int Damage { get; private set; }
    public bool IsQueuedDamage { get; private set; }
    
    public TurretDamageAttack(ATurretProjectileBehaviour projectileSource, Enemy target, int defaultDamage,
        bool isQueuedDamage)
    {
        ProjectileSource = projectileSource;
        Target = target;
        Damage = defaultDamage;
        IsQueuedDamage = isQueuedDamage;
    }

    public void UpdateDamage(int updatedDamage)
    {
        Damage = updatedDamage;
    }
    public void UpdateTarget(Enemy enemyTarget)
    {
        Target = enemyTarget;
    }
}