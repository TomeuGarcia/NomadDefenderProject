public interface IRunStateData 
{
    bool Victory { get; }
    CardDeckAsset StarterDeck { get; }
    CardDeckContent DeckContent { get; }
    float RunDuration { get; }
    string RunDurationAsString();
    int NodesReached { get; }
    int DestroyedNodes { get; }
    
    int TotalBuildingsPlaced { get; }
    int TotalBuildingsUpgraded { get; }
    
    
    int TotalDamageDealt { get; }
    int HighestDamageDealt { get; }
    
    int TotalDamageTaken { get; }
    bool MostDamagingEnemy(out EnemyTypeConfig enemyType, out int damage);
}