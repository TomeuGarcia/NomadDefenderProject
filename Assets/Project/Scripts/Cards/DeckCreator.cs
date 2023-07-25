using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckCreator : MonoBehaviour
{
    [SerializeField] private bool spawnCardsOnAwake = true;
    [SerializeField] private Transform spawnTransform;
    [SerializeField] private GameObject turretCardPrefab;
    [SerializeField] private GameObject supportCardPrefab;
    [SerializeField] private DeckData deckData;
    
    private BuildingCard[] starterCards;

    private bool saveOnDisable = true;


    private void Awake()
    {
        if (spawnCardsOnAwake)
        {
            SpawnDefaultDeckDataCards();
        }
    }

    private void OnDisable()
    {
        if (saveOnDisable)
        {
            deckData.Save();
        }
    }


    public void SpawnDefaultDeckDataCards()
    {
        SpawnCardsAndResetDeckData(deckData, out starterCards);
    }

    public void SpawnCardsAndResetDeckData(DeckData deckData, out BuildingCard[] buildingCards)
    {
        int turretCardNum = deckData.starterTurretCardsComponents.Count;
        int supportCardNum = deckData.starterSupportCardsComponents.Count;
        buildingCards = new BuildingCard[turretCardNum + supportCardNum];

        for (int i = 0; i < turretCardNum; ++i)
        {
            TurretBuildingCard card = GetUninitializedNewTurretCard();
            card.ResetParts(deckData.starterTurretCardsComponents[i]);

            buildingCards[i] = card;
        }
        for (int i = turretCardNum; i < buildingCards.Length; ++i)
        {
            SupportBuildingCard card = GetUninitializedNewSupportCard();
            card.ResetParts(deckData.starterSupportCardsComponents[i - turretCardNum]);

            buildingCards[i] = card;
        }

        deckData.Init(buildingCards);
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


    public void DisableSaveOnDisable()
    {
        saveOnDisable = false;
    }

}
