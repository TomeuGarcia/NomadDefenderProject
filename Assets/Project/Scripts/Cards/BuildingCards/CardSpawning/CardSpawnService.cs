using System;
using UnityEngine;

public class CardSpawnService : MonoBehaviour, ICardSpawnService
{
    [SerializeField] private CardSpawnServiceConfig _config;

    private void Start()
    {
        ServiceLocator.GetInstance().CardSpawnService = this;
        DontDestroyOnLoad(gameObject);
    }


    public TurretBuilding GetTurretBuildingPrefab()
    {
        return _config.TurretPrefab;
    }

    public SupportBuilding GetSupportBuildingPrefab()
    {
        return _config.SupportPrefab;
    }

    
    
    public BuildingCard[] MakeAllCardsFromDeck(CardDeckContent deckContent, Transform parent,
        bool toPermanentlyModifyCards)
    {
        TurretCardData[] turretCardsData = deckContent.TurretCardsData;
        SupportCardData[] supportCardsData = deckContent.SupportCardsData;

        BuildingCard[] buildingCards = new BuildingCard[turretCardsData.Length + supportCardsData.Length];

        for (int cardsI = 0; cardsI < turretCardsData.Length; ++cardsI)
        {
            TurretCardData cardData = toPermanentlyModifyCards
                ? turretCardsData[cardsI]
                : new TurretCardData(turretCardsData[cardsI], true, true);
            buildingCards[cardsI] = MakeNewTurretCard_FromData(cardData, parent);
        }
        for (int cardsI = turretCardsData.Length, i = 0; i < supportCardsData.Length; ++cardsI, ++i)
        {
            buildingCards[cardsI] = MakeNewSupportCard_FromData(supportCardsData[i], parent);
        }
        
        return buildingCards;
    }

        
    
    public TurretBuildingCard MakeNewTurretCard_FromData(TurretCardData cardData, Transform parent)
    {
        TurretBuildingCard turretCard = Instantiate(_config.TurretCardPrefab, parent);
        turretCard.InitWithData(cardData);
        return turretCard;
    }
    private SupportBuildingCard MakeNewSupportCard_FromData(SupportCardData cardData, Transform parent)
    {
        SupportBuildingCard supportCard = Instantiate(_config.SupportCardPrefab, parent);
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