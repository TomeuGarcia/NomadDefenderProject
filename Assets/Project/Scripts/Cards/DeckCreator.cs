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
            TurretCard card = GetUninitializedNewTurretCard();
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

    public TurretCard GetUninitializedNewTurretCard()
    {
        return Instantiate(cardPrefab).GetComponent<TurretCard>();
    }

    public void AddNewTurretCardToDeck(TurretCard turretCard)
    {
        deckData.AddTurretCard(turretCard);
        deckData.SetStarterCardComponentsAsSaved();
    }


}
