public interface ITurretShootingLifetimeCycle
{
    void OnBeforeShootingEnemy();
    void OnBeforeDamagingEnemy(TurretDamageAttack damageAttack);
    void OnAfterDamagingEnemy(TurretDamageAttackResult damageAttackResult);
}