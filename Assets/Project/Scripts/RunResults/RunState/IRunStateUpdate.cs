public interface IRunStateUpdate : 
    IRunStateUpdate.OverworldMap,
    IRunStateUpdate.BuildingsPlacing,
    IRunStateUpdate.BuildingsUpgrading,
    IRunStateUpdate.DealDamage,
    IRunStateUpdate.TakeDamage
{
    public interface OverworldMap
    {
        void IncrementNodesReached();
    }

    public interface BuildingsPlacing
    {
        void IncrementPlacedBuildings();
    }

    public interface BuildingsUpgrading
    {
        void IncrementUpgradedBuildings();
    }
    
    public interface DealDamage
    {
        void AddDamageDealt(int damageDealt);
    }

    public interface TakeDamage
    {
        void AddDamageTaken(int damageTaken, EnemyTypeConfig attacker);
    }

    
}