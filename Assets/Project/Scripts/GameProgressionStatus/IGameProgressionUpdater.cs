public interface IGameProgressionUpdater
{
    void ResetEverything();
    void IncrementVictoryCount(CardDeckAsset starterDeck);
}