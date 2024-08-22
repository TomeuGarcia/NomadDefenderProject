


public class TurretDamageAttack
{
    public readonly ATurretProjectileBehaviour ProjectileSource;
    public readonly Enemy Target;
    public int Damage { get; private set; }
    
    public TurretDamageAttack(ATurretProjectileBehaviour projectileSource, Enemy target, int defaultDamage)
    {
        ProjectileSource = projectileSource;
        Target = target;
        Damage = defaultDamage;
    }

    public void UpdateDamage(int updatedDamage)
    {
        Damage = updatedDamage;
    }
}