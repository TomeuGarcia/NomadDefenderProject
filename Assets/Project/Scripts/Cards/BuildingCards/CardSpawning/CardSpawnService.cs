using System;
using UnityEngine;

public class CardSpawnService : MonoBehaviour, ICardSpawnService
{
    [SerializeField] private TurretBuildingCard _turretCardPrefab;
    [SerializeField] private SupportBuildingCard _supportCardPrefab;

    private void Start()
    {
        ServiceLocator.GetInstance().CardSpawnService = this;
        DontDestroyOnLoad(gameObject);
    }


    public BuildingCard[] MakeAllCardsFromDeck(CardDeckContent deckContent, Transform parent)
    {
        TurretCardData[] turretCardsData = deckContent.TurretCardsData;
        SupportCardData[] supportCardsData = deckContent.SupportCardsData;

        BuildingCard[] buildingCards = new BuildingCard[turretCardsData.Length + supportCardsData.Length];

        for (int cardsI = 0; cardsI < turretCardsData.Length; ++cardsI)
        {
            buildingCards[cardsI] = MakeNewTurretCard_FromData(turretCardsData[cardsI], parent);
        }
        for (int cardsI = turretCardsData.Length, i = 0; i < supportCardsData.Length; ++cardsI, ++i)
        {
            buildingCards[cardsI] = MakeNewSupportCard_FromData(supportCardsData[i], parent);
        }
        
        return buildingCards;
    }

        
    
    public TurretBuildingCard MakeNewTurretCard_FromData(TurretCardData cardData, Transform parent)
    {
        TurretBuildingCard turretCard = Instantiate(_turretCardPrefab, parent);
        turretCard.InitWithData(cardData);
        return turretCard;
    }
    private SupportBuildingCard MakeNewSupportCard_FromData(SupportCardData cardData, Transform parent)
    {
        SupportBuildingCard supportCard = Instantiate(_supportCardPrefab, parent);
        supportCard.InitWithData(cardData);
        return supportCard;
    }
    
    
    public TurretBuildingCard MakeNewTurretCard_FromDataModel(TurretCardDataModel cardDataModel, Transform parent)
    {
        return MakeNewTurretCard_FromData(new TurretCardData(cardDataModel), parent);
    }
    public SupportBuildingCard MakeNewSupportCard_FromDataModel(SupportCardDataModel cardDataModel, Transform parent)
    {
        return MakeNewSupportCard_FromData(new SupportCardData(cardDataModel), parent);
    }
    

}