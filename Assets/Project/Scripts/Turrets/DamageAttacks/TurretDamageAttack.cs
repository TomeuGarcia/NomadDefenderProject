


public class TurretDamageAttack
{
    public readonly TurretPartAttack_Prefab ProjectileSource;
    public readonly Enemy Target;
    public int Damage { get; private set; }
    
    public TurretDamageAttack(TurretPartAttack_Prefab projectileSource, Enemy target, int defaultDamage)
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