public interface IRunStateInitialization
{
    void Init(CardDeckAsset starterDeck, CardDeckContent currentDeckContent);
    void Finish(bool victory);
}