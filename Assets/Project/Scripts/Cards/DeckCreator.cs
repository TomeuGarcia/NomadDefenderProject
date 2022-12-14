using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckCreator : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private DeckData deckData;
    
    private BuildingCard[] starterCards;


    private void Awake()
    {
        starterCards = new BuildingCard[deckData.starterTurretCardsComponents.Count];

        for (int i = 0; i < starterCards.Length; ++i)
        {
            //BuildingCard card = GetUninitializedNewBuildingCard();
            TurretBuildingCard card = GetUninitializedNewTurretCard();
            card.ResetParts(deckData.starterTurretCardsComponents[i]);

            starterCards[i] = card;
        }

        deckData.Init(starterCards);
    }

    private void OnDisable()
    {
        deckData.Save();
    }

    public BuildingCard GetUninitializedNewBuildingCard()
    {
        return Instantiate(cardPrefab).GetComponent<BuildingCard>();
    }

    public TurretBuildingCard GetUninitializedNewTurretCard()
    {
        return Instantiate(cardPrefab).GetComponent<TurretBuildingCard>();
    }

    public void AddNewTurretCardToDeck(TurretBuildingCard turretCard)
    {
        deckData.AddTurretCard(turretCard);
        deckData.SetStarterCardComponentsAsSaved();
    }


}
