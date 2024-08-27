
using UnityEngine;

public interface ICardSpawnService
{
    public BuildingCard[] MakeAllCardsFromDeck(CardDeckContent deckContent, Transform parent);
    public TurretBuildingCard MakeNewTurretCard_FromData(TurretCardData cardData, Transform parent);
    public TurretBuildingCard MakeNewTurretCard_FromDataModel(TurretCardDataModel cardDataModel, Transform parent);
    public SupportBuildingCard MakeNewSupportCard_FromDataModel(SupportCardDataModel cardDataModel, Transform parent);
}