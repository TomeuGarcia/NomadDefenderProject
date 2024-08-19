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


    public BuildingCard[] MakeAllCardsFromDeck(CardDeckContent deckContent)
    {
        TurretCardData[] turretCardsData = deckContent.TurretCardsData;
        SupportCardData[] supportCardsData = deckContent.SupportCardsData;

        BuildingCard[] buildingCards = new BuildingCard[turretCardsData.Length + supportCardsData.Length];

        for (int cardsI = 0; cardsI < turretCardsData.Length; ++cardsI)
        {
            buildingCards[cardsI] = MakeNewTurretCard_FromData(turretCardsData[cardsI]);
        }
        for (int cardsI = turretCardsData.Length, i = 0; i < supportCardsData.Length; ++cardsI, ++i)
        {
            buildingCards[cardsI] = MakeNewSupportCard_FromData(supportCardsData[i]);
        }
        
        return buildingCards;
    }

        
    
    public TurretBuildingCard MakeNewTurretCard_FromData(TurretCardData cardData)
    {
        TurretBuildingCard turretCard = Instantiate(_turretCardPrefab, transform);
        turretCard.InitWithData(cardData);
        return turretCard;
    }
    private SupportBuildingCard MakeNewSupportCard_FromData(SupportCardData cardData)
    {
        SupportBuildingCard supportCard = Instantiate(_supportCardPrefab, transform);
        supportCard.InitWithData(cardData);
        return supportCard;
    }
    
    
    public TurretBuildingCard MakeNewTurretCard_FromDataModel(TurretCardDataModel cardDataModel)
    {
        return MakeNewTurretCard_FromData(new TurretCardData(cardDataModel));
    }
    public SupportBuildingCard MakeNewSupportCard_FromDataModel(SupportCardDataModel cardDataModel)
    {
        return MakeNewSupportCard_FromData(new SupportCardData(cardDataModel));
    }
    

}