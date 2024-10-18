

using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "CardDeckInUseData", 
    menuName = SOAssetPaths.CARDS_DECKS + "CardDeckInUseData")]
public class CardDeckInUseData : ScriptableObject
{
    private ICardSpawnService _cardSpawnService;
    private CardDeckAsset _starterDeck;
    private CardDeckContent _currentDeckContent;
    public UnlockableTrophyModel WinTrophyModel { get; private set; }

    public void InitializeForRun(CardDeckAsset starterDeck, ICardSpawnService cardSpawnService,
        UnlockableTrophyModel winTrophyModel)
    {
        _starterDeck = starterDeck;
        _currentDeckContent = _starterDeck.MakeDeckContent();
        _cardSpawnService = cardSpawnService;

        WinTrophyModel = winTrophyModel;
    }

    public void AddTurretCardToDeck(TurretCardDataModel turretDataModel)
    {
        _currentDeckContent.AddTurretCard(turretDataModel);
    }
    
    public void AddSupportCardToDeck(SupportCardDataModel supportDataModel)
    {
        _currentDeckContent.AddSupportCard(supportDataModel);
    }


    public BuildingCard[] SpawnCurrentDeckBuildingCards(Transform cardsParent)
    {
        return _cardSpawnService.MakeAllCardsFromDeck(_currentDeckContent, cardsParent);
    }
    
}