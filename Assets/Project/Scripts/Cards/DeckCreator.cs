using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckCreator : MonoBehaviour
{
    [SerializeField] private Transform spawnTransform;
    [SerializeField] private GameObject turretCardPrefab;
    [SerializeField] private GameObject supportCardPrefab;
    [SerializeField] private DeckData deckData;
    
    private BuildingCard[] starterCards;


    private void Awake()
    {
        SpawnCardsAndResetDeckData();
    }

    public void SpawnCardsAndResetDeckData()
    {
        int turretCardNum = deckData.starterTurretCardsComponents.Count;
        int supportCardNum = deckData.starterSupportCardsComponents.Count;
        starterCards = new BuildingCard[turretCardNum + supportCardNum];

        for (int i = 0; i < turretCardNum; ++i)
        {
            //BuildingCard card = GetUninitializedNewBuildingCard();
            TurretBuildingCard card = GetUninitializedNewTurretCard();
            card.ResetParts(deckData.starterTurretCardsComponents[i]);

            starterCards[i] = card;
        }
        for (int i = turretCardNum; i < starterCards.Length; ++i)
        {
            //BuildingCard card = GetUninitializedNewBuildingCard();
            SupportBuildingCard card = GetUninitializedNewSupportCard();
            card.ResetParts(deckData.starterSupportCardsComponents[i - turretCardNum]);

            starterCards[i] = card;
        }

        deckData.Init(starterCards);
    }

    private void OnDisable()
    {
        deckData.Save();
    }

    public TurretBuildingCard GetUninitializedNewTurretCard()
    {
        return Instantiate(turretCardPrefab, spawnTransform).GetComponent<TurretBuildingCard>();
    }

    public void AddNewTurretCardToDeck(TurretBuildingCard turretCard)
    {
        deckData.AddTurretCard(turretCard);
        deckData.SetStarterCardComponentsAsSaved();
    }

    public SupportBuildingCard GetUninitializedNewSupportCard()
    {
        return Instantiate(supportCardPrefab, spawnTransform).GetComponent<SupportBuildingCard>();
    }

    public void AddNewSupportCardToDeck(SupportBuildingCard supportCard)
    {
        deckData.AddSupportCard(supportCard);
        deckData.SetStarterCardComponentsAsSaved();
    }


}
