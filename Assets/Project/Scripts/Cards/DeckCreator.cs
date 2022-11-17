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
        starterCards = new BuildingCard[deckData.starterCardsComponents.Count];

        for (int i = 0; i < starterCards.Length; ++i)
        {
            BuildingCard card = GetUninitializedNewCard();
            card.ResetParts(deckData.starterCardsComponents[i].turretPartAttack, 
                            deckData.starterCardsComponents[i].turretPartBody, 
                            deckData.starterCardsComponents[i].turretPartBase);

            starterCards[i] = card;
        }

        deckData.Init(starterCards);
    }

    private void OnDisable()
    {
        deckData.Save();
    }

    public BuildingCard GetUninitializedNewCard()
    {
        return Instantiate(cardPrefab).GetComponent<BuildingCard>();
    }

    public void AddNewCardToDeck(BuildingCard card)
    {
        deckData.AddCard(card);
        deckData.SetStarterCardComponentsAsSaved();
    }

}
