using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckCreator : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private DeckData deckData;

    [System.Serializable]
    private struct CardComponents
    {
        public TurretPartAttack turretPartAttack;
        public TurretPartBody turretPartBody;
        public TurretPartBase turretPartBase;
    }

    [SerializeField] private CardComponents[] starterCardsComponents;
    private BuildingCard[] starterCards;




    private void Awake()
    {
        starterCards = new BuildingCard[starterCardsComponents.Length];

        for (int i = 0; i < starterCards.Length; ++i)
        {
            BuildingCard card = Instantiate(cardPrefab).GetComponent<BuildingCard>();
            card.ResetParts(starterCardsComponents[i].turretPartAttack, starterCardsComponents[i].turretPartBody, starterCardsComponents[i].turretPartBase);

            starterCards[i] = card;
        }

        deckData.Init(starterCards);
    }


}
