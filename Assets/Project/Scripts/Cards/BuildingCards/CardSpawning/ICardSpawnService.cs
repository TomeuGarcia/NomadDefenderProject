
using UnityEngine;

public interface ICardSpawnService
{
    TurretBuilding GetTurretBuildingPrefab();
    SupportBuilding GetSupportBuildingPrefab();
    
    
    BuildingCard[] MakeAllCardsFromDeck(CardDeckContent deckContent, Transform parent, bool toPermanentlyModifyCards);
    TurretBuildingCard MakeNewTurretCard_FromData(TurretCardData cardData, Transform parent);
    TurretBuildingCard MakeNewTurretCard_FromDataModel(TurretCardDataModel cardDataModel, Transform parent);
    SupportBuildingCard MakeNewSupportCard_FromDataModel(SupportCardDataModel cardDataModel, Transform parent);
}