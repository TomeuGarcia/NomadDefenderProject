public class TurretDamageAttackResult
{
    public bool HitKilled { get; }
    public readonly TurretDamageAttack DamageAttackSource;
    public readonly Enemy Target;
    public readonly int DamageTaken;
    public readonly int ArmorDamageTaken;
    public readonly bool HitArmor;
    public readonly bool BrokeArmor;

    public TurretDamageAttackResult(TurretDamageAttack damageAttackSource,
        Enemy target, int damageTaken, int armorDamageTaken, bool hitArmor, bool brokeArmor, bool hitKilled)
    {
        HitKilled = hitKilled;
        DamageAttackSource = damageAttackSource;
        Target = target;
        DamageTaken = damageTaken;
        ArmorDamageTaken = armorDamageTaken;
        HitArmor = hitArmor;
        BrokeArmor = brokeArmor;

        
        if (damageAttackSource.ProjectileSource != null)
        {
            TurretCardStatistics cardStatistics = damageAttackSource.ProjectileSource.TurretOwner.CardData.Statistics;
            cardStatistics.AddTotalDamageDealt(damageTaken);
            if (hitKilled)
            {
                cardStatistics.IncrementTotalKills();
            }
        }
    }
}