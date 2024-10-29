public interface IRunStateUpdate 
{
    void IncrementNodesReached();
    void IncrementDestroyedNodes();
    void IncrementPlacedBuildings();
    void IncrementUpgradedBuildings();
    void AddDamageDealt(TurretDamageAttack damageAttackDealt);
    void AddDamageTaken(int damageTaken, EnemyTypeConfig attacker);
    
}