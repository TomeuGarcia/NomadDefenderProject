
public interface ITurretPassiveAbilitiesNotifier
{

    void OnTurretCreated(TurretBuilding turretOwner);
    void OnTurretDestroyed();
    
    void OnTurretPlacingStart();
    void OnTurretPlacingFinish();
    void OnTurretPlacingMove();
    void OnTurretPlaced();

    void OnBeforeShootingEnemy(TurretDamageAttack damageAttack);
    void OnAfterShootingEnemy(TurretDamageAttackResult damageAttackResult);
}