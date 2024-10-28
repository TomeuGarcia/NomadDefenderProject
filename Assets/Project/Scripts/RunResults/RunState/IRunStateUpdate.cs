public interface IRunStateUpdate 
{
    void IncrementNodesReached();
    void IncrementPlacedBuildings();
    void IncrementUpgradedBuildings();
    void AddDamageDealt(int damageDealt);
    void AddDamageTaken(int damageTaken, EnemyTypeConfig attacker);
    
}