

using UnityEngine;

[CreateAssetMenu(fileName = "CardDeckInUseData", 
    menuName = SOAssetPaths.CARDS_DECKS + "CardDeckInUseData")]
public class CardDeckInUseData : ScriptableObject
{
    private ICardSpawnService _cardSpawnService;
    private CardDeckAsset _starterDeck;
    private CardDeckContent _currentDeckContent;
    

    public void InitializeForRun(CardDeckAsset starterDeck, ICardSpawnService cardSpawnService)
    {
        _starterDeck = starterDeck;
        _currentDeckContent = _starterDeck.MakeDeckContent();
        _cardSpawnService = cardSpawnService;
    }

    public void AddTurretCardToDeck(TurretCardDataModel turretDataModel)
    {
        _currentDeckContent.AddTurretCard(turretDataModel);
    }
    
    public void AddSupportCardToDeck(SupportCardDataModel supportDataModel)
    {
        _currentDeckContent.AddSupportCard(supportDataModel);
    }


    public BuildingCard[] SpawnCurrentDeckBuildingCards()
    {
        return _cardSpawnService.MakeAllCardsFromDeck(_currentDeckContent);
    }
    
}