public interface ITurretShootingLifetimeCycle
{
    void OnBeforeShootingEnemy(ATurretProjectileBehaviour projectile);
    void OnBeforeDamagingEnemy(TurretDamageAttack damageAttack);
    void OnAfterDamagingEnemy(TurretDamageAttackResult damageAttackResult);
}