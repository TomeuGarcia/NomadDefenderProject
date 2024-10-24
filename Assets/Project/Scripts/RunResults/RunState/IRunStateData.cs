public interface IRunStateData 
{
    CardDeckInUseData DeckInUse { get; }
    float RunDuration { get; }
    int NodesReached { get; }
    
    int TotalBuildingsPlaced { get; }
    int TotalBuildingsUpgraded { get; }
    
    
    int TotalDamageDealt { get; }
    int HighestDamageDealt { get; }
    
    int TotalDamageTaken { get; }
}