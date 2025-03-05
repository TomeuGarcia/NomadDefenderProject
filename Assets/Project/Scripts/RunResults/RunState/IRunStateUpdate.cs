public interface IRunStateUpdate 
{
    void IncrementBattleVictories(bool perfectDefense);
    void IncrementNodesReached();
    void IncrementDestroyedNodes();
    void IncrementPlacedBuildings();
    void IncrementUpgradedBuildings();
    void AddDamageDealt(TurretDamageAttack damageAttackDealt);
    void AddDamageTaken(int damageTaken, EnemyTypeConfig attacker);
    
}