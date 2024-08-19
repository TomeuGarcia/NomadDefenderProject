
using UnityEngine;

public interface ICardSpawnService
{
    public BuildingCard[] MakeAllCardsFromDeck(CardDeckContent deckContent);
    public TurretBuildingCard MakeNewTurretCard_FromData(TurretCardData cardData);
    public TurretBuildingCard MakeNewTurretCard_FromDataModel(TurretCardDataModel cardDataModel);
    public SupportBuildingCard MakeNewSupportCard_FromDataModel(SupportCardDataModel cardDataModel);
}