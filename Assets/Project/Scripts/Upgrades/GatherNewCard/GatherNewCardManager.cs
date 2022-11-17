using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatherNewCardManager : MonoBehaviour
{
    [SerializeField] private PartsLibrary partsLibrary;
    [SerializeField] private DeckCreator deckCreator;

    [SerializeField] private int numCards;
    private BuildingCard[] cards;


    private void Init()
    {
        cards = new BuildingCard[numCards];

        for (int i = 0; i < numCards; i++)
        {
            cards[i] = deckCreator.GetUninitializedNewCard();
            cards[i].ResetParts(partsLibrary.GetRandomTurretPartAttack(),
                                partsLibrary.GetRandomTurretPartBody(),
                                partsLibrary.GetRandomTurretPartBase());
        }

    }


}
