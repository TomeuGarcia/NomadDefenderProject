using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckCreator : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private DeckData deckData;
    
    private BuildingCard[] starterCards;


    private List<BuildingCard.CardComponents> cardsComponentsTemp;


    private void Awake()
    {
        starterCards = new BuildingCard[deckData.starterCardsComponents.Count];

        for (int i = 0; i < starterCards.Length; ++i)
        {
            BuildingCard card = Instantiate(cardPrefab).GetComponent<BuildingCard>();
            card.ResetParts(deckData.starterCardsComponents[i].turretPartAttack, 
                            deckData.starterCardsComponents[i].turretPartBody, 
                            deckData.starterCardsComponents[i].turretPartBase);

            starterCards[i] = card;
        }

        deckData.Init(starterCards);

        cardsComponentsTemp = new List<BuildingCard.CardComponents>();
    }

    private void OnDisable()
    {
        deckData.Save();
    }

}
